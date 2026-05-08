# Clinic Booking API

Chuong trinh quan ly dat lich kham benh bang ASP.NET Core Web API, Entity Framework Core, SQL Server, Identity va JWT.

## Chuc nang chinh

- Xac thuc: dang ky benh nhan, dang nhap JWT, phan quyen Admin/Doctor/Patient.
- Chuyen khoa: xem, them, sua, khoa chuyen khoa.
- Bac si: xem danh sach, xem chi tiet, them, cap nhat, khoa bac si.
- Benh nhan: xem va cap nhat ho so ca nhan.
- Lich lam viec: Admin tao lich, he thong tu sinh khung gio, xem/khoa lich.
- Khung gio: xem theo lich, xem slot trong theo bac si va ngay, dong/mo lai slot.
- Dat lich: benh nhan dat/huy lich; Admin xac nhan/huy; bac si/Admin hoan tat hoac danh dau vang mat.

## Tai khoan mac dinh

Khi chay ung dung, he thong tu tao role, tai khoan admin va mot so chuyen khoa mau.

- Email: `admin@clinic.com`
- Mat khau: `123456`

## Cach chay

1. Kiem tra connection string trong `DaoNuHoangLyLy_2123110414/appsettings.json`.
2. Chay backend:

```powershell
dotnet run --project .\DaoNuHoangLyLy_2123110414\DaoNuHoangLyLy_2123110414.csproj
```

3. Mo backend landing page hoac Swagger:

```text
https://localhost:7059
https://localhost:7111/swagger
http://localhost:5117/swagger
```

Neu chay bang profile `http`, frontend mac dinh goi API o `http://localhost:5117`.
Neu chay backend bang profile `https`, tao file `ClinicBookingFrontend/.env` va doi:

```text
VITE_API_BASE_URL=https://localhost:7059
```

4. Chay frontend React:

```powershell
cd .\ClinicBookingFrontend
npm.cmd install
npm.cmd run dev
```

5. Mo frontend:

```text
http://localhost:5173
```

## Luong nghiep vu goi y

1. Dang nhap Admin bang `POST /api/Auth/login`.
2. Tao chuyen khoa neu can bang `POST /api/Specialty`.
3. Tao bac si bang `POST /api/Doctor`.
4. Tao lich lam viec bang `POST /api/Schedule`; he thong tu sinh cac `TimeSlot`.
5. Benh nhan dang ky va dang nhap.
6. Benh nhan xem slot trong bang `GET /api/Schedule/doctor/{doctorId}/available-slots?date=yyyy-MM-dd`.
7. Benh nhan dat lich bang `POST /api/Appointment/book`.
8. Admin xac nhan lich bang `PATCH /api/Appointment/{appointmentId}/confirm`.
9. Bac si hoan tat lich bang `PATCH /api/Appointment/{appointmentId}/complete`.

## Test API nhanh bang file .http

Mo file `DaoNuHoangLyLy_2123110414/DaoNuHoangLyLy_2123110414.http` trong Visual Studio hoac VS Code.

- Chay request `Login Admin`, copy `data.token` dan vao bien `@adminToken`.
- Tao bac si/lich/slot, copy cac `id` tra ve dan vao `@doctorId`, `@scheduleId`, `@slotId`.
- Dang ky va login Patient, copy token vao `@patientToken`.
- Login Doctor vua tao, copy token vao `@doctorToken`.
- Chay cac request Appointment theo thu tu: Book -> Confirm -> Complete/NoShow/Cancel.

## Ghi chu nghiep vu

- Khong cho tao lich hoac dat slot trong qua khu.
- Khong cho dat slot cua bac si/lich lam viec da khoa.
- Khi huy lich, slot duoc chuyen sang `Cancelled` de tranh hien thi la con trong trong khi da co lich su dat lich.
- Khong cho khoa bac si hoac lich lam viec neu con lich hen `Pending` hoac `Confirmed`.
