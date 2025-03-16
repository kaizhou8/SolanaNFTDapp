using NFTClient.Models;
using Solnet.Programs;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NFTClient.Services
{
    /// <summary>
    /// NFT操作服务
    /// NFT operation service
    /// </summary>
    public class NFTService
    {
        private readonly SolanaService _solanaService;
        private readonly PublicKey _programId;
        private readonly Wallet _wallet;
        private ulong _nextSerialNumber = 1;

        // 指令类型
        // Instruction types
        private const byte INITIALIZE_COLLECTION = 0;
        private const byte MINT_NFT = 1;
        private const byte TRANSFER_NFT = 2;
        private const byte BURN_NFT = 3;
        private const byte UPDATE_NFT_METADATA = 4;
        private const byte UPDATE_COLLECTION_METADATA = 5;

        // 种子前缀
        // Seed prefixes
        private readonly byte[] COLLECTION_SEED_PREFIX = Encoding.UTF8.GetBytes("collection");
        private readonly byte[] NFT_SEED_PREFIX = Encoding.UTF8.GetBytes("nft");

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="solanaService">Solana服务 | Solana service</param>
        /// <param name="programId">程序ID | Program ID</param>
        /// <param name="wallet">钱包 | Wallet</param>
        public NFTService(SolanaService solanaService, string programId, Wallet wallet)
        {
            _solanaService = solanaService;
            _programId = new PublicKey(programId);
            _wallet = wallet;
        }

        /// <summary>
        /// 初始化NFT集合
        /// Initialize NFT collection
        /// </summary>
        /// <param name="metadata">元数据 | Metadata</param>
        /// <returns>交易签名 | Transaction signature</returns>
        public async Task<string> InitializeCollection(NFTMetadata metadata)
        {
            Console.WriteLine($"初始化NFT集合: {metadata.Name} | Initializing NFT collection: {metadata.Name}");
            
            // 计算集合PDA
            // Calculate collection PDA
            var collectionSeeds = new List<byte[]>
            {
                COLLECTION_SEED_PREFIX,
                Encoding.UTF8.GetBytes(_wallet.Account.PublicKey),
                Encoding.UTF8.GetBytes(metadata.Name)
            };
            
            var collectionAddress = FindProgramAddress(collectionSeeds.ToArray(), _programId);
            Console.WriteLine($"集合地址: {collectionAddress} | Collection address: {collectionAddress}");
            
            // 创建指令数据
            // Create instruction data
            var instructionData = new List<byte> { INITIALIZE_COLLECTION };
            
            // 添加名称
            // Add name
            AddStringToData(instructionData, metadata.Name);
            
            // 添加符号
            // Add symbol
            AddStringToData(instructionData, metadata.Symbol);
            
            // 添加URI
            // Add URI
            AddStringToData(instructionData, metadata.Image);
            
            // 添加版税百分比
            // Add royalty percentage
            instructionData.Add((byte)(metadata.SellerFeeBasisPoints / 100)); // Convert basis points to percentage
            
            // 添加是否可变
            // Add is mutable
            instructionData.Add(1); // 1 = true, 0 = false
            
            // 创建指令
            // Create instruction
            var instruction = new TransactionInstruction
            {
                ProgramId = _programId,
                Keys = new List<AccountMeta>
                {
                    AccountMeta.Writable(new PublicKey(collectionAddress), false),
                    AccountMeta.Writable(new PublicKey(_wallet.Account.PublicKey), true),
                    AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false)
                },
                Data = instructionData.ToArray()
            };
            
            // 创建并发送交易
            // Create and send transaction
            var transaction = await _solanaService.CreateTransaction(new List<TransactionInstruction> { instruction });
            var signature = await _solanaService.SendTransaction(transaction);
            
            Console.WriteLine($"交易签名: {signature} | Transaction signature: {signature}");
            
            // 等待交易确认
            // Wait for transaction confirmation
            var confirmed = await _solanaService.ConfirmTransaction(signature);
            
            if (confirmed)
            {
                Console.WriteLine("集合初始化成功 | Collection initialized successfully");
                return signature;
            }
            
            throw new Exception("集合初始化失败 | Collection initialization failed");
        }

        /// <summary>
        /// 铸造NFT
        /// Mint NFT
        /// </summary>
        /// <param name="collectionAddress">集合地址 | Collection address</param>
        /// <param name="metadataUri">元数据URI | Metadata URI</param>
        /// <returns>交易签名 | Transaction signature</returns>
        public async Task<string> MintNFT(string collectionAddress, string metadataUri)
        {
            Console.WriteLine($"铸造NFT: {metadataUri} | Minting NFT: {metadataUri}");
            
            // 计算NFT PDA
            // Calculate NFT PDA
            var serialNumberBytes = BitConverter.GetBytes(_nextSerialNumber);
            var nftSeeds = new List<byte[]>
            {
                NFT_SEED_PREFIX,
                Encoding.UTF8.GetBytes(collectionAddress),
                serialNumberBytes
            };
            
            var nftAddress = FindProgramAddress(nftSeeds.ToArray(), _programId);
            Console.WriteLine($"NFT地址: {nftAddress} | NFT address: {nftAddress}");
            
            // 创建指令数据
            // Create instruction data
            var instructionData = new List<byte> { MINT_NFT };
            
            // 添加元数据URI
            // Add metadata URI
            AddStringToData(instructionData, metadataUri);
            
            // 添加序列号
            // Add serial number
            foreach (var b in serialNumberBytes)
            {
                instructionData.Add(b);
            }
            
            // 创建指令
            // Create instruction
            var instruction = new TransactionInstruction
            {
                ProgramId = _programId,
                Keys = new List<AccountMeta>
                {
                    AccountMeta.ReadOnly(new PublicKey(collectionAddress), false),
                    AccountMeta.Writable(new PublicKey(nftAddress), false),
                    AccountMeta.Writable(new PublicKey(_wallet.Account.PublicKey), true),
                    AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false)
                },
                Data = instructionData.ToArray()
            };
            
            // 创建并发送交易
            // Create and send transaction
            var transaction = await _solanaService.CreateTransaction(new List<TransactionInstruction> { instruction });
            var signature = await _solanaService.SendTransaction(transaction);
            
            Console.WriteLine($"交易签名: {signature} | Transaction signature: {signature}");
            
            // 等待交易确认
            // Wait for transaction confirmation
            var confirmed = await _solanaService.ConfirmTransaction(signature);
            
            if (confirmed)
            {
                Console.WriteLine("NFT铸造成功 | NFT minted successfully");
                _nextSerialNumber++;
                return signature;
            }
            
            throw new Exception("NFT铸造失败 | NFT minting failed");
        }

        /// <summary>
        /// 转移NFT
        /// Transfer NFT
        /// </summary>
        /// <param name="nftAddress">NFT地址 | NFT address</param>
        /// <param name="newOwner">新所有者 | New owner</param>
        /// <returns>交易签名 | Transaction signature</returns>
        public async Task<string> TransferNFT(string nftAddress, string newOwner)
        {
            Console.WriteLine($"转移NFT: {nftAddress} 到 {newOwner} | Transferring NFT: {nftAddress} to {newOwner}");
            
            // 创建指令数据
            // Create instruction data
            var instructionData = new List<byte> { TRANSFER_NFT };
            
            // 添加新所有者
            // Add new owner
            var newOwnerBytes = Encoding.UTF8.GetBytes(newOwner);
            foreach (var b in newOwnerBytes)
            {
                instructionData.Add(b);
            }
            
            // 创建指令
            // Create instruction
            var instruction = new TransactionInstruction
            {
                ProgramId = _programId,
                Keys = new List<AccountMeta>
                {
                    AccountMeta.Writable(new PublicKey(nftAddress), false),
                    AccountMeta.Writable(new PublicKey(_wallet.Account.PublicKey), true),
                    AccountMeta.ReadOnly(new PublicKey(newOwner), false)
                },
                Data = instructionData.ToArray()
            };
            
            // 创建并发送交易
            // Create and send transaction
            var transaction = await _solanaService.CreateTransaction(new List<TransactionInstruction> { instruction });
            var signature = await _solanaService.SendTransaction(transaction);
            
            Console.WriteLine($"交易签名: {signature} | Transaction signature: {signature}");
            
            // 等待交易确认
            // Wait for transaction confirmation
            var confirmed = await _solanaService.ConfirmTransaction(signature);
            
            if (confirmed)
            {
                Console.WriteLine("NFT转移成功 | NFT transferred successfully");
                return signature;
            }
            
            throw new Exception("NFT转移失败 | NFT transfer failed");
        }

        /// <summary>
        /// 销毁NFT
        /// Burn NFT
        /// </summary>
        /// <param name="nftAddress">NFT地址 | NFT address</param>
        /// <returns>交易签名 | Transaction signature</returns>
        public async Task<string> BurnNFT(string nftAddress)
        {
            Console.WriteLine($"销毁NFT: {nftAddress} | Burning NFT: {nftAddress}");
            
            // 创建指令数据
            // Create instruction data
            var instructionData = new List<byte> { BURN_NFT };
            
            // 创建指令
            // Create instruction
            var instruction = new TransactionInstruction
            {
                ProgramId = _programId,
                Keys = new List<AccountMeta>
                {
                    AccountMeta.Writable(new PublicKey(nftAddress), false),
                    AccountMeta.Writable(new PublicKey(_wallet.Account.PublicKey), true)
                },
                Data = instructionData.ToArray()
            };
            
            // 创建并发送交易
            // Create and send transaction
            var transaction = await _solanaService.CreateTransaction(new List<TransactionInstruction> { instruction });
            var signature = await _solanaService.SendTransaction(transaction);
            
            Console.WriteLine($"交易签名: {signature} | Transaction signature: {signature}");
            
            // 等待交易确认
            // Wait for transaction confirmation
            var confirmed = await _solanaService.ConfirmTransaction(signature);
            
            if (confirmed)
            {
                Console.WriteLine("NFT销毁成功 | NFT burned successfully");
                return signature;
            }
            
            throw new Exception("NFT销毁失败 | NFT burning failed");
        }

        /// <summary>
        /// 更新NFT元数据
        /// Update NFT metadata
        /// </summary>
        /// <param name="nftAddress">NFT地址 | NFT address</param>
        /// <param name="collectionAddress">集合地址 | Collection address</param>
        /// <param name="newMetadataUri">新元数据URI | New metadata URI</param>
        /// <returns>交易签名 | Transaction signature</returns>
        public async Task<string> UpdateNFTMetadata(string nftAddress, string collectionAddress, string newMetadataUri)
        {
            Console.WriteLine($"更新NFT元数据: {nftAddress} | Updating NFT metadata: {nftAddress}");
            
            // 创建指令数据
            // Create instruction data
            var instructionData = new List<byte> { UPDATE_NFT_METADATA };
            
            // 添加新元数据URI
            // Add new metadata URI
            AddStringToData(instructionData, newMetadataUri);
            
            // 创建指令
            // Create instruction
            var instruction = new TransactionInstruction
            {
                ProgramId = _programId,
                Keys = new List<AccountMeta>
                {
                    AccountMeta.Writable(new PublicKey(nftAddress), false),
                    AccountMeta.Writable(new PublicKey(_wallet.Account.PublicKey), true),
                    AccountMeta.ReadOnly(new PublicKey(collectionAddress), false)
                },
                Data = instructionData.ToArray()
            };
            
            // 创建并发送交易
            // Create and send transaction
            var transaction = await _solanaService.CreateTransaction(new List<TransactionInstruction> { instruction });
            var signature = await _solanaService.SendTransaction(transaction);
            
            Console.WriteLine($"交易签名: {signature} | Transaction signature: {signature}");
            
            // 等待交易确认
            // Wait for transaction confirmation
            var confirmed = await _solanaService.ConfirmTransaction(signature);
            
            if (confirmed)
            {
                Console.WriteLine("NFT元数据更新成功 | NFT metadata updated successfully");
                return signature;
            }
            
            throw new Exception("NFT元数据更新失败 | NFT metadata update failed");
        }

        /// <summary>
        /// 更新集合元数据
        /// Update collection metadata
        /// </summary>
        /// <param name="collectionAddress">集合地址 | Collection address</param>
        /// <param name="newUri">新URI | New URI</param>
        /// <returns>交易签名 | Transaction signature</returns>
        public async Task<string> UpdateCollectionMetadata(string collectionAddress, string newUri)
        {
            Console.WriteLine($"更新集合元数据: {collectionAddress} | Updating collection metadata: {collectionAddress}");
            
            // 创建指令数据
            // Create instruction data
            var instructionData = new List<byte> { UPDATE_COLLECTION_METADATA };
            
            // 添加新URI
            // Add new URI
            AddStringToData(instructionData, newUri);
            
            // 创建指令
            // Create instruction
            var instruction = new TransactionInstruction
            {
                ProgramId = _programId,
                Keys = new List<AccountMeta>
                {
                    AccountMeta.Writable(new PublicKey(collectionAddress), false),
                    AccountMeta.Writable(new PublicKey(_wallet.Account.PublicKey), true)
                },
                Data = instructionData.ToArray()
            };
            
            // 创建并发送交易
            // Create and send transaction
            var transaction = await _solanaService.CreateTransaction(new List<TransactionInstruction> { instruction });
            var signature = await _solanaService.SendTransaction(transaction);
            
            Console.WriteLine($"交易签名: {signature} | Transaction signature: {signature}");
            
            // 等待交易确认
            // Wait for transaction confirmation
            var confirmed = await _solanaService.ConfirmTransaction(signature);
            
            if (confirmed)
            {
                Console.WriteLine("集合元数据更新成功 | Collection metadata updated successfully");
                return signature;
            }
            
            throw new Exception("集合元数据更新失败 | Collection metadata update failed");
        }

        /// <summary>
        /// 查找程序地址
        /// Find program address
        /// </summary>
        /// <param name="seeds">种子 | Seeds</param>
        /// <param name="programId">程序ID | Program ID</param>
        /// <returns>程序地址 | Program address</returns>
        private string FindProgramAddress(byte[][] seeds, PublicKey programId)
        {
            var (address, _) = PublicKey.FindProgramAddress(seeds, programId);
            return address.ToString();
        }

        /// <summary>
        /// 添加字符串到数据
        /// Add string to data
        /// </summary>
        /// <param name="data">数据 | Data</param>
        /// <param name="str">字符串 | String</param>
        private void AddStringToData(List<byte> data, string str)
        {
            var strBytes = Encoding.UTF8.GetBytes(str);
            var strLength = BitConverter.GetBytes(strBytes.Length);
            
            // 添加字符串长度
            // Add string length
            foreach (var b in strLength)
            {
                data.Add(b);
            }
            
            // 添加字符串内容
            // Add string content
            foreach (var b in strBytes)
            {
                data.Add(b);
            }
        }
    }
} 