# Garage Service API Documentation

## 📋 Mục lục
- [Tổng quan](#tổng-quan)
- [Quyền truy cập](#quyền-truy-cập)
- [API Endpoints](#api-endpoints)
- [Ví dụ Request/Response](#ví-dụ-requestresponse)
- [Filter và Sort](#filter-và-sort)
- [Lưu ý](#lưu-ý)

---

## 📖 Tổng quan

Garage Service API cho phép Manager quản lý các dịch vụ của garage, bao gồm:
- Tạo mới dịch vụ
- Cập nhật thông tin dịch vụ
- Xóa dịch vụ (soft delete)
- Xem danh sách có phân trang, filter và sort
- Xem chi tiết dịch vụ

**Base URL**: `https://your-api-domain.com/api/garageservice`

**Response Format**: Tất cả API đều trả về format chuẩn:
```json
{
  "success": true,
  "data": { ... },
  "message": "Thông báo"
}
```

---

## 👤 Quyền truy cập

**Manager**: Có quyền thực hiện tất cả các thao tác CRUD trên Garage Service.

---

## 🔌 API Endpoints

### 1. Lấy danh sách Garage Service có phân trang

**POST** `/api/garageservice/paging`

Lấy danh sách Garage Service với phân trang, filter và sort.

**Request Body:**
```json
{
  "page": 1,
  "pageSize": 10,
  "columnFilters": [
    {
      "columnName": "GarageServiceName",
      "operator": "contains",
      "value": "Thay"
    },
    {
      "columnName": "GarageServicePrice",
      "operator": "greater_than",
      "value": "100000"
    }
  ],
  "columnSorts": [
    {
      "columnName": "GarageServicePrice",
      "sortDirection": "DESC"
    }
  ]
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "garageServiceId": 1,
        "garageServiceName": "Thay dầu máy",
        "garageServicePrice": 200000
      },
      {
        "garageServiceId": 2,
        "garageServiceName": "Thay lốp",
        "garageServicePrice": 500000
      }
    ],
    "total": 50,
    "page": 1,
    "pageSize": 10
  },
  "message": "Lấy danh sách garage service thành công"
}
```

---

### 2. Lấy chi tiết Garage Service

**GET** `/api/garageservice/{id}`

**Parameters:**
- `id` (int): ID của Garage Service

**Response:**
```json
{
  "success": true,
  "data": {
    "garageServiceId": 1,
    "garageServiceName": "Thay dầu máy",
    "garageServicePrice": 200000
  },
  "message": "Lấy chi tiết garage service thành công"
}
```

**Error Response (404):**
```json
{
  "success": false,
  "data": null,
  "message": "Không tìm thấy garage service."
}
```

---

### 3. Tạo mới Garage Service

**POST** `/api/garageservice`

**Request Body:**
```json
{
  "garageServiceName": "Thay dầu máy",
  "garageServicePrice": 200000
}
```

**Validation:**
- `garageServiceName`: Bắt buộc, tối đa 255 ký tự
- `garageServicePrice`: Tùy chọn, phải >= 0

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "garageServiceId": 1
  },
  "message": "Tạo garage service thành công"
}
```

**Error Response (400):**
```json
{
  "success": false,
  "data": null,
  "message": "Tên dịch vụ không được để trống."
}
```

---

### 4. Cập nhật Garage Service

**PUT** `/api/garageservice/{id}`

**Parameters:**
- `id` (int): ID của Garage Service cần cập nhật

**Request Body:**
```json
{
  "garageServiceName": "Thay dầu máy cao cấp",
  "garageServicePrice": 250000
}
```

**Validation:**
- `garageServiceName`: Bắt buộc, tối đa 255 ký tự
- `garageServicePrice`: Tùy chọn, phải >= 0

**Response:**
```json
{
  "success": true,
  "data": {
    "affectedRows": 1
  },
  "message": "Cập nhật garage service thành công"
}
```

**Error Response (404):**
```json
{
  "success": false,
  "data": null,
  "message": "Không tìm thấy garage service cần cập nhật."
}
```

---

### 5. Xóa Garage Service

**DELETE** `/api/garageservice/{id}`

**Parameters:**
- `id` (int): ID của Garage Service cần xóa

**Lưu ý:** Xóa mềm (soft delete), không xóa vĩnh viễn khỏi database.

**Response:**
```json
{
  "success": true,
  "data": {
    "affectedRows": 1
  },
  "message": "Xóa garage service thành công"
}
```

**Error Response (404):**
```json
{
  "success": false,
  "data": null,
  "message": "Không tìm thấy garage service."
}
```

---

## 🔍 Filter và Sort

### Filter Operators

Các toán tử filter được hỗ trợ:

#### Text Operators:
- `equals`: Bằng chính xác
- `not_equals`: Không bằng
- `contains`: Chứa chuỗi
- `not_contains`: Không chứa chuỗi
- `starts_with`: Bắt đầu bằng
- `ends_with`: Kết thúc bằng
- `empty`: Rỗng (null hoặc "")
- `not_empty`: Không rỗng

#### Number Operators:
- `equals`: Bằng
- `not_equals`: Không bằng
- `greater_than`: Lớn hơn
- `less_than`: Nhỏ hơn
- `greater_or_equal`: Lớn hơn hoặc bằng
- `less_or_equal`: Nhỏ hơn hoặc bằng

### Sort Direction

- `ASC`: Sắp xếp tăng dần
- `DESC`: Sắp xếp giảm dần

### Các cột có thể Filter/Sort

- `GarageServiceName`: Tên dịch vụ
- `GarageServicePrice`: Giá dịch vụ
- `GarageServiceId`: ID dịch vụ (chỉ sort)

---

## 📝 Ví dụ Request/Response

### Ví dụ 1: Tìm dịch vụ có tên chứa "Thay" và giá > 100000

**Request:**
```http
POST /api/garageservice/paging
Content-Type: application/json

{
  "page": 1,
  "pageSize": 20,
  "columnFilters": [
    {
      "columnName": "GarageServiceName",
      "operator": "contains",
      "value": "Thay"
    },
    {
      "columnName": "GarageServicePrice",
      "operator": "greater_than",
      "value": "100000"
    }
  ],
  "columnSorts": [
    {
      "columnName": "GarageServicePrice",
      "sortDirection": "DESC"
    }
  ]
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "garageServiceId": 5,
        "garageServiceName": "Thay lốp cao su",
        "garageServicePrice": 500000
      },
      {
        "garageServiceId": 3,
        "garageServiceName": "Thay dầu máy",
        "garageServicePrice": 200000
      }
    ],
    "total": 2,
    "page": 1,
    "pageSize": 20
  },
  "message": "Lấy danh sách garage service thành công"
}
```

---

### Ví dụ 2: Tạo mới dịch vụ

**Request:**
```http
POST /api/garageservice
Content-Type: application/json

{
  "garageServiceName": "Bảo dưỡng định kỳ",
  "garageServicePrice": 300000
}
```

**Response:**
```http
HTTP/1.1 201 Created
Location: /api/garageservice/6

{
  "success": true,
  "data": {
    "garageServiceId": 6
  },
  "message": "Tạo garage service thành công"
}
```

---

### Ví dụ 3: Cập nhật giá dịch vụ

**Request:**
```http
PUT /api/garageservice/6
Content-Type: application/json

{
  "garageServiceName": "Bảo dưỡng định kỳ",
  "garageServicePrice": 350000
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "affectedRows": 1
  },
  "message": "Cập nhật garage service thành công"
}
```

---

### Ví dụ 4: Tìm dịch vụ có giá trong khoảng 100000 - 500000

**Request:**
```http
POST /api/garageservice/paging
Content-Type: application/json

