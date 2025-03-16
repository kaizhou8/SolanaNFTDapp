use borsh::{BorshDeserialize, BorshSerialize};
use solana_program::{
    account_info::{next_account_info, AccountInfo},
    clock::Clock,
    entrypoint,
    entrypoint::ProgramResult,
    msg,
    program_error::ProgramError,
    pubkey::Pubkey,
    system_instruction,
    program::{invoke, invoke_signed},
    sysvar::{rent::Rent, Sysvar},
};

pub mod error;
pub mod instruction;
pub mod state;

use error::NFTError;
use instruction::NFTInstruction;
use state::{
    CollectionAccount, NFTAccount, 
    COLLECTION_ACCOUNT_SIZE, NFT_ACCOUNT_SIZE,
    COLLECTION_SEED_PREFIX, NFT_SEED_PREFIX,
};

// 程序入口点
// Program entry point
entrypoint!(process_instruction);

/// 处理指令
/// Process instruction
pub fn process_instruction(
    program_id: &Pubkey,
    accounts: &[AccountInfo],
    instruction_data: &[u8],
) -> ProgramResult {
    // 解析指令
    // Parse instruction
    let instruction = NFTInstruction::try_from_slice(instruction_data)
        .map_err(|_| NFTError::InvalidInstruction)?;
    
    // 根据指令类型调用相应的处理函数
    // Call the appropriate processing function based on instruction type
    match instruction {
        NFTInstruction::InitializeCollection { 
            name, symbol, uri, royalty_percentage, is_mutable 
        } => {
            msg!("指令: 初始化集合 | Instruction: Initialize Collection");
            process_initialize_collection(
                program_id, accounts, name, symbol, uri, royalty_percentage, is_mutable
            )
        }
        NFTInstruction::MintNFT { metadata_uri, serial_number } => {
            msg!("指令: 铸造NFT | Instruction: Mint NFT");
            process_mint_nft(program_id, accounts, metadata_uri, serial_number)
        }
        NFTInstruction::TransferNFT { new_owner } => {
            msg!("指令: 转移NFT | Instruction: Transfer NFT");
            process_transfer_nft(program_id, accounts, new_owner)
        }
        NFTInstruction::BurnNFT => {
            msg!("指令: 销毁NFT | Instruction: Burn NFT");
            process_burn_nft(program_id, accounts)
        }
        NFTInstruction::UpdateNFTMetadata { new_metadata_uri } => {
            msg!("指令: 更新NFT元数据 | Instruction: Update NFT Metadata");
            process_update_nft_metadata(program_id, accounts, new_metadata_uri)
        }
        NFTInstruction::UpdateCollectionMetadata { new_uri } => {
            msg!("指令: 更新集合元数据 | Instruction: Update Collection Metadata");
            process_update_collection_metadata(program_id, accounts, new_uri)
        }
    }
}

