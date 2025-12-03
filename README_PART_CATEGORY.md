# API Hướng Dẫn Sử Dụng - Part Category (Danh Mục Phụ Tùng)

## Tổng Quan

API Part Category cung cấp các chức năng CRUD (Create, Read, Update, Delete) cho quản lý danh mục phụ tùng trong hệ thống Garage Management System. API hỗ trợ phân trang, lọc, sắp xếp và kiểm tra mã danh mục trùng lặp.

## Base URL

```
http://localhost:5000/api/partcategory
```

## Các API Endpoints

### 1. Lấy Tất Cả Part Category (Cho Select)

**Endpoint:** `GET /api/partcategory/all`

**Mô tả:** Lấy tất cả danh mục phụ tùng (dùng cho dropdown/select).

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "partCategoryId": 1,
      "partCategoryName": "Lốp và bánh xe",
      "partCategoryCode": "DM001"
    },
    {
      "partCategoryId": 2,
      "partCategoryName": "Động cơ",
      "partCategoryCode": "DM002"
    }
  ],
  "message": "Lấy danh sách part category thành công"
}
```

---

### 2. Lấy Danh Sách Part Category (Phân Trang)

**Endpoint:** `POST /api/partcategory/paging`

**Mô tả:** Lấy danh sách danh mục phụ tùng có phân trang, lọc và sắp xếp.

**Request Body:**
```json
{
  "page": 1,
  "pageSize": 10,
  "columnFilters": [
    {
      "columnName": "PartCategoryName",
      "operator": "contains",
      "value": "lốp"
    },
    {
      "columnName": "Status",
      "operator": "equals",
      "value": "Active"
    }
  ],
  "columnSorts": [
    {
      "columnName": "PartCategoryName",
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
        "partCategoryId": 1,
        "partCategoryName": "Lốp và bánh xe",
        "partCategoryCode": "DM001",
        "partCategoryDiscription": "Danh mục lốp và bánh xe",
        "partCategoryPhone": "0123456789",
        "status": "Active"
      }
    ],
    "total": 50,
    "page": 1,
    "pageSize": 10
  },
  "message": "Lấy danh sách part category thành công"
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
- `PartCategoryName` / `part_category_name`: Tên danh mục
- `PartCategoryCode` / `part_category_code`: Mã danh mục
- `PartCategoryDiscription` / `part_category_discription`: Mô tả
- `PartCategoryPhone` / `part_category_phone`: Số điện thoại
- `Status` / `status`: Trạng thái

---

### 3. Lấy Chi Tiết Part Category

**Endpoint:** `GET /api/partcategory/{id}`

**Mô tả:** Lấy thông tin chi tiết của một danh mục phụ tùng theo ID.

**Path Parameters:**
- `id` (int): ID của danh mục phụ tùng

**Response:**
```json
{
  "success": true,
  "data": {
    "partCategoryId": 1,
    "partCategoryName": "Lốp và bánh xe",
    "partCategoryCode": "DM001",
    "partCategoryDiscription": "Danh mục lốp và bánh xe",
    "partCategoryPhone": "0123456789",
    "status": "Active"
  },
  "message": "Lấy chi tiết part category thành công"
}
```

---

### 4. Tạo Mới Part Category

**Endpoint:** `POST /api/partcategory`

**Mô tả:** Tạo mới một danh mục phụ tùng.

**Request Body:**
```json
{
  "partCategoryName": "Lốp và bánh xe",
  "partCategoryCode": "DM001",
  "partCategoryDiscription": "Danh mục lốp và bánh xe",
  "partCategoryPhone": "0123456789",
  "status": "Active"
}
```

**Validation:**
- `partCategoryCode`: Bắt buộc, tối đa 20 ký tự, phải duy nhất
- `partCategoryName`: Tùy chọn, tối đa 100 ký tự
- `partCategoryDiscription`: Tùy chọn, tối đa 255 ký tự
- `partCategoryPhone`: Tùy chọn, tối đa 50 ký tự
- `status`: Tùy chọn, tối đa 20 ký tự

**Response (Success):**
```json
{
  "success": true,
  "data": {
    "partCategoryId": 1
  },
  "message": "Tạo part category thành công"
}
```

**Response (Error - Mã trùng):**
```json
{
  "success": false,
  "data": null,
  "message": "Mã danh mục đã tồn tại."
}
```

---

### 5. Cập Nhật Part Category

**Endpoint:** `PUT /api/partcategory/{id}`

**Mô tả:** Cập nhật thông tin của một danh mục phụ tùng.

**Path Parameters:**
- `id` (int): ID của danh mục phụ tùng cần cập nhật

**Request Body:**
```json
{
  "partCategoryName": "Lốp và bánh xe cao cấp",
  "partCategoryCode": "DM001",
  "partCategoryDiscription": "Danh mục lốp và bánh xe cao cấp",
  "partCategoryPhone": "0987654321",
  "status": "Active"
}
```

**Validation:**
- Tương tự như Create, nhưng `partCategoryCode` có thể trùng với chính record đang cập nhật (excludeId)

**Response (Success):**
```json
{
  "success": true,
  "data": {
    "affectedRows": 1
  },
  "message": "Cập nhật part category thành công"
}
```

**Response (Error - Không tìm thấy):**
```json
{
  "success": false,
  "data": null,
  "message": "Không tìm thấy part category cần cập nhật."
}
```

