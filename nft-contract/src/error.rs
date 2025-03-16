use solana_program::program_error::ProgramError;
use thiserror::Error;

/// NFT程序错误类型
/// NFT program error types
#[derive(Error, Debug, Copy, Clone)]
pub enum NFTError {
    /// 无效的指令
    /// Invalid instruction
    #[error("无效的指令 | Invalid instruction")]
    InvalidInstruction,

    /// 无效的所有者
    /// Invalid owner
    #[error("无效的所有者 | Invalid owner")]
    InvalidOwner,

    /// 账户未初始化
    /// Account not initialized
    #[error("账户未初始化 | Account not initialized")]
    NotInitialized,

    /// 账户已初始化
    /// Account already initialized
    #[error("账户已初始化 | Account already initialized")]
    AlreadyInitialized,

    /// 数据溢出
    /// Overflow
    #[error("数据溢出 | Overflow")]
    Overflow,

    /// 权限不足
    /// Insufficient authority
    #[error("权限不足 | Insufficient authority")]
    InsufficientAuthority,

    /// NFT已铸造
    /// NFT already minted
    #[error("NFT已铸造 | NFT already minted")]
    AlreadyMinted,

    /// NFT未铸造
    /// NFT not minted
    #[error("NFT未铸造 | NFT not minted")]
    NotMinted,

    /// 无效的元数据URI
    /// Invalid metadata URI
    #[error("无效的元数据URI | Invalid metadata URI")]
    InvalidMetadataUri,

    /// 无效的版税百分比
    /// Invalid royalty percentage
    #[error("无效的版税百分比 | Invalid royalty percentage")]
    InvalidRoyaltyPercentage,

    /// 不可变的元数据
    /// Immutable metadata
    #[error("不可变的元数据 | Immutable metadata")]
    ImmutableMetadata,
}

impl From<NFTError> for ProgramError {
    fn from(e: NFTError) -> Self {
        ProgramError::Custom(e as u32)
    }
} 