{
  "page": 1,
  "pageSize": 10,
  "columnFilters": [
    {
      "columnName": "GarageServicePrice",
      "operator": "greater_or_equal",
      "value": "100000"
    },
    {
      "columnName": "GarageServicePrice",
      "operator": "less_or_equal",
      "value": "500000"
    }
  ],
  "columnSorts": [
    {
      "columnName": "GarageServicePrice",
      "sortDirection": "ASC"
    }
  ]
}
```

---

### Ví dụ 5: Tìm dịch vụ chưa có giá

**Request:**
```http
POST /api/garageservice/paging
Content-Type: application/json

{
  "page": 1,
  "pageSize": 10,
  "columnFilters": [
    {
      "columnName": "GarageServicePrice",
      "operator": "empty",
      "value": null
    }
  ]
}
```

---

## ⚠️ Lưu ý

### Validation Rules

1. **Tên dịch vụ:**
   - Bắt buộc phải có
   - Tối đa 255 ký tự
   - Không được để trống

2. **Giá dịch vụ:**
   - Tùy chọn (có thể null)
   - Nếu có giá trị, phải >= 0
   - Kiểu dữ liệu: `decimal(24,2)`

### Error Handling

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
- `500 Internal Server Error`: Lỗi server

### Soft Delete

- Khi xóa Garage Service, hệ thống chỉ đánh dấu `is_deleted = 1`
- Dữ liệu vẫn tồn tại trong database
- Các API list và detail tự động loại trừ các record đã bị xóa

### Best Practices

1. **Phân trang:**
   - Nên sử dụng `pageSize` hợp lý (10-50 records/page)
   - Tránh `pageSize` quá lớn (> 100) để tối ưu performance

2. **Filter:**
   - Sử dụng `contains` cho tìm kiếm text
   - Sử dụng `equals` cho tìm kiếm chính xác
   - Kết hợp nhiều filter với toán tử AND

3. **Sort:**
   - Mặc định sort theo `GarageServiceId DESC` (mới nhất trước)
   - Có thể sort theo nhiều cột cùng lúc

4. **Validation:**
   - Luôn validate dữ liệu trước khi gửi request
   - Kiểm tra response `success` trước khi sử dụng `data`

---

## 🔗 Related Documentation

- [Service Ticket API Documentation](../README_SERVICE_TICKET.md)
- [Garage Service Entity](../../SWP.Core/Entities/GarageService.cs)

---

## 📞 Support

Nếu có vấn đề hoặc câu hỏi, vui lòng liên hệ team phát triển.

---

**Created by:** DuyLC  
**Last Updated:** 02/12/2025


