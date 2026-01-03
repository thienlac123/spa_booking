// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

// Import thư viện chống hack Reentrancy (nếu dùng Remix thì nó tự tải)
import "@openzeppelin/contracts/security/ReentrancyGuard.sol";

contract BookingEscrow is ReentrancyGuard {
     bool public testing = true; // = true: không check thời gian
    enum Status { None, Deposited, Released, Refunded, Claimed, CanceledBySpa, Resolved }

    struct Booking {
        address payable client;
        address payable spa;
        uint256 amount;        // wei
        uint64  cancelBefore;  // timestamp
        uint64  graceUntil;    // timestamp
        Status  status;
    }

    address public owner;
    // Dùng mapping bytes32 để Web C# tự tạo ID (ví dụ hash từ OrderID)
    mapping(bytes32 => Booking) public bookings;

    event Created(bytes32 indexed id, address indexed client, address indexed spa, uint256 amount);
    event Released(bytes32 indexed id);
    event Refunded(bytes32 indexed id);
    event Claimed(bytes32 indexed id);
    event CanceledBySpa(bytes32 indexed id);
    event Resolved(bytes32 indexed id, bool paidToSpa);

    modifier onlyOwner() { require(msg.sender == owner, "only owner"); _; }
    modifier onlySpa(bytes32 id) { require(msg.sender == bookings[id].spa, "only spa"); _; }

    constructor() { owner = msg.sender; }

    // C# sẽ truyền ID vào (Hash của OrderID trong SQL)
    function createBooking(
        bytes32 id,
        address payable spa,
        uint64 cancelBefore,
        uint64 graceUntil
    ) external payable nonReentrant {
        require(bookings[id].status == Status.None, "ID exists");
        require(msg.value > 0, "no deposit");
        require(graceUntil > cancelBefore, "grace > cancel");

        bookings[id] = Booking({
            client: payable(msg.sender),
            spa:    spa,
            amount: msg.value,
            cancelBefore: cancelBefore,
            graceUntil:   graceUntil,
            status: Status.Deposited
        });
        emit Created(id, msg.sender, spa, msg.value);
    }

    // Spa xác nhận -> dùng .call để chuyển tiền an toàn
    function confirmService(bytes32 id) external onlySpa(id) nonReentrant {
        Booking storage b = bookings[id];
        require(b.status == Status.Deposited, "bad status");
        
        b.status = Status.Released;
        
        (bool success, ) = b.spa.call{value: b.amount}("");
        require(success, "transfer failed");
        
        emit Released(id);
    }

    // Khách hủy đúng hạn
    function cancelByClient(bytes32 id) external nonReentrant {
        Booking storage b = bookings[id];
        require(msg.sender == b.client, "only client");
        require(b.status == Status.Deposited, "bad status");
        require(block.timestamp <= b.cancelBefore, "too late");
        
        b.status = Status.Refunded;

        (bool success, ) = b.client.call{value: b.amount}("");
        require(success, "transfer failed");

        emit Refunded(id);
    }

   // Spa claim no-show
function claimNoShow(bytes32 id) external onlySpa(id) nonReentrant {
    Booking storage b = bookings[id];
    require(b.status == Status.Deposited, "bad status");

    //  BỎ CHECK THỜI GIAN KHI TEST
    // require(block.timestamp > b.graceUntil, "too early");

    b.status = Status.Claimed;

    (bool success, ) = b.spa.call{value: b.amount}("");
    require(success, "transfer failed");

    emit Claimed(id);
}


    // Spa chủ động hủy
    function cancelBySpa(bytes32 id) external onlySpa(id) nonReentrant {
        Booking storage b = bookings[id];
        require(b.status == Status.Deposited, "bad status");
        
        b.status = Status.CanceledBySpa;

        (bool success, ) = b.client.call{value: b.amount}("");
        require(success, "transfer failed");

        emit CanceledBySpa(id);
    }

    // Admin xử lý tranh chấp (chỉ khi tiền còn trong contract)
    function resolveDispute(bytes32 id, bool payToSpa) external onlyOwner nonReentrant {
        Booking storage b = bookings[id];
        require(b.status == Status.Deposited, "unresolvable"); // Chỉ xử lý khi tiền chưa chuyển đi
        
        b.status = Status.Resolved;
        
        address payable recipient = payToSpa ? b.spa : b.client;
        (bool success, ) = recipient.call{value: b.amount}("");
        require(success, "transfer failed");

        emit Resolved(id, payToSpa);
    }
}