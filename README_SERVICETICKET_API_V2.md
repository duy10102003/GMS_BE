# Hướng Dẫn Sử Dụng API ServiceTicket (Phiên Bản Mới)

## Tổng Quan

API ServiceTicket được thiết kế lại để đơn giản hóa và tối ưu hóa quy trình quản lý phiếu dịch vụ.

## Thay Đổi Chính

1. **Tạo/Update với List**: Truyền list parts và services trực tiếp trong request
2. **Garage Service không có Quantity**: Chỉ cần truyền list ID, không có quantity
3. **API Change Status riêng biệt**: Mỗi status có một API riêng
4. **Tự động tạo Customer/Vehicle**: Nếu không có ID thì tự động tạo mới
5. **Rollback tự động**: Khi update hoặc hủy, tự động rollback part quantity
6. **API Technical Staff**: Lấy danh sách technical staff với trạng thái rảnh/không rảnh

---

## API Endpoints

### 1. Service Ticket API

#### Tạo Service Ticket (với list parts và services)
```
POST /api/ServiceTicket
Content-Type: application/json

Request Body:
{
  "bookingId": null,                 // Optional
  "customerId": 1,                   // Optional: Nếu có thì dùng customer này
  "customerInfo": {                  // Optional: Nếu không có customerId thì tạo mới
    "customerName": "Nguyễn Văn B",
    "customerPhone": "0987654321",
    "customerEmail": "nguyenvanb@example.com"
  },
  "vehicleId": 1,                    // Optional: Nếu có thì dùng vehicle này (phải thuộc customer)
  "vehicleInfo": {                   // Optional: Nếu không có vehicleId thì tạo mới
    "vehicleName": "Toyota Camry",
    "vehicleLicensePlate": "30B-67890",
    "currentKm": 30000,
    "make": "Toyota",
    "model": "Camry"
  },
  "createdBy": 1,                    // ID của staff
  "initialIssue": "Xe không nổ máy",
  "serviceTicketCode": "ST001",      // Optional
  "assignedToTechnical": 2,          // Optional: ID của technical staff
  "assignDescription": "Kiểm tra hệ thống điện",
  "parts": [                         // Optional: List parts
    {
      "partId": 1,
      "quantity": 2
    },
    {
      "partId": 2,
      "quantity": 1
    }
  ],
  "garageServiceIds": [1, 2, 3]      // Optional: List garage service IDs (không có quantity)
}

Response:
{
  "errorCode": "SUCCESS",
  "userMsg": "Tạo service ticket thành công",
  "data": {
    "serviceTicketId": 1
  }
}

Lưu ý:
- Nếu không có customerId → Tạo customer mới từ customerInfo
- Nếu không có vehicleId → Tạo vehicle mới từ vehicleInfo và link với customer
- Vehicle phải thuộc về customer được chọn
- Status ban đầu: PendingTechnicalConfirmation (0)
- Nếu có assignedToTechnical → Tự động tạo TechnicalTask
```

#### Cập nhật Service Ticket (với list parts và services)
```
PUT /api/ServiceTicket/{id}
Content-Type: application/json

Request Body:
{
  "bookingId": null,                 // Optional
  "customerInfo": {                  // Optional: Cập nhật thông tin customer
    "customerName": "Nguyễn Văn C",
    "customerPhone": "0912345678",
    "customerEmail": "nguyenvanc@example.com"
  },
  "vehicleInfo": {                   // Optional: Cập nhật thông tin vehicle
    "vehicleName": "Honda Civic",
    "vehicleLicensePlate": "30C-11111",
    "currentKm": 40000
  },
  "modifiedBy": 1,                   // ID của staff
  "initialIssue": "Cập nhật vấn đề",
  "serviceTicketCode": "ST001",
  "parts": [                         // Optional: List parts mới (thay thế toàn bộ)
    {
      "partId": 1,
      "quantity": 3                  // Thay đổi số lượng
    },
    {
      "partId": 3,                   // Thêm part mới
      "quantity": 1
    }
  ],
  "garageServiceIds": [1, 3]         // Optional: List garage service IDs mới (thay thế toàn bộ)
}

Response:
{
  "errorCode": "SUCCESS",
  "userMsg": "Cập nhật service ticket thành công",
  "data": {
    "affectedRows": 1
  }
}

Lưu ý:
- Tự động so sánh list cũ và mới
- Rollback part quantity từ list cũ
- Validate và thêm parts/services mới
- Xóa (soft delete) các detail cũ
```

