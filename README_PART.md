# API Hướng Dẫn Sử Dụng - Part (Phụ Tùng)

## Tổng Quan

API Part cung cấp các chức năng CRUD (Create, Read, Update, Delete) cho quản lý phụ tùng trong hệ thống Garage Management System. API hỗ trợ phân trang, lọc, sắp xếp và kiểm tra mã phụ tùng trùng lặp.

## Base URL

```
http://localhost:5000/api/part
```

## Các API Endpoints

### 1. Lấy Tất Cả Part (Cho Select)

**Endpoint:** `GET /api/part/all`

**Mô tả:** Lấy tất cả phụ tùng (dùng cho dropdown/select).

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "partId": 1,
      "partName": "Lốp xe",
      "partCode": "LP001",
      "partQuantity": 50,
      "partUnit": "Cái"
    },
    {
      "partId": 2,
      "partName": "Bugi",
      "partCode": "BG001",
      "partQuantity": 100,
      "partUnit": "Cái"
    }
  ],
  "message": "Lấy danh sách part thành công"
}
```

**Lưu ý:** 
- API này trả về tất cả parts (không phân trang)
- Sắp xếp theo tên (ASC)
- Chỉ lấy các part chưa bị xóa (`is_deleted = 0`)
- Dùng cho dropdown/select trong form

---

### 2. Lấy Danh Sách Part (Phân Trang)

**Endpoint:** `POST /api/part/paging`

**Mô tả:** Lấy danh sách phụ tùng có phân trang, lọc và sắp xếp.

**Request Body:**
```json
{
  "page": 1,
  "pageSize": 10,
  "columnFilters": [
    {
      "columnName": "PartName",
      "operator": "contains",
      "value": "lốp"
    },
    {
      "columnName": "PartQuantity",
      "operator": "greater_or_equal",
      "value": 10
    },
    {
      "columnName": "PartCategoryId",
      "operator": "equals",
      "value": 1
    }
  ],
  "columnSorts": [
    {
      "columnName": "PartName",
      "sortDirection": "ASC"
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
        "partId": 1,
        "partName": "Lốp xe",
        "partCode": "LP001",
        "partQuantity": 50,
        "partUnit": "Cái",
        "partCategoryId": 1,
        "partCategoryName": "Lốp và bánh xe",
        "partCategoryCode": "DM001",
        "partPrice": 500000.00,
        "warrantyMonth": 12
      }
    ],
    "total": 100,
    "page": 1,
    "pageSize": 10
  },
  "message": "Lấy danh sách part thành công"
}
```

**Các Operators Hỗ Trợ:**
- `equals`: Bằng
- `not_equals`: Không bằng
- `contains`: Chứa
- `not_contains`: Không chứa
- `starts_with`: Bắt đầu bằng
- `ends_with`: Kết thúc bằng
- `empty`: Rỗng
- `not_empty`: Không rỗng
- `greater_than`: Lớn hơn
- `less_than`: Nhỏ hơn
- `greater_or_equal`: Lớn hơn hoặc bằng
- `less_or_equal`: Nhỏ hơn hoặc bằng

**Các Cột Có Thể Filter/Sort:**
- `PartName` / `part_name`: Tên phụ tùng
- `PartCode` / `part_code`: Mã phụ tùng
- `PartQuantity` / `part_quantity`: Số lượng tồn kho
- `PartUnit` / `part_unit`: Đơn vị tính
- `PartCategoryId` / `part_category_id`: ID danh mục phụ tùng
- `PartCategoryName` / `part_category_name`: Tên danh mục phụ tùng
- `PartCategoryCode` / `part_category_code`: Mã danh mục phụ tùng
- `PartPrice` / `part_price`: Giá phụ tùng
- `WarrantyMonth` / `warranty_month`: Thời gian bảo hành (tháng)

---

### 3. Lấy Chi Tiết Part

**Endpoint:** `GET /api/part/{id}`

**Mô tả:** Lấy thông tin chi tiết của một phụ tùng theo ID.

**Path Parameters:**
- `id` (int): ID của phụ tùng

**Response:**
```json
{
  "success": true,
  "data": {
    "partId": 1,
    "partName": "Lốp xe",
    "partCode": "LP001",
    "partQuantity": 50,
    "partUnit": "Cái",
    "partCategoryId": 1,
    "partCategoryName": "Lốp và bánh xe",
    "partCategoryCode": "DM001",
    "partCategoryDiscription": "Danh mục lốp và bánh xe",
    "partCategoryPhone": "0123456789",
    "status": "Active",
    "partPrice": 500000.00,
    "warrantyMonth": 12
  },
  "message": "Lấy chi tiết part thành công"
}
```

---

### 4. Tạo Mới Part

**Endpoint:** `POST /api/part`

**Mô tả:** Tạo mới một phụ tùng.

**Request Body:**
```json
{
  "partName": "Lốp xe",
  "partCode": "LP001",
  "partQuantity": 50,
  "partUnit": "Cái",
  "partCategoryId": 1,
  "partPrice": 500000.00,
  "warrantyMonth": 12
}
```

**Validation:**
- `partName`: Bắt buộc, tối đa 100 ký tự
- `partCode`: Bắt buộc, tối đa 20 ký tự, phải duy nhất
- `partQuantity`: Bắt buộc, >= 0
- `partUnit`: Bắt buộc, tối đa 20 ký tự
- `partCategoryId`: Bắt buộc, phải tồn tại trong bảng PartCategory
- `partPrice`: Tùy chọn, >= 0
- `warrantyMonth`: Tùy chọn, >= 0

**Response (Success):**
```json
{
  "success": true,
  "data": {
    "partId": 1
  },
  "message": "Tạo part thành công"
}
```

**Response (Error - Mã trùng):**
```json
{
  "success": false,
  "data": null,
  "message": "Mã phụ tùng đã tồn tại."
}
```

**Response (Error - PartCategory không tồn tại):**
```json
{
  "success": false,
  "data": null,
  "message": "Không tìm thấy danh mục phụ tùng."
}
```

---

### 5. Cập Nhật Part

**Endpoint:** `PUT /api/part/{id}`

**Mô tả:** Cập nhật thông tin của một phụ tùng.

**Path Parameters:**
- `id` (int): ID của phụ tùng cần cập nhật

**Request Body:**
```json
{
  "partName": "Lốp xe cao cấp",
  "partCode": "LP001",
  "partQuantity": 60,
  "partUnit": "Cái",
  "partCategoryId": 1,
  "partPrice": 600000.00,
  "warrantyMonth": 18
}
```

**Validation:**
- Tương tự như Create, nhưng `partCode` có thể trùng với chính record đang cập nhật (excludeId)

**Response (Success):**
```json
{
  "success": true,
  "data": {
    "affectedRows": 1
  },
  "message": "Cập nhật part thành công"
}
```

**Response (Error - Không tìm thấy):**
```json
{
  "success": false,
  "data": null,
  "message": "Không tìm thấy part cần cập nhật."
}
```

---

### 6. Xóa Part

**Endpoint:** `DELETE /api/part/{id}`

**Mô tả:** Xóa mềm (soft delete) một phụ tùng.

**Path Parameters:**
- `id` (int): ID của phụ tùng cần xóa

**Response (Success):**
```json
{
  "success": true,
  "data": {
    "affectedRows": 1
  },
  "message": "Xóa part thành công"
}
```

**Response (Error - Không tìm thấy):**
```json
{
  "success": false,
  "data": null,
  "message": "Không tìm thấy part."
}
```

**Lưu ý:** 
- Xóa mềm (soft delete) - chỉ đánh dấu `is_deleted = 1`, không xóa thực sự khỏi database.

---

### 7. Kiểm Tra Mã Part Trùng

**Endpoint:** `GET /api/part/check-code`

**Mô tả:** Kiểm tra xem mã phụ tùng đã tồn tại hay chưa.

**Query Parameters:**
- `partCode` (string, required): Mã phụ tùng cần kiểm tra
- `excludeId` (int, optional): ID của phụ tùng hiện tại (dùng khi update để loại trừ chính record đang sửa)

**Example Request:**
```
GET /api/part/check-code?partCode=LP001
GET /api/part/check-code?partCode=LP001&excludeId=1
```

**Response:**
```json
{
  "success": true,
  "data": {
    "exists": true
  },
  "message": "Kiểm tra mã part thành công"
}
```

---

## Ví Dụ Sử Dụng

### Ví Dụ 1: Tìm kiếm phụ tùng theo tên và sắp xếp

**Request:**
```json
POST /api/part/paging
{
  "page": 1,
  "pageSize": 20,
  "columnFilters": [
    {
      "columnName": "PartName",
      "operator": "contains",
      "value": "lốp"
    }
  ],
  "columnSorts": [
    {
      "columnName": "PartQuantity",
      "sortDirection": "DESC"
    }
  ]
}
```

### Ví Dụ 2: Lọc phụ tùng có tồn kho thấp

**Request:**
```json
POST /api/part/paging
{
  "page": 1,
  "pageSize": 10,
  "columnFilters": [
    {
      "columnName": "PartQuantity",
      "operator": "less_than",
      "value": 10
    }
  ]
}
```

### Ví Dụ 3: Lọc phụ tùng theo danh mục

**Request:**
```json
POST /api/part/paging
{
  "page": 1,
  "pageSize": 10,
  "columnFilters": [
    {
      "columnName": "PartCategoryId",
      "operator": "equals",
      "value": 1
    }
  ]
}
```

### Ví Dụ 4: Lọc phụ tùng theo giá

**Request:**
```json
POST /api/part/paging
{
  "page": 1,
  "pageSize": 10,
  "columnFilters": [
    {
      "columnName": "PartPrice",
      "operator": "greater_than",
      "value": "1000000"
    }
  ],
  "columnSorts": [
    {
      "columnName": "PartPrice",
      "sortDirection": "ASC"
    }
  ]
}
```

---

## Cấu Trúc Dữ Liệu

### PartListItemDto
```typescript
{
  partId: number;
  partName: string;
  partCode: string;
  partQuantity: number;
  partUnit: string;
  partCategoryId: number;
  partCategoryName: string;
  partCategoryCode: string;
  partPrice?: number;
  warrantyMonth?: number;
}
```

### PartDetailDto
```typescript
{
  partId: number;
  partName: string;
  partCode: string;
  partQuantity: number;
  partUnit: string;
  partCategoryId: number;
  partCategoryName: string;
  partCategoryCode: string;
  partCategoryDiscription?: string;
  partCategoryPhone?: string;
  status?: string;
  partPrice?: number;
  warrantyMonth?: number;
}
```

### PartCreateDto / PartUpdateDto
```typescript
{
  partName: string;          // Required, max 100 chars
  partCode: string;          // Required, max 20 chars, unique
  partQuantity: number;      // Required, >= 0
  partUnit: string;          // Required, max 20 chars
  partCategoryId: number;    // Required, must exist in PartCategory table
  partPrice?: number;        // Optional, >= 0
  warrantyMonth?: number;    // Optional, >= 0
}
```

---

## Lưu Ý

1. **Mã Phụ Tùng (PartCode):**
   - Phải duy nhất trong hệ thống
   - Tối đa 20 ký tự
   - Không được để trống

2. **Danh Mục Phụ Tùng (PartCategoryId):**
   - Phải tồn tại trong bảng PartCategory trước khi tạo/cập nhật Part
   - Nếu không tồn tại, API sẽ trả về lỗi validation

3. **Tồn Kho (PartQuantity):**
   - Phải >= 0
   - Có thể cập nhật khi nhập/xuất kho
   - Khi thêm part vào Service Ticket và confirm task, `partQuantity` sẽ tự động bị trừ

4. **Giá Phụ Tùng (PartPrice):**
   - Tùy chọn (có thể null)
   - Nếu có giá trị, phải >= 0
   - Được lưu trữ với độ chính xác decimal(24,2)

5. **Thời Gian Bảo Hành (WarrantyMonth):**
   - Tùy chọn (có thể null)
   - Nếu có giá trị, phải >= 0
   - Đơn vị: tháng

6. **Soft Delete:**
   - Xóa Part chỉ đánh dấu `is_deleted = 1`
   - Dữ liệu vẫn tồn tại trong database
   - Các API list/get sẽ tự động loại trừ các record đã xóa

7. **Phân Trang:**
   - `page`: Bắt đầu từ 1
   - `pageSize`: Số bản ghi trên mỗi trang (mặc định: 10)
   - `total`: Tổng số bản ghi thỏa mãn điều kiện filter

8. **Tích Hợp với Service Ticket:**
   - Khi thêm part vào Service Ticket: Chỉ validate số lượng, không trừ ngay
   - Khi Mechanic confirm task: Tự động trừ `partQuantity` cho tất cả parts trong service ticket
   - Khi xóa/update detail: Rollback `partQuantity` nếu service ticket đã completed

---

## Error Codes

- **400 Bad Request:** Dữ liệu đầu vào không hợp lệ (validation error)
- **404 Not Found:** Không tìm thấy Part/PartCategory
- **409 Conflict:** Mã phụ tùng đã tồn tại
- **500 Internal Server Error:** Lỗi hệ thống

---

## Created By

DuyLC - 02/12/2025
