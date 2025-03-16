# Solana NFT Gallery Frontend

This is the frontend application for the Solana NFT Gallery project. It allows users to view, create, transfer, and burn NFTs on the Solana blockchain.

## Features

- Connect to Solana wallet (Phantom)
- View NFTs owned by the connected wallet
- Create new NFTs
- Transfer NFTs to other wallets
- Burn NFTs

## Technology Stack

- React
- TypeScript
- Solana Web3.js
- Solana Wallet Adapter

## Getting Started

### Prerequisites

- Node.js (v14 or later)
- npm or yarn
- A Solana wallet (e.g., Phantom)

### Installation

1. Clone the repository
2. Navigate to the frontend directory:
   ```
   cd SolanaNFT/frontend
   ```
3. Install dependencies:
   ```
   npm install
   ```
   or
   ```
   yarn install
   ```

### Running the Development Server

```
npm start
```
or
```
yarn start
```

This will start the development server at [http://localhost:3000](http://localhost:3000).

### Building for Production

```
npm run build
```
or
```
yarn build
```

This will create an optimized production build in the `build` folder.

## Project Structure

- `public/` - Static assets and HTML template
- `src/` - Source code
  - `components/` - Reusable UI components
  - `pages/` - Page components
  - `services/` - Services for interacting with the blockchain
  - `utils/` - Utility functions
  - `contexts/` - React contexts for state management

## Connecting to the Backend

This frontend application is designed to work with the Solana NFT smart contract and C# backend. In a production environment, you would configure the application to connect to your deployed smart contract on the Solana blockchain.

## Development Notes

- The application currently uses mock data for demonstration purposes
- To connect to a real Solana network, update the endpoint in the `App.tsx` file
- For production use, implement proper error handling and loading states

## License

This project is licensed under the MIT License - see the LICENSE file for details. 