#### Lấy Chi Tiết Service Ticket
```
GET /api/ServiceTicket/{id}

Response:
{
  "errorCode": "SUCCESS",
  "userMsg": "Lấy chi tiết service ticket thành công",
  "data": {
    "serviceTicketId": 1,
    "serviceTicketCode": "ST001",
    "vehicle": {
      "vehicleId": 1,
      "vehicleName": "Honda Civic",
      "vehicleLicensePlate": "30A-12345",
      ...
    },
    "customer": {
      "customerId": 1,
      "customerName": "Nguyễn Văn A",
      "customerPhone": "0123456789",
      ...
    },
    "parts": [
      {
        "serviceTicketDetailId": 1,
        "part": {
          "partId": 1,
          "partName": "Nhớt động cơ",
          "partCode": "PART001",
          "partPrice": 200000,
          "partQuantity": 100,
          "partUnit": "Lít",
          "partCategory": { ... }
        },
        "quantity": 2
      }
    ],
    "garageServices": [
      {
        "serviceTicketDetailId": 2,
        "garageService": {
          "garageServiceId": 1,
          "garageServiceName": "Thay nhớt",
          "garageServicePrice": 50000
        }
      }
    ],
    "technicalTasks": [ ... ],
    "serviceTicketStatus": 0
  }
}
```

#### Chuyển Status - PendingTechnicalConfirmation (0)
```
PUT /api/ServiceTicket/{id}/status/pending
Content-Type: application/json

Request Body:
{
  "modifiedBy": 1,
  "note": "Ghi chú (optional)"
}
```

#### Chuyển Status - AdjustedByTechnical (1)
```
PUT /api/ServiceTicket/{id}/status/adjusted
Content-Type: application/json

Request Body:
{
  "modifiedBy": 1,
  "note": "Ghi chú (optional)"
}

Lưu ý: Chỉ cho phép từ PendingTechnicalConfirmation
```

#### Chuyển Status - InProgress (2)
```
PUT /api/ServiceTicket/{id}/status/in-progress
Content-Type: application/json

Request Body:
{
  "modifiedBy": 1,
  "note": "Ghi chú (optional)"
}

Lưu ý: Chỉ cho phép từ AdjustedByTechnical
```

#### Chuyển Status - Completed (3)
```
PUT /api/ServiceTicket/{id}/status/completed
Content-Type: application/json

Request Body:
{
  "modifiedBy": 1,
  "note": "Ghi chú (optional)"
}

Lưu ý: 
- Chỉ cho phép từ InProgress
- Tự động deduct part quantity
```

#### Chuyển Status - Cancelled (4)
```
PUT /api/ServiceTicket/{id}/status/cancelled
Content-Type: application/json

Request Body:
{
  "modifiedBy": 1,
  "note": "Ghi chú (optional)"
}

Lưu ý:
- Chỉ hủy được khi chưa hoàn thành (status != 3)
- Tự động rollback part quantity
```

#### Xóa Service Ticket
```
DELETE /api/ServiceTicket/{id}

Lưu ý:
- Chỉ xóa được khi chưa hoàn thành
- Tự động rollback part quantity
```

#### Assign cho Technical Staff
```
POST /api/ServiceTicket/{id}/assign
Content-Type: application/json

Request Body:
{
  "assignedToTechnical": 2,
  "description": "Mô tả công việc"
}

Response:
{
  "errorCode": "SUCCESS",
  "userMsg": "Assign service ticket cho technical staff thành công",
  "data": {
    "technicalTaskId": 1
  }
}
```

---

### 2. User API

#### Lấy Danh Sách Technical Staff với Trạng Thái
```
GET /api/User/technical-staff?roleName=TechnicalStaff

Response:
{
  "errorCode": "SUCCESS",
  "userMsg": "Lấy danh sách technical staff thành công",
  "data": [
    {
      "userId": 2,
      "fullName": "Nguyễn Văn C",
      "email": "nguyenvanc@example.com",
      "phone": "0912345678",
      "roleId": 3,
      "roleName": "TechnicalStaff",
      "isAvailable": true,              // true = rảnh, false = không rảnh
      "currentTaskCount": 0             // Số task đang làm hoặc đang được assign
    },
    {
      "userId": 3,
      "fullName": "Trần Văn D",
      "email": "tranvand@example.com",
      "phone": "0923456789",
      "roleId": 3,
      "roleName": "TechnicalStaff",
      "isAvailable": false,             // Đang có task
      "currentTaskCount": 2
    }
  ]
}

Lưu ý:
- isAvailable = true khi currentTaskCount = 0
- currentTaskCount = số service ticket có status IN (0, 1, 2) được assign cho technical đó
```

