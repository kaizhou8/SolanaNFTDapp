use borsh::{BorshDeserialize, BorshSerialize};
use solana_program::pubkey::Pubkey;

/// NFT程序指令
/// NFT program instructions
#[derive(BorshSerialize, BorshDeserialize, Debug, Clone)]
pub enum NFTInstruction {
    /// 初始化NFT集合
    /// Initialize NFT collection
    ///
    /// 账户:
    /// Accounts:
    /// 0. `[writable]` 集合账户 (PDA) | Collection account (PDA)
    /// 1. `[signer]` 权限账户 | Authority account
    /// 2. `[]` 系统程序 | System program
    InitializeCollection {
        /// 集合名称
        /// Collection name
        name: String,
        
        /// 集合符号
        /// Collection symbol
        symbol: String,
        
        /// 集合元数据URI
        /// Collection metadata URI
        uri: String,
        
        /// 版税百分比 (0-100)
        /// Royalty percentage (0-100)
        royalty_percentage: u8,
        
        /// 是否可变
        /// Whether metadata can be updated
        is_mutable: bool,
    },
    
    /// 铸造NFT
    /// Mint NFT
    ///
    /// 账户:
    /// Accounts:
    /// 0. `[]` 集合账户 | Collection account
    /// 1. `[writable]` NFT账户 (PDA) | NFT account (PDA)
    /// 2. `[signer]` 权限账户 | Authority account
    /// 3. `[]` 系统程序 | System program
    MintNFT {
        /// 元数据URI
        /// Metadata URI
        metadata_uri: String,
        
        /// 序列号
        /// Serial number
        serial_number: u64,
    },
    
    /// 转移NFT
    /// Transfer NFT
    ///
    /// 账户:
    /// Accounts:
    /// 0. `[writable]` NFT账户 | NFT account
    /// 1. `[signer]` 当前所有者账户 | Current owner account
    /// 2. `[]` 新所有者账户 | New owner account
    TransferNFT {
        /// 新所有者
        /// New owner
        new_owner: Pubkey,
    },
    
    /// 销毁NFT
    /// Burn NFT
    ///
    /// 账户:
    /// Accounts:
    /// 0. `[writable]` NFT账户 | NFT account
    /// 1. `[signer]` 所有者账户 | Owner account
    BurnNFT,
    
    /// 更新NFT元数据
    /// Update NFT metadata
    ///
    /// 账户:
    /// Accounts:
    /// 0. `[writable]` NFT账户 | NFT account
    /// 1. `[signer]` 所有者账户 | Owner account
    /// 2. `[]` 集合账户 | Collection account
    UpdateNFTMetadata {
        /// 新元数据URI
        /// New metadata URI
        new_metadata_uri: String,
    },
    
    /// 更新集合元数据
    /// Update collection metadata
    ///
    /// 账户:
    /// Accounts:
    /// 0. `[writable]` 集合账户 | Collection account
    /// 1. `[signer]` 权限账户 | Authority account
    UpdateCollectionMetadata {
        /// 新集合元数据URI
        /// New collection metadata URI
        new_uri: String,
    },
} 