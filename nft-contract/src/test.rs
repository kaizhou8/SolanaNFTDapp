#[cfg(test)]
mod tests {
    use super::*;
    use solana_program::{
        account_info::AccountInfo,
        clock::Clock,
        entrypoint::ProgramResult,
        program_pack::Pack,
        pubkey::Pubkey,
        rent::Rent,
        sysvar::Sysvar,
    };
    use solana_program_test::*;
    use solana_sdk::{
        account::Account,
        signature::{Keypair, Signer},
        transaction::Transaction,
    };
    use std::mem;

    // 测试初始化集合
    // Test initialize collection
    #[test]
    fn test_initialize_collection() {
        let program_id = Pubkey::new_unique();
        let authority_key = Pubkey::new_unique();
        let system_program_key = Pubkey::new_unique();
        
        // 创建集合账户
        // Create collection account
        let collection_seeds = [
            COLLECTION_SEED_PREFIX,
            &authority_key.to_bytes(),
            "Test Collection".as_bytes(),
        ];
        let (collection_key, _) = Pubkey::find_program_address(&collection_seeds, &program_id);
        
        let mut collection_account_data = vec![0; COLLECTION_ACCOUNT_SIZE];
        let mut lamports = 0;
        
        let collection_account = AccountInfo::new(
            &collection_key,
            false,
            true,
            &mut lamports,
            &mut collection_account_data,
            &program_id,
            false,
            0,
        );
        
        let authority_account = AccountInfo::new(
            &authority_key,
            true,
            false,
            &mut 0,
            &mut vec![],
            &system_program_key,
            false,
            0,
        );
        
        let system_program_account = AccountInfo::new(
            &system_program_key,
            false,
            false,
            &mut 0,
            &mut vec![],
            &system_program_key,
            false,
            0,
        );
        
        let accounts = vec![
            collection_account,
            authority_account,
            system_program_account,
        ];
        
        // 创建指令数据
        // Create instruction data
        let instruction = NFTInstruction::InitializeCollection {
            name: "Test Collection".to_string(),
            symbol: "TEST".to_string(),
            uri: "https://test.com".to_string(),
            royalty_percentage: 5,
            is_mutable: true,
        };
        
        let mut instruction_data = vec![];
        instruction.serialize(&mut instruction_data).unwrap();
        
        // 执行指令
        // Execute instruction
        let result = process_instruction(&program_id, &accounts, &instruction_data);
        
        // 验证结果
        // Verify result
        assert!(result.is_ok());
    }
    
    // 测试铸造NFT
    // Test mint NFT
    #[test]
    fn test_mint_nft() {
        // 类似于上面的测试，但针对铸造NFT指令
        // Similar to the above test, but for the mint NFT instruction
    }
    
    // 测试转移NFT
    // Test transfer NFT
    #[test]
    fn test_transfer_nft() {
        // 类似于上面的测试，但针对转移NFT指令
        // Similar to the above test, but for the transfer NFT instruction
    }
    
    // 测试销毁NFT
    // Test burn NFT
    #[test]
    fn test_burn_nft() {
        // 类似于上面的测试，但针对销毁NFT指令
        // Similar to the above test, but for the burn NFT instruction
    }
    
    // 测试更新NFT元数据
    // Test update NFT metadata
    #[test]
    fn test_update_nft_metadata() {
        // 类似于上面的测试，但针对更新NFT元数据指令
        // Similar to the above test, but for the update NFT metadata instruction
    }
    
    // 测试更新集合元数据
    // Test update collection metadata
    #[test]
    fn test_update_collection_metadata() {
        // 类似于上面的测试，但针对更新集合元数据指令
        // Similar to the above test, but for the update collection metadata instruction
    }
} 