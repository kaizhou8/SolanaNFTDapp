using NFTClient.Models;
using NFTClient.Services;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace NFTClient
{
    /// <summary>
    /// NFT客户端主程序
    /// NFT client main program
    /// </summary>
    class Program
    {
        // Solana RPC URL
        private const string DevnetUrl = "https://api.devnet.solana.com";
        private const string MainnetUrl = "https://api.mainnet-beta.solana.com";
        
        // 配置文件路径
        // Configuration file path
        private const string ConfigFilePath = "config.json";
        
        // 程序ID
        // Program ID
        private static string _programId;
        
        // 私钥
        // Private key
        private static string _privateKey;
        
        // 是否使用开发网
        // Whether to use devnet
        private static bool _useDevnet = true;
        
        /// <summary>
        /// 主函数
        /// Main function
        /// </summary>
        /// <param name="args">命令行参数 | Command line arguments</param>
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Solana NFT 客户端 | Solana NFT Client");
            Console.WriteLine("====================================");
            
            try
            {
                // 加载配置
                // Load configuration
                LoadConfig();
                
                // 初始化服务
                // Initialize services
                var rpcUrl = _useDevnet ? DevnetUrl : MainnetUrl;
                var solanaService = new SolanaService(rpcUrl, _privateKey);
                var wallet = new Wallet(_privateKey);
                var nftService = new NFTService(solanaService, _programId, wallet);
                
                // 显示钱包信息
                // Display wallet information
                var walletAddress = solanaService.GetWalletAddress();
                var balance = await solanaService.GetBalance();
                Console.WriteLine($"钱包地址: {walletAddress} | Wallet address: {walletAddress}");
                Console.WriteLine($"SOL余额: {balance} | SOL balance: {balance}");
                
                // 显示菜单
                // Display menu
                bool exit = false;
                while (!exit)
                {
                    DisplayMenu();
                    var choice = Console.ReadLine();
                    
                    switch (choice)
                    {
                        case "1":
                            await CreateCollection(nftService);
                            break;
                        case "2":
                            await MintNFT(nftService);
                            break;
                        case "3":
                            await TransferNFT(nftService);
                            break;
                        case "4":
                            await BurnNFT(nftService);
                            break;
                        case "5":
                            await UpdateNFTMetadata(nftService);
                            break;
                        case "6":
                            await UpdateCollectionMetadata(nftService);
                            break;
                        case "7":
                            await RequestAirdrop(solanaService);
                            break;
                        case "8":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("无效的选择，请重试 | Invalid choice, please try again");
                            break;
                    }
                    
                    Console.WriteLine("\n按任意键继续... | Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message} | Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        /// <summary>
        /// 显示菜单
        /// Display menu
        /// </summary>
        private static void DisplayMenu()
        {
            Console.WriteLine("\nSolana NFT 操作菜单 | Solana NFT Operation Menu");
            Console.WriteLine("1. 创建NFT集合 | Create NFT Collection");
            Console.WriteLine("2. 铸造NFT | Mint NFT");
            Console.WriteLine("3. 转移NFT | Transfer NFT");
            Console.WriteLine("4. 销毁NFT | Burn NFT");
            Console.WriteLine("5. 更新NFT元数据 | Update NFT Metadata");
            Console.WriteLine("6. 更新集合元数据 | Update Collection Metadata");
            Console.WriteLine("7. 请求SOL空投 | Request SOL Airdrop");
            Console.WriteLine("8. 退出 | Exit");
            Console.Write("请选择操作 (1-8): | Please select an operation (1-8): ");
        }
        
        /// <summary>
        /// 加载配置
        /// Load configuration
        /// </summary>
        private static void LoadConfig()
        {
            if (File.Exists(ConfigFilePath))
            {
                var configJson = File.ReadAllText(ConfigFilePath);
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(configJson);
                
                if (config.TryGetValue("ProgramId", out var programId))
                {
                    _programId = programId;
                }
                
                if (config.TryGetValue("PrivateKey", out var privateKey))
                {
                    _privateKey = privateKey;
                }
                
                if (config.TryGetValue("UseDevnet", out var useDevnet))
                {
                    _useDevnet = bool.Parse(useDevnet);
                }
            }
            else
            {
                // 创建默认配置
                // Create default configuration
                Console.WriteLine("配置文件不存在，正在创建... | Configuration file does not exist, creating...");
                
                Console.Write("请输入程序ID: | Please enter program ID: ");
                _programId = Console.ReadLine();
                
                Console.Write("请输入私钥 (Base58编码): | Please enter private key (Base58 encoded): ");
                _privateKey = Console.ReadLine();
                
                Console.Write("是否使用开发网 (y/n): | Use devnet (y/n): ");
                _useDevnet = Console.ReadLine().ToLower() == "y";
                
                var config = new Dictionary<string, string>
                {
                    { "ProgramId", _programId },
                    { "PrivateKey", _privateKey },
                    { "UseDevnet", _useDevnet.ToString() }
                };
                
                var configJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigFilePath, configJson);
                
                Console.WriteLine("配置已保存 | Configuration saved");
            }
            
            Console.WriteLine($"程序ID: {_programId} | Program ID: {_programId}");
            Console.WriteLine($"使用开发网: {_useDevnet} | Using devnet: {_useDevnet}");
        }
        
        /// <summary>
        /// 创建NFT集合
        /// Create NFT collection
        /// </summary>
        /// <param name="nftService">NFT服务 | NFT service</param>
        private static async Task CreateCollection(NFTService nftService)
        {
            Console.WriteLine("\n创建NFT集合 | Create NFT Collection");
            Console.WriteLine("---------------------------");
            
            Console.Write("集合名称: | Collection name: ");
            var name = Console.ReadLine();
            
            Console.Write("集合符号: | Collection symbol: ");
            var symbol = Console.ReadLine();
            
            Console.Write("集合描述: | Collection description: ");
            var description = Console.ReadLine();
            
            Console.Write("图片URL: | Image URL: ");
            var imageUrl = Console.ReadLine();
            
            Console.Write("外部URL: | External URL: ");
            var externalUrl = Console.ReadLine();
            
            Console.Write("版税百分比 (0-100): | Royalty percentage (0-100): ");
            var royaltyPercentage = int.Parse(Console.ReadLine());
            
            var metadata = new NFTMetadata
            {
                Name = name,
                Symbol = symbol,
                Description = description,
                Image = imageUrl,
                ExternalUrl = externalUrl,
                SellerFeeBasisPoints = royaltyPercentage * 100 // Convert percentage to basis points
            };
            
            Console.WriteLine("正在创建集合... | Creating collection...");
            var signature = await nftService.InitializeCollection(metadata);
            
            Console.WriteLine($"集合已创建 | Collection created");
            Console.WriteLine($"交易签名: {signature} | Transaction signature: {signature}");
        }
        
        /// <summary>
        /// 铸造NFT
        /// Mint NFT
        /// </summary>
        /// <param name="nftService">NFT服务 | NFT service</param>
        private static async Task MintNFT(NFTService nftService)
        {
            Console.WriteLine("\n铸造NFT | Mint NFT");
            Console.WriteLine("---------------------------");
            
            Console.Write("集合地址: | Collection address: ");
            var collectionAddress = Console.ReadLine();
            
            Console.Write("NFT名称: | NFT name: ");
            var name = Console.ReadLine();
            
            Console.Write("NFT描述: | NFT description: ");
            var description = Console.ReadLine();
            
            Console.Write("图片URL: | Image URL: ");
            var imageUrl = Console.ReadLine();
            
            // 创建元数据
            // Create metadata
            var metadata = new NFTMetadata
            {
                Name = name,
                Description = description,
                Image = imageUrl
            };
            
            // 序列化元数据
            // Serialize metadata
            var metadataJson = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
            
            // 在实际应用中，应该将元数据上传到IPFS或其他存储
            // In a real application, metadata should be uploaded to IPFS or other storage
            Console.WriteLine("元数据: | Metadata:");
            Console.WriteLine(metadataJson);
            
            Console.Write("元数据URI: | Metadata URI: ");
            var metadataUri = Console.ReadLine();
            
            Console.WriteLine("正在铸造NFT... | Minting NFT...");
            var signature = await nftService.MintNFT(collectionAddress, metadataUri);
            
            Console.WriteLine($"NFT已铸造 | NFT minted");
            Console.WriteLine($"交易签名: {signature} | Transaction signature: {signature}");
        }
        
        /// <summary>
        /// 转移NFT
        /// Transfer NFT
        /// </summary>
        /// <param name="nftService">NFT服务 | NFT service</param>
        private static async Task TransferNFT(NFTService nftService)
        {
            Console.WriteLine("\n转移NFT | Transfer NFT");
            Console.WriteLine("---------------------------");
            
            Console.Write("NFT地址: | NFT address: ");
            var nftAddress = Console.ReadLine();
            
            Console.Write("新所有者地址: | New owner address: ");
            var newOwner = Console.ReadLine();
            
            Console.WriteLine("正在转移NFT... | Transferring NFT...");
            var signature = await nftService.TransferNFT(nftAddress, newOwner);
            
            Console.WriteLine($"NFT已转移 | NFT transferred");
            Console.WriteLine($"交易签名: {signature} | Transaction signature: {signature}");
        }
        
        /// <summary>
        /// 销毁NFT
        /// Burn NFT
        /// </summary>
        /// <param name="nftService">NFT服务 | NFT service</param>
        private static async Task BurnNFT(NFTService nftService)
        {
            Console.WriteLine("\n销毁NFT | Burn NFT");
            Console.WriteLine("---------------------------");
            
            Console.Write("NFT地址: | NFT address: ");
            var nftAddress = Console.ReadLine();
            
            Console.WriteLine("正在销毁NFT... | Burning NFT...");
            var signature = await nftService.BurnNFT(nftAddress);
            
            Console.WriteLine($"NFT已销毁 | NFT burned");
            Console.WriteLine($"交易签名: {signature} | Transaction signature: {signature}");
        }
        
        /// <summary>
        /// 更新NFT元数据
        /// Update NFT metadata
        /// </summary>
        /// <param name="nftService">NFT服务 | NFT service</param>
        private static async Task UpdateNFTMetadata(NFTService nftService)
        {
            Console.WriteLine("\n更新NFT元数据 | Update NFT Metadata");
            Console.WriteLine("---------------------------");
            
            Console.Write("NFT地址: | NFT address: ");
            var nftAddress = Console.ReadLine();
            
            Console.Write("集合地址: | Collection address: ");
            var collectionAddress = Console.ReadLine();
            
            Console.Write("新元数据URI: | New metadata URI: ");
            var newMetadataUri = Console.ReadLine();
            
            Console.WriteLine("正在更新NFT元数据... | Updating NFT metadata...");
            var signature = await nftService.UpdateNFTMetadata(nftAddress, collectionAddress, newMetadataUri);
            
            Console.WriteLine($"NFT元数据已更新 | NFT metadata updated");
            Console.WriteLine($"交易签名: {signature} | Transaction signature: {signature}");
        }
        
        /// <summary>
        /// 更新集合元数据
        /// Update collection metadata
        /// </summary>
        /// <param name="nftService">NFT服务 | NFT service</param>
        private static async Task UpdateCollectionMetadata(NFTService nftService)
        {
            Console.WriteLine("\n更新集合元数据 | Update Collection Metadata");
            Console.WriteLine("---------------------------");
            
            Console.Write("集合地址: | Collection address: ");
            var collectionAddress = Console.ReadLine();
            
            Console.Write("新URI: | New URI: ");
            var newUri = Console.ReadLine();
            
            Console.WriteLine("正在更新集合元数据... | Updating collection metadata...");
            var signature = await nftService.UpdateCollectionMetadata(collectionAddress, newUri);
            
            Console.WriteLine($"集合元数据已更新 | Collection metadata updated");
            Console.WriteLine($"交易签名: {signature} | Transaction signature: {signature}");
        }
        
        /// <summary>
        /// 请求SOL空投
        /// Request SOL airdrop
        /// </summary>
        /// <param name="solanaService">Solana服务 | Solana service</param>
        private static async Task RequestAirdrop(SolanaService solanaService)
        {
            Console.WriteLine("\n请求SOL空投 | Request SOL Airdrop");
            Console.WriteLine("---------------------------");
            
            Console.Write("空投金额 (SOL): | Airdrop amount (SOL): ");
            var amount = decimal.Parse(Console.ReadLine());
            
            Console.WriteLine($"正在请求 {amount} SOL... | Requesting {amount} SOL...");
            var signature = await solanaService.RequestAirdrop(amount);
            
            Console.WriteLine($"空投请求已发送 | Airdrop request sent");
            Console.WriteLine($"交易签名: {signature} | Transaction signature: {signature}");
            
            // 更新余额
            // Update balance
            var balance = await solanaService.GetBalance();
            Console.WriteLine($"当前SOL余额: {balance} | Current SOL balance: {balance}");
        }
    }
} 