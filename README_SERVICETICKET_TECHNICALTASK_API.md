# Hướng Dẫn Sử Dụng API ServiceTicket và TechnicalTask

## Mục Lục
1. [ServiceTicket API](#serviceticket-api)
2. [TechnicalTask API](#technicaltask-api)
3. [Luồng Nghiệp Vụ](#luồng-nghiệp-vụ)
4. [Status Constants](#status-constants)

---

## ServiceTicket API

### Base URL
```
https://localhost:8080/api/ServiceTicket
```

### 1. Lấy Danh Sách Service Ticket (Phân Trang)

**Endpoint:** `POST /api/ServiceTicket/paging`

**Request Body:**
```json
{
  "page": 1,
  "pageSize": 10,
  "columnFilters": [
    {
      "columnName": "ServiceTicketCode",
      "filterValue": "ST001",
      "filterType": "equals"
    },
    {
      "columnName": "ServiceTicketStatus",
      "filterValue": "0",
      "filterType": "equals"
    }
  ],
  "columnSorts": [
    {
      "columnName": "CreatedDate",
      "sortType": "desc"
    }
  ]
}
```

**Response:**
```json
{
  "errorCode": null,
  "userMsg": "Lấy danh sách service ticket thành công",
  "data": {
    "items": [
      {
        "serviceTicketId": 1,
        "serviceTicketCode": "ST001",
        "customerName": "Nguyễn Văn A",
        "customerPhone": "0123456789",
        "vehicleName": "Honda Civic",
        "vehicleLicensePlate": "30A-12345",
        "createdByName": "Staff 1",
        "createdDate": "2025-12-03T10:00:00",
        "serviceTicketStatus": 0,
        "assignedToTechnical": 2
      }
    ],
    "total": 100,
    "page": 1,
    "pageSize": 10
  }
}
```

**Lưu ý:**
- `columnFilters`: Tùy chọn, có thể filter theo nhiều cột
- `columnSorts`: Tùy chọn, có thể sort theo nhiều cột
- Các cột có thể filter/sort: `ServiceTicketCode`, `ServiceTicketStatus`, `CustomerName`, `CustomerPhone`, `VehicleName`, `VehicleLicensePlate`, `CreatedByName`, `CreatedDate`, `ModifiedDate`

---

### 2. Lấy Chi Tiết Service Ticket

**Endpoint:** `GET /api/ServiceTicket/{id}`

**Ví dụ:** `GET /api/ServiceTicket/1`

**Response:**
```json
{
  "errorCode": null,
  "userMsg": "Lấy chi tiết service ticket thành công",
  "data": {
    "serviceTicketId": 1,
    "serviceTicketCode": "ST001",
    "bookingId": null,
    "vehicle": {
      "vehicleId": 1,
      "vehicleName": "Honda Civic",
      "vehicleLicensePlate": "30A-12345",
      "currentKm": 50000,
      "make": "Honda",
      "model": "Civic"
    },
    "customer": {
      "customerId": 1,
      "customerName": "Nguyễn Văn A",
      "customerPhone": "0123456789",
      "customerEmail": "nguyenvana@example.com"
    },
    "createdByUser": {
      "userId": 1,
      "fullName": "Staff 1",
      "email": "staff1@example.com",
      "phone": "0987654321"
    },
    "modifiedByUser": null,
    "createdDate": "2025-12-03T10:00:00",
    "modifiedDate": null,
    "serviceTicketStatus": 0,
    "initialIssue": "Xe bị lỗi hệ thống điện",
    "parts": [
      {
        "serviceTicketDetailId": 1,
        "part": {
          "partId": 1,
          "partName": "Dầu nhớt",
          "partCode": "OIL001",
          "partPrice": 250000,
          "partQuantity": 100,
          "partUnit": "Lít",
          "partCategory": {
            "partCategoryId": 1,
            "partCategoryName": "Dầu nhớt",
            "partCategoryCode": "DM1"
          }
        },
        "quantity": 2
      }
    ],
    "garageServices": [
      {
        "serviceTicketDetailId": 2,
        "garageService": {
          "garageServiceId": 1,
          "garageServiceName": "Thay dầu nhớt",
          "garageServicePrice": 50000
        }
      }
    ],
    "technicalTasks": [
      {
        "technicalTaskId": 1,
        "serviceTicketId": 1,
        "serviceTicketCode": "ST001",
        "description": "Kiểm tra và sửa chữa hệ thống điện",
        "assignedToTechnical": {
          "userId": 2,
          "fullName": "Nguyễn Văn C",
          "email": "tech1@example.com",
          "phone": "0912345678"
        },
        "assignedAt": "2025-12-03T10:30:00",
        "taskStatus": 0,
        "confirmedBy": null,
        "confirmedAt": null
      }
    ]
  }
}
```

---

### 3. Tạo Mới Service Ticket

**Endpoint:** `POST /api/ServiceTicket`

**Request Body:**
```json
{
  "bookingId": null,
  "customerId": null,
  "customerInfo": {
    "customerName": "Nguyễn Văn A",
    "customerPhone": "0123456789",
    "customerEmail": "nguyenvana@example.com"
  },
  "vehicleId": null,
  "vehicleInfo": {
    "vehicleName": "Honda Civic",
    "vehicleLicensePlate": "30A-12345",
    "currentKm": 50000,
    "make": "Honda",
    "model": "Civic"
  },
  "createdBy": 1,
  "initialIssue": "Xe bị lỗi hệ thống điện",
  "serviceTicketCode": "ST001",
  "assignedToTechnical": 2,
  "assignDescription": "Kiểm tra và sửa chữa hệ thống điện",
  "parts": [
    {
      "partId": 1,
      "quantity": 2
    },
    {
      "partId": 3,
      "quantity": 1
    }
  ],
  "garageServiceIds": [1, 2]
}
```

**Response:**
```json
{
  "errorCode": null,
  "userMsg": "Tạo service ticket thành công",
  "data": {
    "serviceTicketId": 1
  }
}
```

**Lưu ý:**
- Nếu `customerId` có giá trị, sẽ dùng customer đó (bỏ qua `customerInfo`)
- Nếu `customerId` null, sẽ tạo customer mới từ `customerInfo`
- Tương tự với `vehicleId` và `vehicleInfo`
- Nếu `assignedToTechnical` có giá trị, sẽ tự động tạo `TechnicalTask` và set status = `PendingTechnicalConfirmation` (0)
- `parts`: Danh sách parts với quantity (sẽ trừ số lượng trong kho)
- `garageServiceIds`: Danh sách ID của garage services (không có quantity)

---

### 4. Cập Nhật Service Ticket

**Endpoint:** `PUT /api/ServiceTicket/{id}`

**Request Body:**
```json
{
  "bookingId": null,
  "vehicleInfo": {
    "vehicleName": "Honda Civic Updated",
    "vehicleLicensePlate": "30A-12345",
    "currentKm": 51000,
    "make": "Honda",
    "model": "Civic"
  },
  "customerInfo": {
    "customerName": "Nguyễn Văn A Updated",
    "customerPhone": "0123456789",
    "customerEmail": "nguyenvana@example.com"
  },
  "modifiedBy": 1,
  "initialIssue": "Xe bị lỗi hệ thống điện - đã kiểm tra",
  "serviceTicketCode": "ST001",
  "parts": [
    {
      "partId": 1,
      "quantity": 3
    },
    {
      "partId": 2,
      "quantity": 1
    }
  ],
  "garageServiceIds": [1, 3]
}
```

**Response:**
```json
{
  "errorCode": null,
  "userMsg": "Cập nhật service ticket thành công",
  "data": {
    "affectedRows": 1
  }
}
```

**Lưu ý:**
- Nếu `parts` có giá trị, sẽ thay thế toàn bộ parts cũ (rollback quantity cũ, trừ quantity mới)
- Nếu `garageServiceIds` có giá trị, sẽ thay thế toàn bộ garage services cũ
- Chỉ cập nhật thông tin vehicle/customer, không đổi `vehicle_id` hoặc `customer_id`

---

### 5. Xóa Service Ticket

**Endpoint:** `DELETE /api/ServiceTicket/{id}`

**Response:**
```json
{
  "errorCode": null,
  "userMsg": "Xóa service ticket thành công",
  "data": {
    "affectedRows": 1
  }
}
```

**Lưu ý:**
- Soft delete (set `is_deleted = 1`)
- Tự động rollback part quantity

---

### 6. Assign Service Ticket Cho Technical Staff

**Endpoint:** `POST /api/ServiceTicket/{id}/assign`

**Request Body:**
```json
{
  "assignedToTechnical": 2,
  "description": "Kiểm tra và sửa chữa hệ thống điện"
}
```

**Response:**
```json
{
  "errorCode": null,
  "userMsg": "Assign service ticket cho technical staff thành công",
  "data": {
    "technicalTaskId": 1
  }
}
```

**Lưu ý:**
- Tự động tạo `TechnicalTask`
- Set status = `PendingTechnicalConfirmation` (0)

---

### 7. Thay Đổi Status Service Ticket

#### 7.1. Chuyển Sang PendingTechnicalConfirmation (0)

**Endpoint:** `PUT /api/ServiceTicket/{id}/status/pending`

**Request Body:**
```json
{
  "modifiedBy": 1,
  "note": "Chuyển về chờ technical xác nhận"
}
```

#### 7.2. Chuyển Sang AdjustedByTechnical (1)

**Endpoint:** `PUT /api/ServiceTicket/{id}/status/adjusted`

**Request Body:**
```json
{
  "modifiedBy": 1,
  "note": "Technical đã điều chỉnh"
}
```

#### 7.3. Chuyển Sang InProgress (2)

**Endpoint:** `PUT /api/ServiceTicket/{id}/status/in-progress`

**Request Body:**
```json
{
  "modifiedBy": 1,
  "note": "Bắt đầu làm việc"
}
```

#### 7.4. Chuyển Sang Completed (3)

**Endpoint:** `PUT /api/ServiceTicket/{id}/status/completed`

**Request Body:**
```json
{
  "modifiedBy": 1,
  "note": "Hoàn thành công việc"
}
```

#### 7.5. Chuyển Sang Cancelled (4)

**Endpoint:** `PUT /api/ServiceTicket/{id}/status/cancelled`

**Request Body:**
```json
{
  "modifiedBy": 1,
  "note": "Hủy service ticket"
}
```

**Lưu ý:**
- Chỉ hủy được khi status chưa phải `Completed` (3)
- Tự động rollback part quantity khi hủy

---

### 8. Lấy Danh Sách Tasks Của Mechanic

**Endpoint:** `POST /api/ServiceTicket/mechanic/{mechanicId}/tasks`

**Request Body:** (Giống như ServiceTicket paging)
```json
{
  "page": 1,
  "pageSize": 10,
  "columnFilters": [],
  "columnSorts": []
}
```

**Response:**
```json
{
  "errorCode": null,
  "userMsg": "Lấy danh sách tasks thành công",
  "data": {
    "items": [
      {
        "technicalTaskId": 1,
        "serviceTicketId": 1,
        "serviceTicketCode": "ST001",
        "description": "Kiểm tra và sửa chữa hệ thống điện",
        "assignedAt": "2025-12-03T10:30:00",
        "taskStatus": 0,
        "serviceTicketStatus": 0,
        "customerName": "Nguyễn Văn A",
        "vehicleName": "Honda Civic",
        "vehicleLicensePlate": "30A-12345"
      }
    ],
    "total": 10,
    "page": 1,
    "pageSize": 10
  }
}
```

---

### 9. Lấy Chi Tiết Task Của Mechanic

**Endpoint:** `GET /api/ServiceTicket/mechanic/{mechanicId}/tasks/{technicalTaskId}`

**Response:**
```json
{
  "errorCode": null,
  "userMsg": "Lấy chi tiết task thành công",
  "data": {
    "technicalTaskId": 1,
    "serviceTicketId": 1,
    "serviceTicketCode": "ST001",
    "description": "Kiểm tra và sửa chữa hệ thống điện",
    "assignedAt": "2025-12-03T10:30:00",
    "taskStatus": 0,
    "confirmedAt": null,
    "serviceTicket": {
      "serviceTicketId": 1,
      "serviceTicketCode": "ST001",
      "serviceTicketStatus": 0,
      "initialIssue": "Xe bị lỗi hệ thống điện",
      "vehicle": {
        "vehicleId": 1,
        "vehicleName": "Honda Civic",
        "vehicleLicensePlate": "30A-12345",
        "currentKm": 50000,
        "make": "Honda",
        "model": "Civic"
      },
      "customer": {
        "customerId": 1,
        "customerName": "Nguyễn Văn A",
        "customerPhone": "0123456789",
        "customerEmail": "nguyenvana@example.com"
      }
    },
    "parts": [
      {
        "serviceTicketDetailId": 1,
        "part": {
          "partId": 1,
          "partName": "Dầu nhớt",
          "partCode": "OIL001",
          "partPrice": 250000,
          "partQuantity": 100,
          "partUnit": "Lít",
          "partCategory": {
            "partCategoryId": 1,
            "partCategoryName": "Dầu nhớt",
            "partCategoryCode": "DM1"
          }
        },
        "quantity": 2
      }
    ],
    "garageServices": [
      {
        "serviceTicketDetailId": 2,
        "garageService": {
          "garageServiceId": 1,
          "garageServiceName": "Thay dầu nhớt",
          "garageServicePrice": 50000
        }
      }
    ]
  }
}
```

---

## TechnicalTask API

### Base URL
```
https://localhost:8080/api/TechnicalTask
```

### 1. Lấy Danh Sách Technical Task (Phân Trang)

**Endpoint:** `POST /api/TechnicalTask/paging`

**Request Body:**
```json
{
  "page": 1,
  "pageSize": 10,
  "assignedToTechnical": 2,
  "taskStatus": 0,
  "serviceTicketStatus": 0,
  "columnFilters": [],
  "columnSorts": []
}
```

**Response:**
```json
{
  "errorCode": null,
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
        "assignedAt": "2025-12-03T10:30:00",
        "taskStatus": 0,
        "confirmedBy": null,
        "confirmedByName": null,
        "confirmedAt": null,
        "serviceTicketStatus": 0,
        "customerName": "Nguyễn Văn A",
        "vehicleName": "Honda Civic",
        "vehicleLicensePlate": "30A-12345"
      }
    ],
    "total": 10,
    "page": 1,
    "pageSize": 10
  }
}
```

**Lưu ý:**
- `assignedToTechnical`: Filter theo technical staff ID
- `taskStatus`: Filter theo trạng thái task (0: Pending, 1: InProgress, 2: Completed)
- `serviceTicketStatus`: Filter theo trạng thái service ticket
- Mặc định chỉ lấy service ticket chưa hoàn thành (status != 3)

---

### 2. Lấy Chi Tiết Technical Task

**Endpoint:** `GET /api/TechnicalTask/{id}`

**Ví dụ:** `GET /api/TechnicalTask/1`

**Response:**
```json
{
  "errorCode": null,
  "userMsg": "Lấy chi tiết technical task thành công",
  "data": {
    "technicalTaskId": 1,
    "serviceTicketId": 1,
    "serviceTicketCode": "ST001",
    "description": "Kiểm tra và sửa chữa hệ thống điện",
    "assignedToTechnical": {
      "userId": 2,
      "fullName": "Nguyễn Văn C",
      "email": "tech1@example.com",
      "phone": "0912345678"
    },
    "assignedAt": "2025-12-03T10:30:00",
    "taskStatus": 0,
    "confirmedBy": null,
    "confirmedAt": null,
    "serviceTicketStatus": 0,
    "customer": {
      "customerId": 1,
      "customerName": "Nguyễn Văn A",
      "customerPhone": "0123456789",
      "customerEmail": "nguyenvana@example.com"
    },
    "vehicle": {
      "vehicleId": 1,
      "vehicleName": "Honda Civic",
      "vehicleLicensePlate": "30A-12345",
      "currentKm": 50000,
      "make": "Honda",
      "model": "Civic"
    },
    "parts": [
      {
        "serviceTicketDetailId": 1,
        "part": {
          "partId": 1,
          "partName": "Dầu nhớt",
          "partCode": "OIL001",
          "partPrice": 250000,
          "partQuantity": 100,
          "partUnit": "Lít",
          "partCategory": {
            "partCategoryId": 1,
            "partCategoryName": "Dầu nhớt",
            "partCategoryCode": "DM1"
          }
        },
        "quantity": 2
      }
    ],
    "garageServices": [
      {
        "serviceTicketDetailId": 2,
        "garageService": {
          "garageServiceId": 1,
          "garageServiceName": "Thay dầu nhớt",
          "garageServicePrice": 50000
        }
      }
    ]
  }
}
```

---

### 3. Technical Staff Điều Chỉnh Parts và Services

**Endpoint:** `PUT /api/TechnicalTask/{id}/adjust`

**Request Body:**
```json
{
  "parts": [
    {
      "partId": 1,
      "quantity": 3
    },
    {
      "partId": 2,
      "quantity": 1
    }
  ],
  "garageServices": [
    {
      "garageServiceId": 1,
      "quantity": 1
    },
    {
      "garageServiceId": 2,
      "quantity": 1
    }
  ]
}
```

**Response:**
```json
{
  "errorCode": null,
  "userMsg": "Điều chỉnh parts và services thành công",
  "data": {
    "affectedRows": 1
  }
}
```

**Lưu ý:**
- Chỉ cho phép khi service ticket status = `PendingTechnicalConfirmation` (0)
- Thay thế toàn bộ parts/services cũ
- Tự động rollback part quantity từ parts cũ
- Tự động set service ticket status = `AdjustedByTechnical` (1)
- `garageServices` có `quantity` nhưng không ảnh hưởng đến inventory

---

### 4. Staff Xác Nhận Điều Chỉnh Từ Technical

**Endpoint:** `PUT /api/TechnicalTask/{id}/confirm?confirmedBy=1`

**Response:**
```json
{
  "errorCode": null,
  "userMsg": "Xác nhận điều chỉnh thành công",
  "data": {
    "affectedRows": 1
  }
}
```

**Lưu ý:**
- Chỉ cho phép khi service ticket status = `AdjustedByTechnical` (1)
- Tự động set service ticket status = `InProgress` (2)
- Cập nhật `TechnicalTask.confirmedBy` và `confirmedAt`

---

### 5. Technical Staff Hoàn Thành Công Việc

**Endpoint:** `PUT /api/TechnicalTask/{id}/complete`

**Response:**
```json
{
  "errorCode": null,
  "userMsg": "Hoàn thành công việc thành công",
  "data": {
    "affectedRows": 1
  }
}
```

**Lưu ý:**
- Chỉ cho phép khi service ticket status = `InProgress` (2)
- Tự động set service ticket status = `Completed` (3)
- Tự động deduct part quantity từ inventory
- Set `TechnicalTask.taskStatus` = 2 (Completed)

---

## Luồng Nghiệp Vụ

### Luồng Tạo Service Ticket và Xử Lý

1. **Staff tạo Service Ticket**
   - `POST /api/ServiceTicket`
   - Có thể chọn customer/vehicle có sẵn hoặc tạo mới
   - Có thể thêm parts và garage services ngay khi tạo
   - Có thể assign cho technical staff ngay (tạo TechnicalTask)
   - Status: `PendingTechnicalConfirmation` (0)

2. **Technical Staff xem danh sách tasks**
   - `POST /api/TechnicalTask/paging` với `assignedToTechnical = {technicalId}`
   - Hoặc `POST /api/ServiceTicket/mechanic/{mechanicId}/tasks`

3. **Technical Staff xem chi tiết task**
   - `GET /api/TechnicalTask/{id}`
   - Hoặc `GET /api/ServiceTicket/mechanic/{mechanicId}/tasks/{technicalTaskId}`

4. **Technical Staff điều chỉnh parts/services**
   - `PUT /api/TechnicalTask/{id}/adjust`
   - Status chuyển: `PendingTechnicalConfirmation` (0) → `AdjustedByTechnical` (1)

5. **Staff xác nhận điều chỉnh**
   - `PUT /api/TechnicalTask/{id}/confirm?confirmedBy={staffId}`
   - Status chuyển: `AdjustedByTechnical` (1) → `InProgress` (2)

6. **Technical Staff hoàn thành công việc**
   - `PUT /api/TechnicalTask/{id}/complete`
   - Status chuyển: `InProgress` (2) → `Completed` (3)
   - Tự động deduct part quantity

7. **Staff tạo Invoice** (sau khi completed)
   - Sử dụng Invoice API (không nằm trong scope này)

### Luồng Hủy Service Ticket

- Chỉ hủy được khi status chưa phải `Completed` (3)
- `PUT /api/ServiceTicket/{id}/status/cancelled`
- Tự động rollback part quantity

---

## Status Constants

### ServiceTicket Status

| Giá trị | Constant | Mô tả |
|---------|----------|-------|
| 0 | `PendingTechnicalConfirmation` | Đang chờ technical xác nhận |
| 1 | `AdjustedByTechnical` | Được điều chỉnh từ technical staff |
| 2 | `InProgress` | Đang làm |
| 3 | `Completed` | Đã hoàn thành |
| 4 | `Cancelled` | Đã hủy |

### TechnicalTask Status

| Giá trị | Mô tả |
|---------|-------|
| 0 | Pending (Chờ xử lý) |
| 1 | InProgress (Đang làm) |
| 2 | Completed (Đã hoàn thành) |

---

## Error Handling

Tất cả các API đều trả về format chuẩn:

**Success:**
```json
{
  "errorCode": null,
  "userMsg": "Thông báo thành công",
  "data": { ... }
}
```

**Error:**
```json
{
  "errorCode": "ERR_404",
  "devMsg": "Không tìm thấy service ticket.",
  "userMsg": "Không tìm thấy dữ liệu bạn yêu cầu.",
  "traceId": "0HNHI9MHBVGDC:0000000B"
}
```

**Các Error Code phổ biến:**
- `ERR_400`: Bad Request (validation error)
- `ERR_404`: Not Found
- `ERR_409`: Conflict (business rule violation)
- `ERR_500`: Internal Server Error

---

## Notes

1. **Part Quantity Management:**
   - Khi tạo/cập nhật service ticket với parts: Tự động trừ quantity
   - Khi technical điều chỉnh: Rollback quantity cũ, trừ quantity mới
   - Khi hủy: Rollback quantity
   - Khi hoàn thành: Deduct quantity (nếu chưa deduct)

2. **Garage Service:**
   - Không có quantity trong inventory
   - Chỉ cần `garageServiceId` khi thêm vào service ticket

3. **Customer/Vehicle:**
   - Có thể chọn có sẵn hoặc tạo mới
   - Khi update, chỉ cập nhật thông tin, không đổi ID

4. **TechnicalTask:**
   - Tự động tạo khi assign service ticket cho technical staff
   - Mỗi service ticket có thể có nhiều technical tasks (theo thiết kế hiện tại)

---

## Ví Dụ Sử Dụng (Curl)

### Tạo Service Ticket
```bash
curl -X POST 'https://localhost:8080/api/ServiceTicket' \
  -H 'Content-Type: application/json' \
  -d '{
    "customerInfo": {
      "customerName": "Nguyễn Văn A",
      "customerPhone": "0123456789",
      "customerEmail": "nguyenvana@example.com"
    },
    "vehicleInfo": {
      "vehicleName": "Honda Civic",
      "vehicleLicensePlate": "30A-12345",
      "currentKm": 50000,
      "make": "Honda",
      "model": "Civic"
    },
    "createdBy": 1,
    "initialIssue": "Xe bị lỗi hệ thống điện",
    "assignedToTechnical": 2,
    "assignDescription": "Kiểm tra và sửa chữa",
    "parts": [
      {
        "partId": 1,
        "quantity": 2
      }
    ],
    "garageServiceIds": [1]
  }'
```

### Technical Staff Điều Chỉnh Parts
```bash
curl -X PUT 'https://localhost:8080/api/TechnicalTask/1/adjust' \
  -H 'Content-Type: application/json' \
  -d '{
    "parts": [
      {
        "partId": 1,
        "quantity": 3
      },
      {
        "partId": 2,
        "quantity": 1
      }
    ],
    "garageServices": [
      {
        "garageServiceId": 1,
        "quantity": 1
      }
    ]
  }'
```

### Staff Xác Nhận Điều Chỉnh
```bash
curl -X PUT 'https://localhost:8080/api/TechnicalTask/1/confirm?confirmedBy=1'
```

### Technical Staff Hoàn Thành
```bash
curl -X PUT 'https://localhost:8080/api/TechnicalTask/1/complete'
```

---

**Tài liệu này được tạo bởi: DuyLC (03/12/2025)**