---

### 6. Xóa Part Category

**Endpoint:** `DELETE /api/partcategory/{id}`

**Mô tả:** Xóa mềm (soft delete) một danh mục phụ tùng.

**Path Parameters:**
- `id` (int): ID của danh mục phụ tùng cần xóa

**Response (Success):**
```json
{
  "success": true,
  "data": {
    "affectedRows": 1
  },
  "message": "Xóa part category thành công"
}
```

**Response (Error - Không tìm thấy):**
```json
{
  "success": false,
  "data": null,
  "message": "Không tìm thấy part category."
}
```

**Lưu ý:** 
- Xóa mềm (soft delete) - chỉ đánh dấu `is_deleted = 1`, không xóa thực sự khỏi database.
- Nên kiểm tra xem danh mục có đang được sử dụng trong Part không trước khi xóa.

---

### 7. Kiểm Tra Mã Part Category Trùng

**Endpoint:** `GET /api/partcategory/check-code`

**Mô tả:** Kiểm tra xem mã danh mục phụ tùng đã tồn tại hay chưa.

**Query Parameters:**
- `partCategoryCode` (string, required): Mã danh mục cần kiểm tra
- `excludeId` (int, optional): ID của danh mục hiện tại (dùng khi update để loại trừ chính record đang sửa)

**Example Request:**
```
GET /api/partcategory/check-code?partCategoryCode=DM001
GET /api/partcategory/check-code?partCategoryCode=DM001&excludeId=1
```

**Response:**
```json
{
  "success": true,
  "data": {
    "exists": true
  },
  "message": "Kiểm tra mã part category thành công"
}
```

---

## Ví Dụ Sử Dụng

### Ví Dụ 1: Lấy danh sách cho dropdown

**Request:**
```
GET /api/partcategory/all
```

**Use Case:** Dùng để populate dropdown/select khi tạo/cập nhật Part.

---

### Ví Dụ 2: Tìm kiếm danh mục theo tên

**Request:**
```json
POST /api/partcategory/paging
{
  "page": 1,
  "pageSize": 20,
  "columnFilters": [
    {
      "columnName": "PartCategoryName",
      "operator": "contains",
      "value": "lốp"
    }
  ],
  "columnSorts": [
    {
      "columnName": "PartCategoryName",
      "sortDirection": "ASC"
    }
  ]
}
```

---

### Ví Dụ 3: Lọc danh mục theo trạng thái

**Request:**
```json
POST /api/partcategory/paging
{
  "page": 1,
  "pageSize": 10,
  "columnFilters": [
    {
      "columnName": "Status",
      "operator": "equals",
      "value": "Active"
    }
  ]
}
```

---

## Cấu Trúc Dữ Liệu

### PartCategorySelectDto (Cho Select)
```typescript
{
  partCategoryId: number;
  partCategoryName?: string;
  partCategoryCode: string;
}
```

### PartCategoryListItemDto
```typescript
{
  partCategoryId: number;
  partCategoryName?: string;
  partCategoryCode?: string;
  partCategoryDiscription?: string;
  partCategoryPhone?: string;
  status?: string;
}
```

### PartCategoryDetailDto
```typescript
{
  partCategoryId: number;
  partCategoryName?: string;
  partCategoryCode?: string;
  partCategoryDiscription?: string;
  partCategoryPhone?: string;
  status?: string;
}
```

### PartCategoryCreateDto / PartCategoryUpdateDto
```typescript
{
  partCategoryName?: string;          // Optional, max 100 chars
  partCategoryCode: string;            // Required, max 20 chars, unique
  partCategoryDiscription?: string;   // Optional, max 255 chars
  partCategoryPhone?: string;         // Optional, max 50 chars
  status?: string;                    // Optional, max 20 chars
}
```

---

## Lưu Ý

1. **Mã Danh Mục (PartCategoryCode):**
   - Phải duy nhất trong hệ thống
   - Tối đa 20 ký tự
   - Không được để trống

2. **Tên Danh Mục (PartCategoryName):**
   - Tùy chọn (có thể null)
   - Tối đa 100 ký tự

3. **Trạng Thái (Status):**
   - Tùy chọn (có thể null)
   - Tối đa 20 ký tự
   - Thường dùng: "Active", "Inactive", etc.

4. **Xóa Danh Mục:**
   - Nên kiểm tra xem danh mục có đang được sử dụng trong Part không
   - Xóa mềm (soft delete) - chỉ đánh dấu `is_deleted = 1`

5. **Phân Trang:**
   - `page`: Bắt đầu từ 1
   - `pageSize`: Số bản ghi trên mỗi trang (mặc định: 10)
   - `total`: Tổng số bản ghi thỏa mãn điều kiện filter

6. **API GetAll:**
   - Trả về tất cả danh mục (không phân trang)
   - Sắp xếp theo tên (ASC)
   - Dùng cho dropdown/select trong form

---

## Error Codes

- **400 Bad Request:** Dữ liệu đầu vào không hợp lệ (validation error)
- **404 Not Found:** Không tìm thấy PartCategory
- **409 Conflict:** Mã danh mục đã tồn tại
- **500 Internal Server Error:** Lỗi hệ thống

---

## Created By

DuyLC - 03/12/2025


