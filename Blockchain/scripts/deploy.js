const fs = require("fs");
async function main() {
  const [deployer] = await ethers.getSigners();
  console.log("Deploying with:", deployer.address);

  const BookingEscrow = await ethers.getContractFactory("BookingEscrow");
  const escrow = await BookingEscrow.deploy();
  await escrow.waitForDeployment();

  const address = await escrow.getAddress();
  console.log("BookingEscrow deployed at:", address);

  // lưu lại để web đọc
  fs.mkdirSync("deployments", { recursive: true });
  fs.writeFileSync("deployments/localhost.json", JSON.stringify({
    BookingEscrow: address
  }, null, 2));
}
main().catch((e) => { console.error(e); process.exit(1); });