---

### 3. Customer API

#### Tìm kiếm Customer cho Select
```
POST /api/Customer/search
Content-Type: application/json

Request Body:
{
  "searchKeyword": "Nguyễn",           // Tìm theo tên, số điện thoại, email
  "limit": 50
}
```

---

### 4. Vehicle API

#### Tìm kiếm Vehicle cho Select (lọc theo Customer)
```
POST /api/Vehicle/search
Content-Type: application/json

Request Body:
{
  "customerId": 1,                     // Optional: Lọc theo customer
  "searchKeyword": "Honda",           // Tìm theo tên xe, biển số
  "limit": 50
}
```

---

### 5. Part API

#### Tìm kiếm Part cho Select
```
GET /api/Part/search?searchKeyword=nhớt&limit=50
```

---

### 6. Garage Service API

#### Tìm kiếm Garage Service cho Select
```
POST /api/GarageService/search
Content-Type: application/json

Request Body:
{
  "searchKeyword": "thay nhớt",
  "limit": 50
}
```

---

## Luồng Nghiệp Vụ Mới

### 1. Staff Tạo Phiếu Dịch Vụ

```
1. Tìm kiếm/Chọn Customer
   → POST /api/Customer/search
   
2. Tìm kiếm/Chọn Vehicle của Customer (hoặc nhập mới)
   → POST /api/Vehicle/search?customerId=1
   
3. Tìm kiếm/Chọn Parts (nhiều)
   → GET /api/Part/search?searchKeyword=xxx
   
4. Tìm kiếm/Chọn Garage Services (nhiều)
   → POST /api/GarageService/search
   
5. Lấy danh sách Technical Staff rảnh
   → GET /api/User/technical-staff
   
6. Tạo Service Ticket với tất cả thông tin
   → POST /api/ServiceTicket
   {
     "customerId": 1,              // Hoặc customerInfo để tạo mới
     "vehicleId": 1,                // Hoặc vehicleInfo để tạo mới
     "parts": [{ "partId": 1, "quantity": 2 }],
     "garageServiceIds": [1, 2],
     "assignedToTechnical": 2,
     "assignDescription": "..."
   }
   → Status: PendingTechnicalConfirmation (0)
```

### 2. Staff Cập Nhật Phiếu Dịch Vụ

```
→ PUT /api/ServiceTicket/{id}
{
  "parts": [{ "partId": 1, "quantity": 3 }],  // List mới (thay thế toàn bộ)
  "garageServiceIds": [1, 3]                    // List mới (thay thế toàn bộ)
}
→ Tự động rollback part quantity từ list cũ
→ Validate và thêm parts/services mới
```

### 3. Staff Chuyển Status

```
→ PUT /api/ServiceTicket/{id}/status/pending
→ PUT /api/ServiceTicket/{id}/status/adjusted
→ PUT /api/ServiceTicket/{id}/status/in-progress
→ PUT /api/ServiceTicket/{id}/status/completed
→ PUT /api/ServiceTicket/{id}/status/cancelled
```

### 4. Technical Staff Xem Danh Sách Công Việc

```
→ POST /api/TechnicalTask/paging
{
  "assignedToTechnical": 2,
  "page": 1,
  "pageSize": 20
}
```

### 5. Technical Staff Điều Chỉnh Parts/Services

```
→ PUT /api/TechnicalTask/{id}/adjust
{
  "parts": [{ "partId": 1, "quantity": 3 }],
  "garageServiceIds": [1, 2]
}
→ Status chuyển thành: AdjustedByTechnical (1)
```

### 6. Staff Xác Nhận Điều Chỉnh

```
→ PUT /api/TechnicalTask/{id}/confirm?confirmedBy=1
→ Status chuyển thành: InProgress (2)
```

### 7. Technical Staff Hoàn Thành

```
→ PUT /api/TechnicalTask/{id}/complete
→ Deduct part quantity
→ Status chuyển thành: Completed (3)
```

### 8. Staff Xuất Hóa Đơn

```
→ POST /api/Invoice
{
  "serviceTicketId": 1,
  "customerId": 1,
  "taxAmount": 10000,
  "discountAmount": 5000
}
```

---

