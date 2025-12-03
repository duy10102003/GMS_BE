# Service Ticket API Documentation

## 📋 Mục lục
- [Tổng quan](#tổng-quan)
- [Roles và Quyền hạn](#roles-và-quyền-hạn)
- [Luồng Nghiệp vụ](#luồng-nghiệp-vụ)
- [Trạng thái (Status)](#trạng-thái-status)
- [API Endpoints](#api-endpoints)
- [Ví dụ Request/Response](#ví-dụ-requestresponse)
- [Lưu ý](#lưu-ý)

---

## 📖 Tổng quan

Hệ thống Service Ticket quản lý quy trình sửa chữa xe trong garage, bao gồm:
- **Staff**: Tạo và quản lý service tickets, assign cho mechanic, duyệt đề xuất
- **Mechanic**: Nhận tasks, đề xuất parts/services, thực hiện và hoàn thành công việc

**Base URL**: `https://your-api-domain.com/api/serviceticket`

**Response Format**: Tất cả API đều trả về format chuẩn:
```json
{
  "success": true,
  "data": { ... },
  "message": "Thông báo"
}
```

---

## 👥 Roles và Quyền hạn

### Staff
- ✅ Tạo Service Ticket (nhập/chọn customer, vehicle)
- ✅ Cập nhật thông tin Service Ticket (customer, vehicle, status)
- ✅ Assign Service Ticket cho Mechanic
- ✅ Thêm/xóa Parts và Garage Services
- ✅ Duyệt đề xuất Parts/Services từ Mechanic
- ✅ Xem danh sách và chi tiết Service Ticket
- ✅ Xóa Service Ticket

### Mechanic
- ✅ Xem danh sách tasks được assign
- ✅ Xem chi tiết task
- ✅ Đề xuất Parts và Services (gửi cho Staff duyệt)
- ✅ Bắt đầu task
- ✅ Confirm task hoàn thành

---

## 🔄 Luồng Nghiệp vụ

### Luồng chính

```
1. Staff tạo Service Ticket
   ├─ Nhập thông tin Customer (mới hoặc chọn từ DB)
   ├─ Nhập thông tin Vehicle (mới hoặc chọn từ DB)
   └─ (Tùy chọn) Assign ngay cho Mechanic
   
2. Staff Assign Service Ticket cho Mechanic
   ├─ Tạo TechnicalTask
   └─ ServiceTicket Status → Assigned (1)
   
3. Mechanic xem tasks của mình
   └─ GET /api/serviceticket/mechanic/{mechanicId}/tasks
   
4. Mechanic đề xuất Parts/Services
   ├─ POST /api/serviceticket/technical-tasks/{id}/propose
   └─ Gửi danh sách parts/services cần dùng
   
5. Staff duyệt đề xuất
   ├─ POST /api/serviceticket/technical-tasks/{id}/approve
   └─ ServiceTicket Status → InProgress (2)
   
6. Mechanic bắt đầu task
   ├─ POST /api/serviceticket/technical-tasks/{id}/start
   └─ TaskStatus → InProgress (1)
   
7. Mechanic confirm task hoàn thành
   ├─ POST /api/serviceticket/technical-tasks/{id}/confirm
   ├─ TaskStatus → Completed (2)
   └─ Nếu tất cả tasks hoàn thành → ServiceTicket Status → Completed (3)
```

### Luồng thay thế (Staff thêm Parts/Services trực tiếp)

```
1. Staff tạo Service Ticket
2. Staff thêm Parts/Services trực tiếp
   ├─ POST /api/serviceticket/{id}/parts
   └─ POST /api/serviceticket/{id}/garage-services
3. Staff Assign cho Mechanic
4. Mechanic bắt đầu và hoàn thành task
```

---

## 📊 Trạng thái (Status)

### Service Ticket Status

| Giá trị | Tên | Mô tả |
|---------|-----|-------|
| `0` | `Pending` | Mới tạo, chờ assign cho mechanic |
| `1` | `Assigned` | Đã assign cho technical staff |
| `2` | `InProgress` | Mechanic đã nhận task và đang thực hiện |
| `3` | `Completed` | Đã hoàn thành |
| `4` | `Cancelled` | Đã hủy |

### Technical Task Status

| Giá trị | Tên | Mô tả |
|---------|-----|-------|
| `0` | `Pending` | Mới được assign, chưa bắt đầu |
| `1` | `InProgress` | Đang thực hiện |
| `2` | `Completed` | Đã hoàn thành |

---

## 🔌 API Endpoints

### 1. Phân trang Service Ticket

**POST** `/api/serviceticket/paging`

Lấy danh sách Service Ticket có phân trang, filter và sort.

**Request Body:**
```json
{
  "page": 1,
  "pageSize": 10,
  "columnFilters": [
    {
      "columnName": "ServiceTicketCode",
      "operator": "contains",
      "value": "ST"
    },
    {
      "columnName": "ServiceTicketStatus",
      "operator": "equals",
      "value": "2"
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

**Operators hỗ trợ:**
- `equals`, `not_equals`
- `contains`, `not_contains`
- `starts_with`, `ends_with`
- `empty`, `not_empty`
- `greater_than`, `less_than`
- `greater_or_equal`, `less_or_equal`

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [...],
    "total": 100,
    "page": 1,
    "pageSize": 10
  },
  "message": "Lấy danh sách service ticket thành công"
}
```

---

### 2. Lấy chi tiết Service Ticket

**GET** `/api/serviceticket/{id}`

**Response:**
```json
{
  "success": true,
  "data": {
    "serviceTicketId": 1,
    "serviceTicketCode": "ST20241201001",
    "bookingId": null,
    "vehicle": {
      "vehicleId": 1,
      "vehicleName": "Honda Civic",
      "vehicleLicensePlate": "30A-12345",
      "make": "Honda",
      "model": "Civic",
      "currentKm": 50000
    },
    "customer": {
      "customerId": 1,
      "customerName": "Nguyễn Văn A",
      "customerPhone": "0912345678",
      "customerEmail": "nguyenvana@email.com"
    },
    "createdByUser": {
      "userId": 1,
      "fullName": "Staff User",
      "email": "staff@garage.com",
      "phone": "0987654321"
    },
    "serviceTicketStatus": 2,
    "initialIssue": "Xe bị hỏng phanh",
    "parts": [
      {
        "serviceTicketDetailId": 1,
        "part": {
          "partId": 1,
          "partName": "Phanh trước",
          "partCode": "P001",
          "inventoryPrice": 500000,
          "partStock": 10,
          "partUnit": "Cái",
          "supplier": {
            "supplierId": 1,
            "supplierName": "Nhà cung cấp A",
            "supplierCode": "NCC001"
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
          "garageServiceName": "Thay phanh",
          "garageServicePrice": 200000
        },
        "quantity": 1
      }
    ],
    "technicalTasks": [...]
  },
  "message": "Lấy chi tiết service ticket thành công"
}
```

---

### 3. Tạo Service Ticket (Staff)

**POST** `/api/serviceticket`

**Request Body:**
```json
{
  "bookingId": null,
  "customerId": null,
  "customerInfo": {
    "customerName": "Nguyễn Văn A",
    "customerPhone": "0912345678",
    "customerEmail": "nguyenvana@email.com",
    "userId": null
  },
  "vehicleId": null,
  "vehicleInfo": {
    "vehicleName": "Honda Civic",
    "vehicleLicensePlate": "30A-12345",
    "currentKm": 50000,
    "make": "Honda",
    "model": "Civic",
    "customerId": null
  },
  "createdBy": 1,
  "initialIssue": "Xe bị hỏng phanh",
  "serviceTicketCode": null,
  "assignedToTechnical": 2,
  "assignDescription": "Kiểm tra và thay phanh"
}
```

**Lưu ý:**
- Nếu `customerId` có giá trị → dùng customer có sẵn
- Nếu `customerId` null → tạo mới từ `customerInfo`
- Tương tự với `vehicleId` và `vehicleInfo`
- Nếu có `assignedToTechnical` → tự động tạo TechnicalTask

**Response:**
```json
{
  "success": true,
  "data": {
    "serviceTicketId": 1
  },
  "message": "Tạo service ticket thành công"
}
```

---

### 4. Cập nhật Service Ticket (Staff)

**PUT** `/api/serviceticket/{id}`

**Request Body:**
```json
{
  "bookingId": 1,
  "vehicleInfo": {
    "vehicleName": "Honda Civic Updated",
    "vehicleLicensePlate": "30A-12345",
    "currentKm": 51000,
    "make": "Honda",
    "model": "Civic",
    "customerId": 1
  },
  "customerInfo": {
    "customerName": "Nguyễn Văn A Updated",
    "customerPhone": "0912345678",
    "customerEmail": "nguyenvana@email.com",
    "userId": null
  },
  "modifiedBy": 1,
  "initialIssue": "Xe bị hỏng phanh và lốp",
  "serviceTicketCode": "ST20241201001",
  "serviceTicketStatus": 2
}
```

**Lưu ý:**
- Không thể thay đổi `vehicleId` (chỉ cập nhật thông tin vehicle)
- Có thể cập nhật thông tin customer liên quan đến vehicle

---

### 5. Assign Service Ticket cho Mechanic

**POST** `/api/serviceticket/{id}/assign`

**Request Body:**
```json
{
  "assignedToTechnical": 2,
  "description": "Kiểm tra và thay phanh, kiểm tra lốp"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "technicalTaskId": 1
  },
  "message": "Assign service ticket cho technical staff thành công"
}
```

**Kết quả:**
- Tạo TechnicalTask mới
- ServiceTicket Status → `Assigned` (1)

---

### 6. Thêm Part vào Service Ticket

**POST** `/api/serviceticket/{id}/parts`

**Request Body:**
```json
{
  "partId": 1,
  "quantity": 2
}
```

**Validation:**
- Kiểm tra part tồn tại
- Kiểm tra số lượng tồn kho đủ

---

### 7. Thêm Garage Service vào Service Ticket

**POST** `/api/serviceticket/{id}/garage-services`

**Request Body:**
```json
{
  "garageServiceId": 1,
  "quantity": 1
}
```

---

### 8. Xóa Part/Service khỏi Service Ticket

**DELETE** `/api/serviceticket/{id}/details/{detailId}`

**Lưu ý:** Không thể xóa khi ServiceTicket đã Completed

---

### 9. Duyệt đề xuất của Mechanic (Staff)

**POST** `/api/serviceticket/technical-tasks/{technicalTaskId}/approve?staffId=1`

**Request Body:**
```json
{
  "parts": [
    {
      "serviceTicketDetailId": null,
      "partId": 1,
      "quantity": 2
    },
    {
      "serviceTicketDetailId": 3,
      "partId": 2,
      "quantity": 1
    }
  ],
  "garageServices": [
    {
      "serviceTicketDetailId": null,
      "garageServiceId": 1,
      "quantity": 1
    }
  ]
}
```

**Lưu ý:**
- `serviceTicketDetailId` = null → thêm mới
- `serviceTicketDetailId` có giá trị → cập nhật
- Sau khi duyệt → ServiceTicket Status → `InProgress` (2)

---

## 🔧 Mechanic Operations

### 10. Lấy danh sách Tasks của Mechanic

**POST** `/api/serviceticket/mechanic/{mechanicId}/tasks`

**Request Body:** (Tương tự như phân trang Service Ticket)

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "technicalTaskId": 1,
        "serviceTicketId": 1,
        "serviceTicketCode": "ST20241201001",
        "description": "Kiểm tra và thay phanh",
        "assignedAt": "2024-12-01T10:00:00Z",
        "taskStatus": 0,
        "serviceTicket": {
          "serviceTicketId": 1,
          "serviceTicketCode": "ST20241201001",
          "serviceTicketStatus": 1,
          "initialIssue": "Xe bị hỏng phanh",
          "vehicle": { ... },
          "customer": { ... }
        },
        "parts": [...],
        "garageServices": [...]
      }
    ],
    "total": 5,
    "page": 1,
    "pageSize": 10
  },
  "message": "Lấy danh sách tasks thành công"
}
```

---

### 11. Lấy chi tiết Task

**GET** `/api/serviceticket/mechanic/{mechanicId}/tasks/{technicalTaskId}`

---

### 12. Đề xuất Parts/Services (Mechanic)

**POST** `/api/serviceticket/technical-tasks/{technicalTaskId}/propose?mechanicId=2`

**Request Body:**
```json
{
  "parts": [
    {
      "serviceTicketDetailId": null,
      "partId": 1,
      "quantity": 2
    }
  ],
  "garageServices": [
    {
      "serviceTicketDetailId": null,
      "garageServiceId": 1,
      "quantity": 1
    }
  ]
}
```

**Lưu ý:**
- Mechanic chỉ có thể đề xuất task của chính mình
- Đề xuất sẽ được gửi cho Staff duyệt

---

### 13. Bắt đầu Task

**POST** `/api/serviceticket/technical-tasks/{technicalTaskId}/start?mechanicId=2`

**Kết quả:**
- TaskStatus → `InProgress` (1)
- ServiceTicket Status → `InProgress` (2)

**Validation:**
- Chỉ có thể bắt đầu task ở trạng thái `Pending` (0)
- Chỉ mechanic được assign mới có quyền

---

### 14. Confirm Task hoàn thành

**POST** `/api/serviceticket/technical-tasks/{technicalTaskId}/confirm?mechanicId=2`

**Kết quả:**
- TaskStatus → `Completed` (2)
- Nếu tất cả tasks của ServiceTicket đã hoàn thành → ServiceTicket Status → `Completed` (3)

**Validation:**
- Chỉ có thể confirm task ở trạng thái `InProgress` (1)
- Chỉ mechanic được assign mới có quyền

---

## 📝 Ví dụ Request/Response

### Ví dụ 1: Tạo Service Ticket với Customer và Vehicle mới

**Request:**
```http
POST /api/serviceticket
Content-Type: application/json

{
  "customerInfo": {
    "customerName": "Trần Thị B",
    "customerPhone": "0987654321",
    "customerEmail": "tranthib@email.com"
  },
  "vehicleInfo": {
    "vehicleName": "Toyota Camry",
    "vehicleLicensePlate": "29B-67890",
    "currentKm": 30000,
    "make": "Toyota",
    "model": "Camry"
  },
  "createdBy": 1,
  "initialIssue": "Xe cần bảo dưỡng định kỳ"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "serviceTicketId": 2
  },
  "message": "Tạo service ticket thành công"
}
```

---

### Ví dụ 2: Filter và Sort Service Tickets

**Request:**
```http
POST /api/serviceticket/paging
Content-Type: application/json

{
  "page": 1,
  "pageSize": 20,
  "columnFilters": [
    {
      "columnName": "ServiceTicketStatus",
      "operator": "equals",
      "value": "2"
    },
    {
      "columnName": "CustomerName",
      "operator": "contains",
      "value": "Nguyễn"
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

---

## ⚠️ Lưu ý

### Validation Rules

1. **Tạo Service Ticket:**
   - Phải có `CustomerId` hoặc `CustomerInfo`
   - Phải có `VehicleId` hoặc `VehicleInfo`
   - `CustomerPhone` là bắt buộc nếu tạo customer mới
   - `VehicleName` và `VehicleLicensePlate` là bắt buộc nếu tạo vehicle mới

2. **Thêm Part:**
   - Kiểm tra số lượng tồn kho đủ
   - Part phải tồn tại và chưa bị xóa

3. **Assign Task:**
   - Technical staff phải tồn tại
   - `Description` là bắt buộc

4. **Mechanic Operations:**
   - Mechanic chỉ có thể thao tác với tasks được assign cho mình
   - Chỉ có thể bắt đầu task ở trạng thái `Pending`
   - Chỉ có thể confirm task ở trạng thái `InProgress`

5. **Xóa Service Ticket:**
   - Không thể xóa nếu đã có TechnicalTask hoặc Parts

### Error Responses

Tất cả lỗi đều trả về format:
```json
{
  "success": false,
  "data": null,
  "message": "Thông báo lỗi chi tiết"
}
```

**HTTP Status Codes:**
- `200 OK`: Thành công
- `201 Created`: Tạo mới thành công
- `400 Bad Request`: Lỗi validation
- `404 Not Found`: Không tìm thấy resource
- `409 Conflict`: Conflict (ví dụ: mã code đã tồn tại)
- `500 Internal Server Error`: Lỗi server

---

## 🔗 Related Documentation

- [Service Ticket Status Constants](../../SWP.Core/Constants/ServiceTicketStatus/ServiceTicketStatus.cs)
- [Technical Task Entity](../../SWP.Core/Entities/TechnicalTask.cs)
- [Service Ticket Entity](../../SWP.Core/Entities/ServiceTicket.cs)

---

**Created by:** DuyLC  
**Last Updated:** 02/12/2025


