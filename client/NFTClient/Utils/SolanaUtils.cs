using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Text;

namespace NFTClient.Utils
{
    /// <summary>
    /// Solana工具类
    /// Solana utilities
    /// </summary>
    public static class SolanaUtils
    {
        /// <summary>
        /// 查找程序地址
        /// Find program address
        /// </summary>
        /// <param name="seeds">种子 | Seeds</param>
        /// <param name="programId">程序ID | Program ID</param>
        /// <returns>程序地址和种子 | Program address and seed</returns>
        public static (string Address, byte Bump) FindProgramAddress(byte[][] seeds, string programId)
        {
            var (address, bump) = PublicKey.FindProgramAddress(seeds, new PublicKey(programId));
            return (address.ToString(), bump);
        }

        /// <summary>
        /// 创建密钥对
        /// Create key pair
        /// </summary>
        /// <returns>密钥对 | Key pair</returns>
        public static Keypair CreateKeypair()
        {
            return new Keypair();
        }

        /// <summary>
        /// 从私钥创建钱包
        /// Create wallet from private key
        /// </summary>
        /// <param name="privateKey">私钥 | Private key</param>
        /// <returns>钱包 | Wallet</returns>
        public static Wallet CreateWalletFromPrivateKey(string privateKey)
        {
            return new Wallet(privateKey);
        }

        /// <summary>
        /// 从助记词创建钱包
        /// Create wallet from mnemonic
        /// </summary>
        /// <param name="mnemonic">助记词 | Mnemonic</param>
        /// <returns>钱包 | Wallet</returns>
        public static Wallet CreateWalletFromMnemonic(string mnemonic)
        {
            return new Wallet(mnemonic);
        }

        /// <summary>
        /// 生成助记词
        /// Generate mnemonic
        /// </summary>
        /// <returns>助记词 | Mnemonic</returns>
        public static string GenerateMnemonic()
        {
            return new Mnemonic(WordList.English, WordCount.Twelve).ToString();
        }

        /// <summary>
        /// 验证公钥
        /// Validate public key
        /// </summary>
        /// <param name="publicKey">公钥 | Public key</param>
        /// <returns>是否有效 | Whether valid</returns>
        public static bool IsValidPublicKey(string publicKey)
        {
            try
            {
                _ = new PublicKey(publicKey);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// SOL转Lamports
        /// Convert SOL to lamports
        /// </summary>
        /// <param name="sol">SOL</param>
        /// <returns>Lamports</returns>
        public static ulong SolToLamports(decimal sol)
        {
            return (ulong)(sol * 1_000_000_000m);
        }

        /// <summary>
        /// Lamports转SOL
        /// Convert lamports to SOL
        /// </summary>
        /// <param name="lamports">Lamports</param>
        /// <returns>SOL</returns>
        public static decimal LamportsToSol(ulong lamports)
        {
            return (decimal)lamports / 1_000_000_000m;
        }
    }
} 