import React, { useState, useEffect } from 'react';
import { useWallet } from '@solana/wallet-adapter-react';
import { WalletMultiButton } from '@solana/wallet-adapter-react-ui';
import NFTCard from '../components/NFTCard';
import CreateNFTForm from '../components/CreateNFTForm';

interface NFT {
  id: string;
  name: string;
  description: string;
  image: string;
  owner: string;
}

const Home: React.FC = () => {
  const { publicKey, connected } = useWallet();
  const [nfts, setNfts] = useState<NFT[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [showCreateForm, setShowCreateForm] = useState<boolean>(false);

  // Mock data for demonstration
  const mockNfts: NFT[] = [
    {
      id: '1',
      name: 'Cosmic Explorer',
      description: 'A journey through the cosmos',
      image: 'https://via.placeholder.com/300',
      owner: publicKey?.toBase58() || '',
    },
    {
      id: '2',
      name: 'Digital Dreams',
      description: 'Abstract digital art piece',
      image: 'https://via.placeholder.com/300',
      owner: publicKey?.toBase58() || '',
    },
    {
      id: '3',
      name: 'Crypto Punk',
      description: 'Unique pixel art character',
      image: 'https://via.placeholder.com/300',
      owner: publicKey?.toBase58() || '',
    },
  ];

  useEffect(() => {
    if (connected) {
      // In a real application, we would fetch NFTs from the blockchain here
      setLoading(true);
      // Simulate API call
      setTimeout(() => {
        setNfts(mockNfts);
        setLoading(false);
      }, 1000);
    } else {
      setNfts([]);
    }
  }, [connected, publicKey]);

  const handleCreateNFT = (nft: Omit<NFT, 'id' | 'owner'>) => {
    // In a real application, we would mint the NFT on the blockchain here
    const newNft: NFT = {
      ...nft,
      id: Math.random().toString(36).substring(2, 9),
      owner: publicKey?.toBase58() || '',
    };
    
    setNfts([...nfts, newNft]);
    setShowCreateForm(false);
  };

  return (
    <div className="container">
      <header className="header">
        <h1>Solana NFT Gallery</h1>
        <WalletMultiButton />
      </header>

      {connected ? (
        <>
          <div className="card">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
              <h2>Your NFT Collection</h2>
              <button 
                className="btn" 
                onClick={() => setShowCreateForm(!showCreateForm)}
              >
                {showCreateForm ? 'Cancel' : 'Create New NFT'}
              </button>
            </div>

            {showCreateForm && (
              <CreateNFTForm onSubmit={handleCreateNFT} onCancel={() => setShowCreateForm(false)} />
            )}

            {loading ? (
              <p>Loading your NFTs...</p>
            ) : nfts.length > 0 ? (
              <div className="nft-grid">
                {nfts.map((nft) => (
                  <NFTCard key={nft.id} nft={nft} />
                ))}
              </div>
            ) : (
              <p>You don't have any NFTs yet. Create one to get started!</p>
            )}
          </div>
        </>
      ) : (
        <div className="card">
          <h2>Connect your wallet to view your NFTs</h2>
          <p>Please connect your Solana wallet to view and manage your NFT collection.</p>
        </div>
      )}
    </div>
  );
};

export default Home; 