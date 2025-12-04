# Hướng Dẫn Sử Dụng API ServiceTicket

## Tổng Quan

API ServiceTicket được thiết kế để hỗ trợ quy trình quản lý phiếu dịch vụ với 2 vai trò chính:
- **Staff**: Tạo phiếu dịch vụ, quản lý customer/vehicle, assign cho technical staff, xác nhận điều chỉnh, xuất hóa đơn
- **Technical Staff (Mechanic)**: Xem danh sách công việc, điều chỉnh parts/services, hoàn thành công việc

## Luồng Nghiệp Vụ

### 1. Staff Tạo Phiếu Dịch Vụ

```
1. Tìm kiếm/Chọn Customer (hoặc nhập mới)
   → POST /api/Customer/search
   
2. Tìm kiếm/Chọn Vehicle của Customer (hoặc nhập mới)
   → POST /api/Vehicle/search (với customerId)
   
3. Tìm kiếm/Chọn Parts (nhiều)
   → GET /api/Part/search?searchKeyword=xxx
   
4. Tìm kiếm/Chọn Garage Services (nhiều hoặc 1)
   → POST /api/GarageService/search
   
5. Tạo Service Ticket với Parts và Services
   → POST /api/ServiceTicket/create-with-parts-services
   → Status: PendingTechnicalConfirmation (0)
```

### 2. Staff Assign Cho Technical Staff

```
→ POST /api/ServiceTicket/{id}/assign
→ Tạo TechnicalTask
→ Status vẫn là PendingTechnicalConfirmation (0)
```

### 3. Technical Staff Xem Danh Sách Công Việc

```
→ POST /api/TechnicalTask/paging
→ Filter: AssignedToTechnical = {technicalStaffId}
→ Chỉ hiển thị Service Ticket chưa hoàn thành (status != 3)
```

### 4. Technical Staff Điều Chỉnh Parts/Services

```
→ PUT /api/TechnicalTask/{id}/adjust
→ Body: { Parts: [...], GarageServices: [...] }
→ Status chuyển thành: AdjustedByTechnical (1)
→ Rollback part quantity từ các parts cũ
```

### 5. Staff Xác Nhận Điều Chỉnh

```
→ PUT /api/TechnicalTask/{id}/confirm?confirmedBy={staffId}
→ Status chuyển thành: InProgress (2)
```

### 6. Technical Staff Hoàn Thành Công Việc

```
→ PUT /api/TechnicalTask/{id}/complete
→ Deduct part quantity
→ Status chuyển thành: Completed (3)
```

### 7. Staff Xuất Hóa Đơn

```
→ POST /api/Invoice
→ Body: { ServiceTicketId, CustomerId, TaxAmount, DiscountAmount, ... }
→ Tự động tính PartsAmount, GarageServiceAmount, TotalAmount
```

### 8. Hủy Service Ticket

```
→ DELETE /api/ServiceTicket/{id}
→ Chỉ hủy được khi status != Completed (3)
→ Rollback part quantity
```

---

## API Endpoints

### 1. Customer API

#### Tìm kiếm Customer cho Select
```
POST /api/Customer/search
Content-Type: application/json

Request Body:
{
  "searchKeyword": "Nguyễn Văn A",  // Tìm theo tên, số điện thoại, email
  "limit": 50                        // Mặc định 50
}

Response:
{
  "errorCode": "SUCCESS",
  "userMsg": "Tìm kiếm customer thành công",
  "data": [
    {
      "customerId": 1,
      "customerName": "Nguyễn Văn A",
      "customerPhone": "0123456789",
      "customerEmail": "nguyenvana@example.com"
    }
  ]
}
```

---

### 2. Vehicle API

#### Tìm kiếm Vehicle cho Select
```
POST /api/Vehicle/search
Content-Type: application/json

Request Body:
{
  "customerId": 1,                   // Optional: Lọc theo customer
  "searchKeyword": "30A-12345",      // Tìm theo tên xe, biển số
  "limit": 50
}

Response:
{
  "errorCode": "SUCCESS",
  "userMsg": "Tìm kiếm vehicle thành công",
  "data": [
    {
      "vehicleId": 1,
      "vehicleName": "Honda Civic",
      "vehicleLicensePlate": "30A-12345",
      "currentKm": 50000,
      "make": "Honda",
      "model": "Civic",
      "customerId": 1,
      "customerName": "Nguyễn Văn A"
    }
  ]
}
```

---

### 3. Part API

