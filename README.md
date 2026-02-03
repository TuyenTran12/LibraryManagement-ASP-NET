# 📚 Library Management System (MyWeb)

Dự án website quản lý thư viện back-end được xây dựng bằng **ASP.NET Core MVC**. Ứng dụng cung cấp các tính năng quản lý sách toàn diện cùng hệ thống bảo mật người dùng hiện đại, nổi bật với phong cách giao diện **High Contrast UI**.

---

## ✨ Chức năng chính (Core Features)

### 👤 Quản lý người dùng (Authentication)
* **Đăng ký tài khoản mới:** Với đầy đủ các trường thông tin và kiểm tra hợp lệ.
* **Đăng nhập hệ thống:** Bảo mật và lưu phiên làm việc (Remember Me).
* **Xác thực Email:** Tính năng Verify Email giúp lấy lại quyền truy cập tài khoản.
* **Đổi mật khẩu:** Cho phép người dùng cập nhật mật khẩu mới một cách an toàn.

### 📖 Quản lý sách (Book Management)
* **Hệ thống CRUD:** Thực hiện đầy đủ các thao tác Thêm, Xem, Sửa, Xóa thông tin sách.
* **Thông tin chi tiết:** Quản lý Tiêu đề, Tác giả, Năm xuất bản và Thể loại sách.
* **Phân loại:** Liên kết sách với các danh mục (Categories) tương ứng.

### 🎨 UI/UX: High Contrast Style
* **Giao diện nền tối (Dark Mode):** Mang lại cảm giác hiện đại và giảm mỏi mắt.
* **Input Group tùy chỉnh:** Các ô nhập liệu được thiết kế nền trắng, viền đen dày (2px) và chữ đen, giúp tăng độ tương phản và khả năng tập trung khi nhập liệu.
* **Responsive Design:** Hiển thị tốt trên cả máy tính và thiết bị di động.

---

## 🛠 Công nghệ sử dụng (Tech Stack)

* **Backend:** C#, ASP.NET Core MVC.
* **Database:** SQL Server, Entity Framework Core.
* **Frontend:** HTML5, CSS3 (Custom Styles), Bootstrap 5, FontAwesome Icons.
* **Tools:** Visual Studio 2022, Git.

---

## 📸 Hình ảnh dự án (Screenshots)

Quản Lý Sách
 <img width="1920" height="1080" alt="Index" src="https://github.com/user-attachments/assets/bb4d02c8-16a5-47eb-8c18-cb9cd96cc91b" />
 
Trang Đăng Nhập
 <img width="1920" height="1080" alt="Login" src="https://github.com/user-attachments/assets/bf8b3f76-d16c-4cfc-8d83-eb582c639901" />
 
Form Nhập Liệu
<img width="1920" height="1080" alt="Create" src="https://github.com/user-attachments/assets/87129b44-5425-460a-97a6-b90ca4a62721" />
 


---

## ⚙️ Hướng dẫn cài đặt

1.  **Clone dự án:**
    ```bash
    git clone [https://github.com/TuyenTran12/LibraryManagement-ASP-NET.git](https://github.com/TuyenTran12/LibraryManagement-ASP-NET.git)
    ```
2.  **Cấu hình Database:** Thay đổi chuỗi kết nối (Connection String) trong file `appsettings.json` cho phù hợp với máy của bạn.
3.  **Migration:** Mở Package Manager Console và chạy:
    ```powershell
    Update-Database
    ```
4.  **Run:** Bấm `F5` hoặc `Ctrl + F5` trong Visual Studio để khởi chạy.

---

## 📧 Liên hệ
* **Họ tên:** Trần Thế Tuyên
* **Email:** tranthetuyen221202@gmail.com

---
*Dự án được xây dựng với mục đích học tập và làm quen với hệ sinh thái ASP.NET Core.*