## Ví Dụ Hoàn Chỉnh

### Scenario: Staff tạo phiếu dịch vụ với customer và vehicle mới

1. **Tìm kiếm Parts**
   ```
   GET /api/Part/search?searchKeyword=nhớt
   ```

2. **Tìm kiếm Services**
   ```
   POST /api/GarageService/search
   { "searchKeyword": "thay nhớt" }
   ```

3. **Lấy danh sách Technical Staff rảnh**
   ```
   GET /api/User/technical-staff?roleName=TechnicalStaff
   ```

4. **Tạo Service Ticket**
   ```
   POST /api/ServiceTicket
   {
     "customerInfo": {                    // Tạo customer mới
       "customerName": "Nguyễn Văn E",
       "customerPhone": "0934567890",
       "customerEmail": "nguyenvane@example.com"
     },
     "vehicleInfo": {                     // Tạo vehicle mới (tự động link với customer)
       "vehicleName": "Ford Ranger",
       "vehicleLicensePlate": "30D-22222",
       "currentKm": 25000,
       "make": "Ford",
       "model": "Ranger"
     },
     "createdBy": 1,
     "initialIssue": "Xe bị rò rỉ dầu",
     "assignedToTechnical": 2,
     "assignDescription": "Kiểm tra và sửa chữa rò rỉ dầu",
     "parts": [
       { "partId": 1, "quantity": 2 },
       { "partId": 2, "quantity": 1 }
     ],
     "garageServiceIds": [1, 2]
   }
   ```
   → Tự động tạo customer mới
   → Tự động tạo vehicle mới và link với customer
   → Tự động tạo TechnicalTask
   → Status: PendingTechnicalConfirmation (0)

5. **Cập nhật Service Ticket (thay đổi parts)**
   ```
   PUT /api/ServiceTicket/1
   {
     "modifiedBy": 1,
     "parts": [
       { "partId": 1, "quantity": 3 },   // Thay đổi số lượng
       { "partId": 3, "quantity": 1 }     // Thêm part mới
     ],
     "garageServiceIds": [1, 3]           // Thay đổi services
   }
   ```
   → Rollback part quantity từ parts cũ (partId: 1 quantity: 2, partId: 2 quantity: 1)
   → Validate và thêm parts/services mới

6. **Chuyển Status**
   ```
   PUT /api/ServiceTicket/1/status/completed
   {
     "modifiedBy": 1
   }
   ```
   → Deduct part quantity (partId: 1 quantity: 3, partId: 3 quantity: 1)

---

## So Sánh Với Phiên Bản Cũ

### Đã Xóa (Không Còn Cần Thiết)
- ❌ `POST /api/ServiceTicket/{id}/parts` - Thêm part riêng lẻ
- ❌ `POST /api/ServiceTicket/{id}/garage-services` - Thêm service riêng lẻ
- ❌ `DELETE /api/ServiceTicket/{id}/details/{detailId}` - Xóa detail riêng lẻ
- ❌ `POST /api/ServiceTicket/technical-tasks/{id}/approve` - Duyệt đề xuất

### Đã Thêm Mới
- ✅ `PUT /api/ServiceTicket/{id}/status/pending` - Chuyển status riêng biệt
- ✅ `PUT /api/ServiceTicket/{id}/status/adjusted` - Chuyển status riêng biệt
- ✅ `PUT /api/ServiceTicket/{id}/status/in-progress` - Chuyển status riêng biệt
- ✅ `PUT /api/ServiceTicket/{id}/status/completed` - Chuyển status riêng biệt
- ✅ `PUT /api/ServiceTicket/{id}/status/cancelled` - Chuyển status riêng biệt
- ✅ `GET /api/User/technical-staff` - Lấy danh sách technical staff với trạng thái

### Đã Cải Thiện
- ✅ Tạo/Update với list parts và services trong một request
- ✅ Garage service không có quantity
- ✅ Tự động tạo customer/vehicle nếu không có ID
- ✅ Tự động rollback khi update hoặc hủy
- ✅ Validate vehicle phải thuộc về customer

---

## Notes

- **Garage Service**: Không có quantity, chỉ cần truyền list ID
- **Parts**: Có quantity, truyền list với partId và quantity
- **Rollback**: Tự động xử lý, không cần frontend can thiệp
- **Status Flow**: Tuân thủ đúng luồng, mỗi status có API riêng
- **Technical Staff Availability**: Dựa vào số task đang làm (status IN (0,1,2))