#### Tìm kiếm Part cho Select
```
GET /api/Part/search?searchKeyword=nhớt&limit=50

Response:
{
  "errorCode": "SUCCESS",
  "userMsg": "Tìm kiếm part thành công",
  "data": [
    {
      "partId": 1,
      "partName": "Nhớt động cơ 5W-30",
      "partCode": "PART001",
      "partQuantity": 100,
      "partUnit": "Lít"
    }
  ]
}
```

---

### 4. Garage Service API

#### Tìm kiếm Garage Service cho Select
```
POST /api/GarageService/search
Content-Type: application/json

Request Body:
{
  "searchKeyword": "thay nhớt",
  "limit": 50
}

Response:
{
  "errorCode": "SUCCESS",
  "userMsg": "Tìm kiếm garage service thành công",
  "data": [
    {
      "garageServiceId": 1,
      "garageServiceName": "Thay nhớt động cơ",
      "garageServicePrice": 50000
    }
  ]
}
```

---

### 5. Service Ticket API

#### Tạo Service Ticket với Parts và Services
```
POST /api/ServiceTicket/create-with-parts-services
Content-Type: application/json

Request Body:
{
  "bookingId": null,                 // Optional
  "customerId": 1,                   // Nếu có customer sẵn
  "customerInfo": {                  // Nếu tạo customer mới
    "customerName": "Nguyễn Văn B",
    "customerPhone": "0987654321",
    "customerEmail": "nguyenvanb@example.com"
  },
  "vehicleId": 1,                    // Nếu có vehicle sẵn
  "vehicleInfo": {                   // Nếu tạo vehicle mới
    "vehicleName": "Toyota Camry",
    "vehicleLicensePlate": "30B-67890",
    "currentKm": 30000,
    "make": "Toyota",
    "model": "Camry",
    "customerId": 1                  // Phải thuộc về customer được chọn
  },
  "createdBy": 1,                    // ID của staff
  "initialIssue": "Xe không nổ máy",
  "serviceTicketCode": "ST001",      // Optional, có thể tự động generate
  "assignedToTechnical": 2,          // ID của technical staff (bắt buộc)
  "assignDescription": "Kiểm tra và sửa chữa hệ thống điện",
  "parts": [                         // Optional
    {
      "partId": 1,
      "quantity": 2
    }
  ],
  "garageServices": [                // Optional
    {
      "garageServiceId": 1,
      "quantity": 1
    }
  ]
}

Response:
{
  "errorCode": "SUCCESS",
  "userMsg": "Tạo service ticket thành công",
  "data": {
    "serviceTicketId": 1
  }
}
```

**Lưu ý:**
- Nếu có `customerId` thì không cần `customerInfo`
- Nếu có `vehicleId` thì không cần `vehicleInfo`
- Vehicle phải thuộc về Customer được chọn
- Status ban đầu: `PendingTechnicalConfirmation` (0)
- Tự động tạo TechnicalTask

#### Lấy Danh Sách Service Ticket (Phân Trang)
```
POST /api/ServiceTicket/paging
Content-Type: application/json

Request Body:
{
  "page": 1,
  "pageSize": 20,
  "columnFilters": [
    {
      "columnName": "ServiceTicketStatus",
      "operator": "equals",
      "value": 0
    }
  ],
  "columnSorts": [
    {
      "columnName": "CreatedDate",
      "sortDirection": "DESC"
    }
  ]
}
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
    "vehicle": { ... },
    "customer": { ... },
    "parts": [ ... ],
    "garageServices": [ ... ],
    "technicalTasks": [ ... ],
    "serviceTicketStatus": 0
  }
}
```

#### Hủy Service Ticket
```
DELETE /api/ServiceTicket/{id}

Lưu ý:
- Chỉ hủy được khi status != Completed (3)
- Tự động rollback part quantity
```

---

### 6. Technical Task API