/// 处理初始化集合指令
/// Process initialize collection instruction
fn process_initialize_collection(
    program_id: &Pubkey,
    accounts: &[AccountInfo],
    name: String,
    symbol: String,
    uri: String,
    royalty_percentage: u8,
    is_mutable: bool,
) -> ProgramResult {
    // 获取账户
    // Get accounts
    let account_info_iter = &mut accounts.iter();
    let collection_account = next_account_info(account_info_iter)?;
    let authority = next_account_info(account_info_iter)?;
    let system_program = next_account_info(account_info_iter)?;
    
    // 验证账户
    // Validate accounts
    if !authority.is_signer {
        return Err(ProgramError::MissingRequiredSignature);
    }
    
    // 验证版税百分比
    // Validate royalty percentage
    if royalty_percentage > 100 {
        return Err(NFTError::InvalidRoyaltyPercentage.into());
    }
    
    // 验证集合账户是否已初始化
    // Check if collection account is already initialized
    if !collection_account.data.borrow().iter().all(|&x| x == 0) {
        return Err(NFTError::AlreadyInitialized.into());
    }
    
    // 计算集合PDA
    // Calculate collection PDA
    let collection_seeds = [
        COLLECTION_SEED_PREFIX,
        authority.key.as_ref(),
        name.as_bytes(),
    ];
    let (expected_collection_address, bump_seed) = 
        Pubkey::find_program_address(&collection_seeds, program_id);
    
    // 验证集合地址
    // Validate collection address
    if expected_collection_address != *collection_account.key {
        return Err(ProgramError::InvalidAccountData);
    }
    
    // 创建集合账户
    // Create collection account
    let rent = Rent::get()?;
    let rent_lamports = rent.minimum_balance(COLLECTION_ACCOUNT_SIZE);
    
    // 创建账户
    // Create account
    invoke_signed(
        &system_instruction::create_account(
            authority.key,
            collection_account.key,
            rent_lamports,
            COLLECTION_ACCOUNT_SIZE as u64,
            program_id,
        ),
        &[
            authority.clone(),
            collection_account.clone(),
            system_program.clone(),
        ],
        &[&[
            COLLECTION_SEED_PREFIX,
            authority.key.as_ref(),
            name.as_bytes(),
            &[bump_seed],
        ]],
    )?;
    
    // 获取当前时间戳
    // Get current timestamp
    let clock = Clock::get()?;
    let current_timestamp = clock.unix_timestamp;
    
    // 创建集合数据
    // Create collection data
    let collection_data = CollectionAccount::new(
        *authority.key,
        name,
        symbol,
        uri,
        royalty_percentage,
        is_mutable,
    );
    
    // 序列化并存储数据
    // Serialize and store data
    collection_data.serialize(&mut *collection_account.data.borrow_mut())?;
    
    msg!("NFT集合已初始化 | NFT collection initialized");
    Ok(())
}

/// 处理铸造NFT指令
/// Process mint NFT instruction
fn process_mint_nft(
    program_id: &Pubkey,
    accounts: &[AccountInfo],
    metadata_uri: String,
    serial_number: u64,
) -> ProgramResult {
    // 获取账户
    // Get accounts
    let account_info_iter = &mut accounts.iter();
    let collection_account = next_account_info(account_info_iter)?;
    let nft_account = next_account_info(account_info_iter)?;
    let authority = next_account_info(account_info_iter)?;
    let system_program = next_account_info(account_info_iter)?;
    
    // 验证账户
    // Validate accounts
    if !authority.is_signer {
        return Err(ProgramError::MissingRequiredSignature);
    }
    
    // 验证集合账户
    // Validate collection account
    if collection_account.owner != program_id {
        return Err(ProgramError::IncorrectProgramId);
    }
    
    // 反序列化集合数据
    // Deserialize collection data
    let collection_data = CollectionAccount::try_from_slice(&collection_account.data.borrow())?;
    
    // 验证权限
    // Validate authority
    if collection_data.authority != *authority.key {
        return Err(NFTError::InsufficientAuthority.into());
    }
    
    // 验证NFT账户是否已初始化
    // Check if NFT account is already initialized
    if !nft_account.data.borrow().iter().all(|&x| x == 0) {
        return Err(NFTError::AlreadyInitialized.into());
    }
    
    // 计算NFT PDA
    // Calculate NFT PDA
    let nft_seeds = [
        NFT_SEED_PREFIX,
        collection_account.key.as_ref(),
        &serial_number.to_le_bytes(),
    ];
    let (expected_nft_address, bump_seed) = 
        Pubkey::find_program_address(&nft_seeds, program_id);
    
    // 验证NFT地址
    // Validate NFT address
    if expected_nft_address != *nft_account.key {
        return Err(ProgramError::InvalidAccountData);
    }
    
    // 创建NFT账户
    // Create NFT account
    let rent = Rent::get()?;
    let rent_lamports = rent.minimum_balance(NFT_ACCOUNT_SIZE);
    
    // 创建账户
    // Create account
    invoke_signed(
        &system_instruction::create_account(
            authority.key,
            nft_account.key,
            rent_lamports,
            NFT_ACCOUNT_SIZE as u64,
            program_id,
        ),
        &[
            authority.clone(),
            nft_account.clone(),
            system_program.clone(),
        ],
        &[&[
            NFT_SEED_PREFIX,
            collection_account.key.as_ref(),
            &serial_number.to_le_bytes(),
            &[bump_seed],
        ]],
    )?;
    
    // 获取当前时间戳
    // Get current timestamp
    let clock = Clock::get()?;
    let current_timestamp = clock.unix_timestamp;
    
    // 创建NFT数据
    // Create NFT data
    let nft_data = NFTAccount::new(
        *authority.key,
        *collection_account.key,
        metadata_uri,
        serial_number,
        current_timestamp,
    );
    
    // 序列化并存储数据
    // Serialize and store data
    nft_data.serialize(&mut *nft_account.data.borrow_mut())?;
    
    // 更新集合数据
    // Update collection data
    let mut updated_collection_data = collection_data;
    updated_collection_data.increment_minted();
    updated_collection_data.serialize(&mut *collection_account.data.borrow_mut())?;
    
    msg!("NFT已铸造 | NFT minted");
    Ok(())
}

