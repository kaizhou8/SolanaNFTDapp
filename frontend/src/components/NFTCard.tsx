import React from 'react';

interface NFT {
  id: string;
  name: string;
  description: string;
  image: string;
  owner: string;
}

interface NFTCardProps {
  nft: NFT;
}

const NFTCard: React.FC<NFTCardProps> = ({ nft }) => {
  return (
    <div className="nft-card">
      <img src={nft.image} alt={nft.name} />
      <div className="nft-card-content">
        <h3 className="nft-card-title">{nft.name}</h3>
        <p className="nft-card-description">{nft.description}</p>
        <div style={{ display: 'flex', justifyContent: 'space-between' }}>
          <button className="btn">Transfer</button>
          <button className="btn btn-secondary">Burn</button>
        </div>
      </div>
    </div>
  );
};

export default NFTCard; 