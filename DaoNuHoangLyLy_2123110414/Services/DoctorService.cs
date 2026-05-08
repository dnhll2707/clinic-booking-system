using DaoNuHoangLyLy_2123110414.Data;
using DaoNuHoangLyLy_2123110414.DTOs;
using DaoNuHoangLyLy_2123110414.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<DoctorResponseDTOs>> GetAllAsync(int? specialtyId = null, bool activeOnly = true)
        {
            var query = _context.DoctorProfiles
                .Include(x => x.User)
                .Include(x => x.Specialty)
                .AsQueryable();

            if (activeOnly)
                query = query.Where(x => x.IsActive);

            if (specialtyId.HasValue)
                query = query.Where(x => x.SpecialtyId == specialtyId.Value);

            var doctors = await query
                .OrderBy(x => x.FullName)
                .ToListAsync();

            return doctors.Select(MapToResponse).ToList();
        }

        public async Task<DoctorResponseDTOs?> GetByIdAsync(int id)
        {
            var doctor = await _context.DoctorProfiles
                .Include(x => x.User)
                .Include(x => x.Specialty)
                .FirstOrDefaultAsync(x => x.Id == id);

            return doctor == null ? null : MapToResponse(doctor);
        }

        public async Task<ServiceResult<DoctorResponseDTOs>> CreateAsync(CreateDoctorDTOs model)
        {
            var fullName = model.FullName.Trim();
            var email = model.Email.Trim();
            var phoneNumber = model.PhoneNumber?.Trim() ?? string.Empty;
            var description = NormalizeOptional(model.Description);

            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(x => x.Id == model.SpecialtyId && x.IsActive);

            if (specialty == null)
                return ServiceResult<DoctorResponseDTOs>.Fail("Vui lòng chọn chuyên khoa.");

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return ServiceResult<DoctorResponseDTOs>.Fail("Email đã tồn tại.");

            var duplicatePhone = await _context.DoctorProfiles
                .AnyAsync(x => x.PhoneNumber == phoneNumber);
            if (duplicatePhone)
                return ServiceResult<DoctorResponseDTOs>.Fail("Số điện thoại đã tồn tại.");

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                PhoneNumber = phoneNumber
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
                return ServiceResult<DoctorResponseDTOs>.Fail(ToVietnameseIdentityErrors(createResult));

            var roleResult = await _userManager.AddToRoleAsync(user, SystemRoles.Doctor);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return ServiceResult<DoctorResponseDTOs>.Fail(ToVietnameseIdentityErrors(roleResult));
            }

            var doctor = new DoctorProfile
            {
                UserId = user.Id,
                FullName = fullName,
                SpecialtyId = model.SpecialtyId,
                PhoneNumber = phoneNumber,
                Description = description,
                IsActive = true
            };

            _context.DoctorProfiles.Add(doctor);
            await _context.SaveChangesAsync();

            await _context.Entry(doctor).Reference(x => x.User).LoadAsync();
            await _context.Entry(doctor).Reference(x => x.Specialty).LoadAsync();

            return ServiceResult<DoctorResponseDTOs>.Ok(MapToResponse(doctor), "Tạo bác sĩ thành công.");
        }

        public async Task<ServiceResult<DoctorResponseDTOs>> UpdateAsync(int id, UpdateDoctorDTOs model)
        {
            var doctor = await _context.DoctorProfiles
                .Include(x => x.User)
                .Include(x => x.Specialty)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (doctor == null)
                return ServiceResult<DoctorResponseDTOs>.Fail("Không tìm thấy bác sĩ.");

            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(x => x.Id == model.SpecialtyId && x.IsActive);

            if (specialty == null)
                return ServiceResult<DoctorResponseDTOs>.Fail("Vui lòng chọn chuyên khoa.");

            var phoneNumber = model.PhoneNumber?.Trim() ?? string.Empty;
            var duplicatePhone = await _context.DoctorProfiles
                .AnyAsync(x => x.Id != id && x.PhoneNumber == phoneNumber);
            if (duplicatePhone)
                return ServiceResult<DoctorResponseDTOs>.Fail("Số điện thoại đã tồn tại.");

            var wasActive = doctor.IsActive;

            doctor.FullName = model.FullName.Trim();
            doctor.PhoneNumber = phoneNumber;
            doctor.SpecialtyId = model.SpecialtyId;
            doctor.Description = NormalizeOptional(model.Description);
            doctor.IsActive = model.IsActive;

            if (doctor.User != null)
            {
                doctor.User.PhoneNumber = phoneNumber;
                var updateUserResult = await _userManager.UpdateAsync(doctor.User);
                if (!updateUserResult.Succeeded)
                    return ServiceResult<DoctorResponseDTOs>.Fail(ToVietnameseIdentityErrors(updateUserResult));
            }

            if (wasActive && !doctor.IsActive)
                await CloseFutureAvailableSlotsAsync(doctor.Id);

            await _context.SaveChangesAsync();
            await _context.Entry(doctor).Reference(x => x.Specialty).LoadAsync();

            return ServiceResult<DoctorResponseDTOs>.Ok(MapToResponse(doctor), "Cập nhật bác sĩ thành công.");
        }

        public async Task<ServiceResult> DeactivateAsync(int id)
        {
            var doctor = await _context.DoctorProfiles
                .FirstOrDefaultAsync(x => x.Id == id);

            if (doctor == null)
                return ServiceResult.Fail("Không tìm thấy bác sĩ.");

            var hasActiveAppointments = await _context.Appointments.AnyAsync(x =>
                x.DoctorProfileId == id &&
                (x.AppointmentStatus == AppointmentStatuses.Pending ||
                 x.AppointmentStatus == AppointmentStatuses.Confirmed));

            if (hasActiveAppointments)
                return ServiceResult.Fail("Không thể khóa bác sĩ đang có lịch hẹn chờ xác nhận hoặc đã xác nhận.");

            doctor.IsActive = false;
            await CloseFutureAvailableSlotsAsync(id);
            await _context.SaveChangesAsync();

            return ServiceResult.Ok("Đã khóa bác sĩ và đóng các khung giờ còn trống trong tương lai.");
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var doctor = await _context.DoctorProfiles
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (doctor == null)
                return ServiceResult.Fail("Không tìm thấy bác sĩ.");

            var hasSchedules = await _context.DoctorSchedules.AnyAsync(x => x.DoctorProfileId == id);
            var hasTimeSlots = await _context.TimeSlots.AnyAsync(x => x.DoctorProfileId == id);
            var hasAppointments = await _context.Appointments.AnyAsync(x => x.DoctorProfileId == id);

            if (hasSchedules || hasTimeSlots || hasAppointments)
                return ServiceResult.Fail("Không thể xóa bác sĩ đã có lịch làm việc hoặc lịch hẹn.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            _context.DoctorProfiles.Remove(doctor);
            await _context.SaveChangesAsync();

            if (doctor.User != null)
            {
                var deleteUserResult = await _userManager.DeleteAsync(doctor.User);
                if (!deleteUserResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return ServiceResult.Fail(ToVietnameseIdentityErrors(deleteUserResult));
                }
            }

            await transaction.CommitAsync();

            return ServiceResult.Ok("Xóa bác sĩ thành công.");
        }

        private async Task CloseFutureAvailableSlotsAsync(int doctorId)
        {
            var today = DateTime.Today;
            var slots = await _context.TimeSlots
                .Where(x =>
                    x.DoctorProfileId == doctorId &&
                    x.SlotDate.Date >= today &&
                    x.SlotStatus == SlotStatuses.Available)
                .ToListAsync();

            foreach (var slot in slots)
            {
                slot.SlotStatus = SlotStatuses.Closed;
            }

            if (slots.Count > 0)
                await _context.SaveChangesAsync();
        }

        private static DoctorResponseDTOs MapToResponse(DoctorProfile doctor)
        {
            return new DoctorResponseDTOs
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                FullName = doctor.FullName,
                SpecialtyId = doctor.SpecialtyId,
                Email = doctor.User?.Email ?? string.Empty,
                PhoneNumber = doctor.PhoneNumber,
                Description = doctor.Description,
                IsActive = doctor.IsActive,
                CreatedAt = doctor.CreatedAt,
                Specialty = doctor.Specialty == null
                    ? null
                    : new DoctorSpecialtyResponseDTOs
                    {
                        Id = doctor.Specialty.Id,
                        Name = doctor.Specialty.Name,
                        Description = doctor.Specialty.Description,
                        IsActive = doctor.Specialty.IsActive
                    }
            };
        }

        private static string? NormalizeOptional(string? value)
        {
            var trimmed = value?.Trim();
            return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
        }

        private static string ToVietnameseIdentityErrors(IdentityResult result)
        {
            var messages = result.Errors.Select(error => error.Code switch
            {
                nameof(IdentityErrorDescriber.DuplicateEmail) => "Email đã tồn tại.",
                nameof(IdentityErrorDescriber.DuplicateUserName) => "Email đã tồn tại.",
                nameof(IdentityErrorDescriber.InvalidEmail) => "Email không đúng định dạng.",
                nameof(IdentityErrorDescriber.PasswordTooShort) => "Mật khẩu phải có ít nhất 6 ký tự.",
                _ => error.Description
            });

            return string.Join(" ", messages.Distinct());
        }
    }
}