/// 处理转移NFT指令
/// Process transfer NFT instruction
fn process_transfer_nft(
    program_id: &Pubkey,
    accounts: &[AccountInfo],
    new_owner: Pubkey,
) -> ProgramResult {
    // 获取账户
    // Get accounts
    let account_info_iter = &mut accounts.iter();
    let nft_account = next_account_info(account_info_iter)?;
    let current_owner = next_account_info(account_info_iter)?;
    
    // 验证账户
    // Validate accounts
    if !current_owner.is_signer {
        return Err(ProgramError::MissingRequiredSignature);
    }
    
    // 验证NFT账户
    // Validate NFT account
    if nft_account.owner != program_id {
        return Err(ProgramError::IncorrectProgramId);
    }
    
    // 反序列化NFT数据
    // Deserialize NFT data
    let mut nft_data = NFTAccount::try_from_slice(&nft_account.data.borrow())?;
    
    // 验证所有权
    // Validate ownership
    if nft_data.owner != *current_owner.key {
        return Err(NFTError::InvalidOwner.into());
    }
    
    // 验证NFT是否已铸造
    // Validate NFT is minted
    if !nft_data.is_minted {
        return Err(NFTError::NotMinted.into());
    }
    
    // 获取当前时间戳
    // Get current timestamp
    let clock = Clock::get()?;
    let current_timestamp = clock.unix_timestamp;
    
    // 转移NFT
    // Transfer NFT
    nft_data.transfer(new_owner, current_timestamp);
    
    // 序列化并存储数据
    // Serialize and store data
    nft_data.serialize(&mut *nft_account.data.borrow_mut())?;
    
    msg!("NFT已转移 | NFT transferred");
    Ok(())
}

/// 处理销毁NFT指令
/// Process burn NFT instruction
fn process_burn_nft(
    program_id: &Pubkey,
    accounts: &[AccountInfo],
) -> ProgramResult {
    // 获取账户
    // Get accounts
    let account_info_iter = &mut accounts.iter();
    let nft_account = next_account_info(account_info_iter)?;
    let owner = next_account_info(account_info_iter)?;
    
    // 验证账户
    // Validate accounts
    if !owner.is_signer {
        return Err(ProgramError::MissingRequiredSignature);
    }
    
    // 验证NFT账户
    // Validate NFT account
    if nft_account.owner != program_id {
        return Err(ProgramError::IncorrectProgramId);
    }
    
    // 反序列化NFT数据
    // Deserialize NFT data
    let nft_data = NFTAccount::try_from_slice(&nft_account.data.borrow())?;
    
    // 验证所有权
    // Validate ownership
    if nft_data.owner != *owner.key {
        return Err(NFTError::InvalidOwner.into());
    }
    
    // 验证NFT是否已铸造
    // Validate NFT is minted
    if !nft_data.is_minted {
        return Err(NFTError::NotMinted.into());
    }
    
    // 关闭账户并退还租金
    // Close account and refund rent
    let dest_starting_lamports = owner.lamports();
    **owner.lamports.borrow_mut() = dest_starting_lamports
        .checked_add(nft_account.lamports())
        .ok_or(NFTError::Overflow)?;
    **nft_account.lamports.borrow_mut() = 0;
    
    // 清空数据
    // Clear data
    let mut data = nft_account.data.borrow_mut();
    for byte in data.iter_mut() {
        *byte = 0;
    }
    
    msg!("NFT已销毁 | NFT burned");
    Ok(())
}