#### Lấy Danh Sách Technical Task (Cho Technical Staff)
```
POST /api/TechnicalTask/paging
Content-Type: application/json

Request Body:
{
  "page": 1,
  "pageSize": 20,
  "assignedToTechnical": 2,           // ID của technical staff
  "taskStatus": null,                 // Optional: 0=Pending, 1=InProgress, 2=Completed
  "serviceTicketStatus": null         // Optional: 0=PendingTechnicalConfirmation, 1=AdjustedByTechnical, 2=InProgress, 3=Completed
}

Response:
{
  "errorCode": "SUCCESS",
  "userMsg": "Lấy danh sách technical task thành công",
  "data": {
    "items": [
      {
        "technicalTaskId": 1,
        "serviceTicketId": 1,
        "serviceTicketCode": "ST001",
        "description": "Kiểm tra và sửa chữa hệ thống điện",
        "assignedToTechnical": 2,
        "assignedToTechnicalName": "Nguyễn Văn C",
        "assignedAt": "2025-12-03T10:00:00",
        "taskStatus": 0,
        "serviceTicketStatus": 0,
        "customerName": "Nguyễn Văn A",
        "vehicleName": "Honda Civic",
        "vehicleLicensePlate": "30A-12345"
      }
    ],
    "total": 10,
    "page": 1,
    "pageSize": 20
  }
}
```

#### Lấy Chi Tiết Technical Task
```
GET /api/TechnicalTask/{id}

Response:
{
  "errorCode": "SUCCESS",
  "userMsg": "Lấy chi tiết technical task thành công",
  "data": {
    "technicalTaskId": 1,
    "serviceTicketId": 1,
    "serviceTicketCode": "ST001",
    "description": "Kiểm tra và sửa chữa hệ thống điện",
    "assignedToTechnical": { ... },
    "taskStatus": 0,
    "serviceTicketStatus": 0,
    "customer": { ... },
    "vehicle": { ... },
    "parts": [ ... ],
    "garageServices": [ ... ]
  }
}
```

#### Technical Staff Điều Chỉnh Parts và Services
```
PUT /api/TechnicalTask/{id}/adjust
Content-Type: application/json

Request Body:
{
  "parts": [
    {
      "partId": 1,
      "quantity": 3                    // Thay đổi số lượng
    },
    {
      "partId": 2,                     // Thêm part mới
      "quantity": 1
    }
  ],
  "garageServices": [
    {
      "garageServiceId": 1,
      "quantity": 1
    }
  ]
}

Lưu ý:
- Thay thế toàn bộ parts/services cũ
- Tự động rollback part quantity từ parts cũ
- Status chuyển thành: AdjustedByTechnical (1)
- Chỉ cho phép khi status = PendingTechnicalConfirmation (0)
```

#### Staff Xác Nhận Điều Chỉnh
```
PUT /api/TechnicalTask/{id}/confirm?confirmedBy=1

Lưu ý:
- Chỉ cho phép khi status = AdjustedByTechnical (1)
- Status chuyển thành: InProgress (2)
```

#### Technical Staff Hoàn Thành Công Việc
```
PUT /api/TechnicalTask/{id}/complete

Lưu ý:
- Chỉ cho phép khi status = InProgress (2)
- Tự động deduct part quantity
- Status chuyển thành: Completed (3)
```

---

### 7. Invoice API

#### Tạo Invoice từ Service Ticket
```
POST /api/Invoice
Content-Type: application/json

Request Body:
{
  "serviceTicketId": 1,               // Service Ticket đã hoàn thành
  "customerId": 1,
  "taxAmount": 10000,                  // Optional: Thuế VAT
  "discountAmount": 5000,              // Optional: Giảm giá
  "invoiceCode": "INV001"              // Optional: Có thể tự động generate
}

Response:
{
  "errorCode": "SUCCESS",
  "userMsg": "Tạo invoice thành công",
  "data": {
    "invoiceId": 1
  }
}

Lưu ý:
- Chỉ tạo được khi Service Ticket status = Completed (3)
- Tự động tính PartsAmount, GarageServiceAmount từ Service Ticket
- TotalAmount = PartsAmount + GarageServiceAmount + TaxAmount - DiscountAmount
```

#### Lấy Danh Sách Invoice (Phân Trang)
```
POST /api/Invoice/paging
Content-Type: application/json

Request Body:
{
  "page": 1,
  "pageSize": 20,
  "customerId": 1,                     // Optional
  "invoiceStatus": null,               // Optional
  "fromDate": "2025-12-01",            // Optional
  "toDate": "2025-12-31"               // Optional
}
```

#### Lấy Chi Tiết Invoice
```
GET /api/Invoice/{id}

Response:
{
  "errorCode": "SUCCESS",
  "userMsg": "Lấy chi tiết invoice thành công",
  "data": {
    "invoiceId": 1,
    "serviceTicketId": 1,
    "serviceTicketCode": "ST001",
    "customer": { ... },
    "vehicle": { ... },
    "issueDate": "2025-12-03T10:00:00",
    "partsAmount": 200000,
    "garageServiceAmount": 50000,
    "taxAmount": 10000,
    "discountAmount": 5000,
    "totalAmount": 255000,
    "invoiceStatus": 0,
    "invoiceCode": "INV001",
    "parts": [ ... ],
    "garageServices": [ ... ]
  }
}
```

