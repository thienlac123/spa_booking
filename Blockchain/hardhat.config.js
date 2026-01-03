require("@nomicfoundation/hardhat-toolbox");
require("dotenv").config(); // Nạp biến môi trường từ file .env

/** @type import('hardhat/config').HardhatUserConfig */
module.exports = {
  // Phiên bản Solidity phải khớp với file .sol của bạn (đang là ^0.8.20)
  solidity: "0.8.20",
  
  networks: {
    // Cấu hình mạng cục bộ (để test nhanh không tốn tiền)
    hardhat: {
    },
    // Cấu hình mạng Sepolia (để chạy demo đồ án)
    sepolia: {
      url: process.env.SEPOLIA_URL || "", // Đường dẫn kết nối mạng
      accounts: process.env.PRIVATE_KEY ? [process.env.PRIVATE_KEY] : [], // Ví của bạn
    }
  },
  // Cấu hình Etherscan để xác thực code (tùy chọn)
  etherscan: {
    apiKey: process.env.ETHERSCAN_API_KEY,
  },
};