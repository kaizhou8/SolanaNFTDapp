# Solana NFT Dapp

A full-stack decentralized application for creating, viewing, transferring, and burning NFTs on the Solana blockchain.

## Project Structure

This project consists of three main components:

1. **Smart Contract**: Rust-based Solana program for NFT operations
2. **C# Client**: .NET client for interacting with the Solana blockchain
3. **Frontend**: React-based web application for user interaction

## Features

- Create NFT collections
- Mint new NFTs
- View owned NFTs
- Transfer NFTs to other wallets
- Burn NFTs
- Update NFT metadata

## Getting Started

### Prerequisites

- Rust and Solana CLI for smart contract development
- .NET 6.0+ for C# client
- Node.js and npm for frontend development
- Solana wallet (e.g., Phantom)

### Installation

1. Clone the repository:
   ```
   git clone https://github.com/kaizhou8/SolanaNFTDapp.git
   cd SolanaNFTDapp
   ```

2. Set up the smart contract:
   ```
   cd smart-contract
   cargo build-bpf
   ```

3. Set up the C# client:
   ```
   cd ../csharp-client
   dotnet build
   ```

4. Set up the frontend:
   ```
   cd ../frontend
   npm install
   ```

### Running the Application

1. Start the frontend development server:
   ```
   cd frontend
   npm start
   ```

2. Open your browser and navigate to `http://localhost:3000`

3. Connect your Solana wallet to interact with the application

## Development Status

- Smart Contract: Basic implementation completed
- C# Client: Basic implementation completed
- Frontend: Basic implementation completed
- Testing and Integration: In progress

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Solana Foundation for the blockchain platform
- Metaplex for NFT standards
- React and TypeScript communities for frontend tools 