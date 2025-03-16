use borsh::{BorshDeserialize, BorshSerialize};
use solana_program::pubkey::Pubkey;

/// NFT集合账户
/// NFT collection account
#[derive(BorshSerialize, BorshDeserialize, Debug, Clone)]
pub struct CollectionAccount {
    /// 集合权限
    /// Collection authority
    pub authority: Pubkey,
    
    /// 集合名称
    /// Collection name
    pub name: String,
    
    /// 集合符号
    /// Collection symbol
    pub symbol: String,
    
    /// 集合元数据URI
    /// Collection metadata URI
    pub uri: String,
    
    /// 版税百分比 (0-100)
    /// Royalty percentage (0-100)
    pub royalty_percentage: u8,
    
    /// 是否可变
    /// Whether metadata can be updated
    pub is_mutable: bool,
    
    /// 已铸造的NFT数量
    /// Number of minted NFTs
    pub total_minted: u64,
}

/// NFT账户
/// NFT account
#[derive(BorshSerialize, BorshDeserialize, Debug, Clone)]
pub struct NFTAccount {
    /// NFT所有者
    /// NFT owner
    pub owner: Pubkey,
    
    /// 所属集合
    /// Collection this NFT belongs to
    pub collection: Pubkey,
    
    /// 元数据URI
    /// Metadata URI
    pub metadata_uri: String,
    
    /// 是否已铸造
    /// Whether the NFT is minted
    pub is_minted: bool,
    
    /// 序列号
    /// Serial number within collection
    pub serial_number: u64,
    
    /// 创建时间戳
    /// Creation timestamp
    pub created_at: i64,
    
    /// 最后更新时间戳
    /// Last update timestamp
    pub updated_at: i64,
}

/// 集合账户大小常量
/// Collection account size constants
pub const COLLECTION_ACCOUNT_SIZE: usize = 
    32 +                // authority
    4 + 50 +           // name (max 50 chars)
    4 + 10 +           // symbol (max 10 chars)
    4 + 200 +          // uri (max 200 chars)
    1 +                // royalty_percentage
    1 +                // is_mutable
    8;                 // total_minted

/// NFT账户大小常量
/// NFT account size constants
pub const NFT_ACCOUNT_SIZE: usize = 
    32 +                // owner
    32 +                // collection
    4 + 200 +           // metadata_uri (max 200 chars)
    1 +                 // is_minted
    8 +                 // serial_number
    8 +                 // created_at
    8;                  // updated_at

/// 集合种子前缀
/// Collection seed prefix
pub const COLLECTION_SEED_PREFIX: &[u8] = b"collection";

/// NFT种子前缀
/// NFT seed prefix
pub const NFT_SEED_PREFIX: &[u8] = b"nft";

impl CollectionAccount {
    /// 创建新集合
    /// Create a new collection
    pub fn new(
        authority: Pubkey,
        name: String,
        symbol: String,
        uri: String,
        royalty_percentage: u8,
        is_mutable: bool,
    ) -> Self {
        Self {
            authority,
            name,
            symbol,
            uri,
            royalty_percentage,
            is_mutable,
            total_minted: 0,
        }
    }
    
    /// 增加已铸造NFT数量
    /// Increment minted NFT count
    pub fn increment_minted(&mut self) {
        self.total_minted += 1;
    }
}

impl NFTAccount {
    /// 创建新NFT
    /// Create a new NFT
    pub fn new(
        owner: Pubkey,
        collection: Pubkey,
        metadata_uri: String,
        serial_number: u64,
        timestamp: i64,
    ) -> Self {
        Self {
            owner,
            collection,
            metadata_uri,
            is_minted: true,
            serial_number,
            created_at: timestamp,
            updated_at: timestamp,
        }
    }
    
    /// 转移NFT
    /// Transfer NFT
    pub fn transfer(&mut self, new_owner: Pubkey, timestamp: i64) {
        self.owner = new_owner;
        self.updated_at = timestamp;
    }
    
    /// 更新元数据
    /// Update metadata
    pub fn update_metadata(&mut self, new_metadata_uri: String, timestamp: i64) {
        self.metadata_uri = new_metadata_uri;
        self.updated_at = timestamp;
    }
} 