/// 处理更新NFT元数据指令
/// Process update NFT metadata instruction
fn process_update_nft_metadata(
    program_id: &Pubkey,
    accounts: &[AccountInfo],
    new_metadata_uri: String,
) -> ProgramResult {
    // 获取账户
    // Get accounts
    let account_info_iter = &mut accounts.iter();
    let nft_account = next_account_info(account_info_iter)?;
    let owner = next_account_info(account_info_iter)?;
    let collection_account = next_account_info(account_info_iter)?;
    
    // 验证账户
    // Validate accounts
    if !owner.is_signer {
        return Err(ProgramError::MissingRequiredSignature);
    }
    
    // 验证NFT账户
    // Validate NFT account
    if nft_account.owner != program_id {
        return Err(ProgramError::IncorrectProgramId);
    }
    
    // 验证集合账户
    // Validate collection account
    if collection_account.owner != program_id {
        return Err(ProgramError::IncorrectProgramId);
    }
    
    // 反序列化NFT数据
    // Deserialize NFT data
    let mut nft_data = NFTAccount::try_from_slice(&nft_account.data.borrow())?;
    
    // 验证所有权
    // Validate ownership
    if nft_data.owner != *owner.key {
        return Err(NFTError::InvalidOwner.into());
    }
    
    // 验证NFT是否已铸造
    // Validate NFT is minted
    if !nft_data.is_minted {
        return Err(NFTError::NotMinted.into());
    }
    
    // 验证集合
    // Validate collection
    if nft_data.collection != *collection_account.key {
        return Err(ProgramError::InvalidAccountData);
    }
    
    // 反序列化集合数据
    // Deserialize collection data
    let collection_data = CollectionAccount::try_from_slice(&collection_account.data.borrow())?;
    
    // 验证元数据是否可变
    // Validate metadata is mutable
    if !collection_data.is_mutable {
        return Err(NFTError::ImmutableMetadata.into());
    }
    
    // 获取当前时间戳
    // Get current timestamp
    let clock = Clock::get()?;
    let current_timestamp = clock.unix_timestamp;
    
    // 更新元数据
    // Update metadata
    nft_data.update_metadata(new_metadata_uri, current_timestamp);
    
    // 序列化并存储数据
    // Serialize and store data
    nft_data.serialize(&mut *nft_account.data.borrow_mut())?;
    
    msg!("NFT元数据已更新 | NFT metadata updated");
    Ok(())
}

/// 处理更新集合元数据指令
/// Process update collection metadata instruction
fn process_update_collection_metadata(
    program_id: &Pubkey,
    accounts: &[AccountInfo],
    new_uri: String,
) -> ProgramResult {
    // 获取账户
    // Get accounts
    let account_info_iter = &mut accounts.iter();
    let collection_account = next_account_info(account_info_iter)?;
    let authority = next_account_info(account_info_iter)?;
    
    // 验证账户
    // Validate accounts
    if !authority.is_signer {
        return Err(ProgramError::MissingRequiredSignature);
    }
    
    // 验证集合账户
    // Validate collection account
    if collection_account.owner != program_id {
        return Err(ProgramError::IncorrectProgramId);
    }
    
    // 反序列化集合数据
    // Deserialize collection data
    let mut collection_data = CollectionAccount::try_from_slice(&collection_account.data.borrow())?;
    
    // 验证权限
    // Validate authority
    if collection_data.authority != *authority.key {
        return Err(NFTError::InsufficientAuthority.into());
    }
    
    // 验证元数据是否可变
    // Validate metadata is mutable
    if !collection_data.is_mutable {
        return Err(NFTError::ImmutableMetadata.into());
    }
    
    // 更新元数据
    // Update metadata
    collection_data.uri = new_uri;
    
    // 序列化并存储数据
    // Serialize and store data
    collection_data.serialize(&mut *collection_account.data.borrow_mut())?;
    
    msg!("集合元数据已更新 | Collection metadata updated");
    Ok(())
} 