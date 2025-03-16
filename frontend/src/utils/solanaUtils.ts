import { Connection, PublicKey, Transaction, SystemProgram, LAMPORTS_PER_SOL } from '@solana/web3.js';

/**
 * Get the balance of a Solana wallet
 * @param connection - Solana connection
 * @param publicKey - Wallet public key
 * @returns Promise with the balance in SOL
 */
export const getWalletBalance = async (connection: Connection, publicKey: PublicKey): Promise<number> => {
  try {
    const balance = await connection.getBalance(publicKey);
    return balance / LAMPORTS_PER_SOL;
  } catch (error) {
    console.error('Error getting wallet balance:', error);
    throw error;
  }
};

/**
 * Airdrop SOL to a wallet (only works on devnet and testnet)
 * @param connection - Solana connection
 * @param publicKey - Wallet public key
 * @param amount - Amount of SOL to airdrop
 * @returns Promise with the transaction signature
 */
export const airdropSol = async (
  connection: Connection,
  publicKey: PublicKey,
  amount: number = 1
): Promise<string> => {
  try {
    const signature = await connection.requestAirdrop(
      publicKey,
      amount * LAMPORTS_PER_SOL
    );
    
    await connection.confirmTransaction(signature);
    return signature;
  } catch (error) {
    console.error('Error airdropping SOL:', error);
    throw error;
  }
};

/**
 * Transfer SOL from one wallet to another
 * @param connection - Solana connection
 * @param fromWallet - Sender wallet
 * @param toPublicKey - Recipient public key
 * @param amount - Amount of SOL to transfer
 * @returns Promise with the transaction signature
 */
export const transferSol = async (
  connection: Connection,
  fromWallet: any, // Should be a Keypair or Wallet with signing capability
  toPublicKey: PublicKey,
  amount: number
): Promise<string> => {
  try {
    const transaction = new Transaction().add(
      SystemProgram.transfer({
        fromPubkey: fromWallet.publicKey,
        toPubkey: toPublicKey,
        lamports: amount * LAMPORTS_PER_SOL,
      })
    );
    
    const signature = await connection.sendTransaction(transaction, [fromWallet]);
    await connection.confirmTransaction(signature);
    return signature;
  } catch (error) {
    console.error('Error transferring SOL:', error);
    throw error;
  }
};

/**
 * Format a public key for display
 * @param publicKey - Public key to format
 * @param length - Length of the formatted string
 * @returns Formatted public key string
 */
export const formatPublicKey = (publicKey: PublicKey | null | undefined, length: number = 4): string => {
  if (!publicKey) return '';
  
  const publicKeyStr = publicKey.toBase58();
  return `${publicKeyStr.slice(0, length)}...${publicKeyStr.slice(-length)}`;
};

/**
 * Get the Solana explorer URL for a transaction or address
 * @param signature - Transaction signature or address
 * @param cluster - Solana cluster (mainnet-beta, devnet, testnet)
 * @returns Explorer URL
 */
export const getExplorerUrl = (signature: string, cluster: string = 'devnet'): string => {
  return `https://explorer.solana.com/${signature.includes('.') ? 'address' : 'tx'}/${signature}?cluster=${cluster}`;
}; 