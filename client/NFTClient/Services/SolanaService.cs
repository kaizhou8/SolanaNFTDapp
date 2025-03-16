using Solnet.Rpc;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NFTClient.Services
{
    /// <summary>
    /// Solana区块链交互服务
    /// Solana blockchain interaction service
    /// </summary>
    public class SolanaService
    {
        private readonly IRpcClient _rpcClient;
        private readonly Wallet _wallet;
        private readonly IStreamingRpcClient _streamingRpcClient;

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="rpcUrl">RPC URL</param>
        /// <param name="privateKey">私钥 | Private key</param>
        public SolanaService(string rpcUrl, string privateKey)
        {
            _rpcClient = ClientFactory.GetClient(rpcUrl);
            _streamingRpcClient = ClientFactory.GetStreamingClient(rpcUrl);
            _wallet = new Wallet(privateKey);
            
            Console.WriteLine($"钱包地址: {_wallet.Account.PublicKey} | Wallet address: {_wallet.Account.PublicKey}");
        }

        /// <summary>
        /// 获取钱包地址
        /// Get wallet address
        /// </summary>
        /// <returns>钱包地址 | Wallet address</returns>
        public string GetWalletAddress()
        {
            return _wallet.Account.PublicKey;
        }

        /// <summary>
        /// 获取SOL余额
        /// Get SOL balance
        /// </summary>
        /// <param name="publicKey">公钥 | Public key</param>
        /// <returns>SOL余额 | SOL balance</returns>
        public async Task<decimal> GetBalance(string publicKey = null)
        {
            var address = publicKey ?? _wallet.Account.PublicKey;
            var balance = await _rpcClient.GetBalanceAsync(address);
            
            if (balance.WasSuccessful)
            {
                return (decimal)balance.Result.Value / 1_000_000_000m; // Convert lamports to SOL
            }
            
            throw new Exception($"获取余额失败: {balance.Reason} | Failed to get balance: {balance.Reason}");
        }

        /// <summary>
        /// 发送交易
        /// Send transaction
        /// </summary>
        /// <param name="transaction">交易 | Transaction</param>
        /// <returns>交易签名 | Transaction signature</returns>
        public async Task<string> SendTransaction(byte[] transaction)
        {
            var response = await _rpcClient.SendTransactionAsync(transaction);
            
            if (response.WasSuccessful)
            {
                return response.Result;
            }
            
            throw new Exception($"发送交易失败: {response.Reason} | Failed to send transaction: {response.Reason}");
        }

        /// <summary>
        /// 创建交易
        /// Create transaction
        /// </summary>
        /// <param name="instructions">指令列表 | Instruction list</param>
        /// <returns>交易 | Transaction</returns>
        public async Task<byte[]> CreateTransaction(List<TransactionInstruction> instructions)
        {
            var recentBlockhash = await _rpcClient.GetRecentBlockHashAsync();
            
            if (!recentBlockhash.WasSuccessful)
            {
                throw new Exception($"获取最近区块哈希失败: {recentBlockhash.Reason} | Failed to get recent blockhash: {recentBlockhash.Reason}");
            }
            
            var transaction = new TransactionBuilder()
                .SetRecentBlockHash(recentBlockhash.Result.Value.Blockhash)
                .SetFeePayer(_wallet.Account.PublicKey)
                .AddInstructions(instructions)
                .Build(_wallet.Account);
                
            return transaction;
        }

        /// <summary>
        /// 获取账户信息
        /// Get account info
        /// </summary>
        /// <param name="publicKey">公钥 | Public key</param>
        /// <returns>账户信息 | Account info</returns>
        public async Task<AccountInfo> GetAccountInfo(string publicKey)
        {
            var response = await _rpcClient.GetAccountInfoAsync(publicKey);
            
            if (response.WasSuccessful)
            {
                return response.Result.Value;
            }
            
            throw new Exception($"获取账户信息失败: {response.Reason} | Failed to get account info: {response.Reason}");
        }

        /// <summary>
        /// 确认交易
        /// Confirm transaction
        /// </summary>
        /// <param name="signature">交易签名 | Transaction signature</param>
        /// <returns>是否确认 | Whether confirmed</returns>
        public async Task<bool> ConfirmTransaction(string signature)
        {
            var response = await _rpcClient.ConfirmTransactionAsync(signature);
            
            if (response.WasSuccessful)
            {
                return response.Result.Value;
            }
            
            throw new Exception($"确认交易失败: {response.Reason} | Failed to confirm transaction: {response.Reason}");
        }

        /// <summary>
        /// 获取交易
        /// Get transaction
        /// </summary>
        /// <param name="signature">交易签名 | Transaction signature</param>
        /// <returns>交易 | Transaction</returns>
        public async Task<TransactionMetaSlotInfo> GetTransaction(string signature)
        {
            var response = await _rpcClient.GetTransactionAsync(signature);
            
            if (response.WasSuccessful)
            {
                return response.Result;
            }
            
            throw new Exception($"获取交易失败: {response.Reason} | Failed to get transaction: {response.Reason}");
        }

        /// <summary>
        /// 请求空投
        /// Request airdrop
        /// </summary>
        /// <param name="amount">金额 | Amount</param>
        /// <returns>交易签名 | Transaction signature</returns>
        public async Task<string> RequestAirdrop(decimal amount)
        {
            var lamports = (ulong)(amount * 1_000_000_000m); // Convert SOL to lamports
            var response = await _rpcClient.RequestAirdropAsync(_wallet.Account.PublicKey, lamports);
            
            if (response.WasSuccessful)
            {
                return response.Result;
            }
            
            throw new Exception($"请求空投失败: {response.Reason} | Failed to request airdrop: {response.Reason}");
        }
    }
} 