---

## Status Codes

### Service Ticket Status
- `0` - PendingTechnicalConfirmation: Đang chờ technical xác nhận
- `1` - AdjustedByTechnical: Được điều chỉnh từ technical staff
- `2` - InProgress: Đang làm
- `3` - Completed: Đã hoàn thành
- `4` - Cancelled: Đã hủy

### Technical Task Status
- `0` - Pending: Chờ xác nhận
- `1` - InProgress: Đang làm
- `2` - Completed: Đã hoàn thành

---

## Error Handling

Tất cả API trả về format chuẩn:

```json
{
  "errorCode": "ERR_400",
  "devMsg": "Chi tiết lỗi cho developer",
  "userMsg": "Thông báo lỗi cho người dùng",
  "traceId": "xxx"
}
```

Các mã lỗi thường gặp:
- `ERR_400`: Bad Request (validation error)
- `ERR_404`: Not Found
- `ERR_409`: Conflict (duplicate)
- `ERR_500`: Internal Server Error

---

## Best Practices

1. **Tìm kiếm Customer/Vehicle/Part/Service**: Luôn sử dụng search API trước khi cho phép user chọn
2. **Validation**: Frontend nên validate:
   - Vehicle phải thuộc về Customer được chọn
   - Part quantity phải <= PartQuantity hiện có
   - Service Ticket chỉ hủy được khi chưa hoàn thành
3. **Status Flow**: Tuân thủ đúng luồng status:
   - PendingTechnicalConfirmation → AdjustedByTechnical → InProgress → Completed
4. **Part Quantity**: Hệ thống tự động quản lý part quantity:
   - Rollback khi điều chỉnh hoặc hủy
   - Deduct khi hoàn thành task

---

## Ví Dụ Luồng Hoàn Chỉnh

### Scenario: Staff tạo phiếu dịch vụ và technical hoàn thành

1. **Staff tìm customer**
   ```
   POST /api/Customer/search
   { "searchKeyword": "Nguyễn" }
   ```

2. **Staff tìm vehicle của customer**
   ```
   POST /api/Vehicle/search
   { "customerId": 1, "searchKeyword": "Honda" }
   ```

3. **Staff tìm parts**
   ```
   GET /api/Part/search?searchKeyword=nhớt
   ```

4. **Staff tìm services**
   ```
   POST /api/GarageService/search
   { "searchKeyword": "thay nhớt" }
   ```

5. **Staff tạo service ticket**
   ```
   POST /api/ServiceTicket/create-with-parts-services
   {
     "customerId": 1,
     "vehicleId": 1,
     "createdBy": 1,
     "assignedToTechnical": 2,
     "assignDescription": "Thay nhớt và kiểm tra",
     "parts": [{ "partId": 1, "quantity": 2 }],
     "garageServices": [{ "garageServiceId": 1, "quantity": 1 }]
   }
   ```
   → Status: `PendingTechnicalConfirmation` (0)

6. **Technical xem danh sách công việc**
   ```
   POST /api/TechnicalTask/paging
   { "assignedToTechnical": 2 }
   ```

7. **Technical điều chỉnh parts**
   ```
   PUT /api/TechnicalTask/1/adjust
   {
     "parts": [{ "partId": 1, "quantity": 3 }],
     "garageServices": [{ "garageServiceId": 1, "quantity": 1 }]
   }
   ```
   → Status: `AdjustedByTechnical` (1)

8. **Staff xác nhận**
   ```
   PUT /api/TechnicalTask/1/confirm?confirmedBy=1
   ```
   → Status: `InProgress` (2)

9. **Technical hoàn thành**
   ```
   PUT /api/TechnicalTask/1/complete
   ```
   → Status: `Completed` (3)
   → Part quantity được deduct

10. **Staff xuất hóa đơn**
    ```
    POST /api/Invoice
    {
      "serviceTicketId": 1,
      "customerId": 1,
      "taxAmount": 10000,
      "discountAmount": 5000
    }
    ```

---

## Notes

- Tất cả API đều yêu cầu authentication (nếu có)
- Timestamps theo format ISO 8601
- Số tiền tính bằng VNĐ (decimal)
- Part quantity được quản lý tự động, không cần frontend xử lý

