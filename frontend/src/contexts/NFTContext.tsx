import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { useConnection, useWallet } from '@solana/wallet-adapter-react';
import { NFTService, NFT, NFTMetadata } from '../services/nftService';

interface NFTContextProps {
  nfts: NFT[];
  loading: boolean;
  error: string | null;
  createNFT: (metadata: NFTMetadata) => Promise<NFT | null>;
  transferNFT: (nftId: string, toPublicKey: string) => Promise<boolean>;
  burnNFT: (nftId: string) => Promise<boolean>;
  refreshNFTs: () => Promise<void>;
}

const NFTContext = createContext<NFTContextProps | undefined>(undefined);

export const useNFT = () => {
  const context = useContext(NFTContext);
  if (!context) {
    throw new Error('useNFT must be used within an NFTProvider');
  }
  return context;
};

interface NFTProviderProps {
  children: ReactNode;
}

export const NFTProvider: React.FC<NFTProviderProps> = ({ children }) => {
  const { connection } = useConnection();
  const { publicKey, connected } = useWallet();
  
  const [nfts, setNfts] = useState<NFT[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [nftService, setNftService] = useState<NFTService | null>(null);
  
  // Initialize NFT service when connection is available
  useEffect(() => {
    if (connection) {
      const service = new NFTService(connection.rpcEndpoint);
      setNftService(service);
    }
  }, [connection]);
  
  // Fetch NFTs when wallet is connected
  useEffect(() => {
    if (connected && publicKey && nftService) {
      refreshNFTs();
    } else {
      setNfts([]);
    }
  }, [connected, publicKey, nftService]);
  
  const refreshNFTs = async () => {
    if (!publicKey || !nftService) return;
    
    setLoading(true);
    setError(null);
    
    try {
      const fetchedNfts = await nftService.getNFTsByOwner(publicKey);
      setNfts(fetchedNfts);
    } catch (err) {
      console.error('Error fetching NFTs:', err);
      setError('Failed to fetch NFTs. Please try again.');
    } finally {
      setLoading(false);
    }
  };
  
  const createNFT = async (metadata: NFTMetadata): Promise<NFT | null> => {
    if (!publicKey || !nftService) {
      setError('Wallet not connected');
      return null;
    }
    
    setLoading(true);
    setError(null);
    
    try {
      const newNft = await nftService.createNFT(publicKey, metadata);
      setNfts([...nfts, newNft]);
      return newNft;
    } catch (err) {
      console.error('Error creating NFT:', err);
      setError('Failed to create NFT. Please try again.');
      return null;
    } finally {
      setLoading(false);
    }
  };
  
  const transferNFT = async (nftId: string, toPublicKey: string): Promise<boolean> => {
    if (!publicKey || !nftService) {
      setError('Wallet not connected');
      return false;
    }
    
    setLoading(true);
    setError(null);
    
    try {
      const success = await nftService.transferNFT(
        nftId,
        publicKey,
        new (window as any).solanaWeb3.PublicKey(toPublicKey)
      );
      
      if (success) {
        // Remove the transferred NFT from the list
        setNfts(nfts.filter(nft => nft.id !== nftId));
      }
      
      return success;
    } catch (err) {
      console.error('Error transferring NFT:', err);
      setError('Failed to transfer NFT. Please try again.');
      return false;
    } finally {
      setLoading(false);
    }
  };
  
  const burnNFT = async (nftId: string): Promise<boolean> => {
    if (!publicKey || !nftService) {
      setError('Wallet not connected');
      return false;
    }
    
    setLoading(true);
    setError(null);
    
    try {
      const success = await nftService.burnNFT(nftId, publicKey);
      
      if (success) {
        // Remove the burned NFT from the list
        setNfts(nfts.filter(nft => nft.id !== nftId));
      }
      
      return success;
    } catch (err) {
      console.error('Error burning NFT:', err);
      setError('Failed to burn NFT. Please try again.');
      return false;
    } finally {
      setLoading(false);
    }
  };
  
  const value = {
    nfts,
    loading,
    error,
    createNFT,
    transferNFT,
    burnNFT,
    refreshNFTs,
  };
  
  return <NFTContext.Provider value={value}>{children}</NFTContext.Provider>;
}; 