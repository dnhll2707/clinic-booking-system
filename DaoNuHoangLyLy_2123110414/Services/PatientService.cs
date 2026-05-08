using DaoNuHoangLyLy_2123110414.Data;
using DaoNuHoangLyLy_2123110414.DTOs;
using DaoNuHoangLyLy_2123110414.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public class PatientService : IPatientService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<PatientProfileResponseDTOs>> GetAllAsync(bool activeOnly = true)
        {
            var profiles = await _context.PatientProfiles
                .Include(x => x.User)
                .Include(x => x.Appointments)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var data = profiles
                .Where(x => x.User != null)
                .Select(x => MapToResponse(x, x.User!))
                .ToList();

            return activeOnly ? data.Where(x => x.IsActive).ToList() : data;
        }

        public async Task<ServiceResult<PatientProfileResponseDTOs>> GetByIdAsync(int id)
        {
            var profile = await _context.PatientProfiles
                .Include(x => x.User)
                .Include(x => x.Appointments)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (profile?.User == null)
                return ServiceResult<PatientProfileResponseDTOs>.Fail("Không tìm thấy bệnh nhân.");

            return ServiceResult<PatientProfileResponseDTOs>.Ok(MapToResponse(profile, profile.User));
        }

        public async Task<ServiceResult<PatientProfileResponseDTOs>> CreateAsync(CreatePatientDTOs model)
        {
            if (model.DateOfBirth.HasValue && model.DateOfBirth.Value.Date > DateTime.Today)
                return ServiceResult<PatientProfileResponseDTOs>.Fail("Ngày sinh không được lớn hơn ngày hiện tại.");

            var email = model.Email.Trim();
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return ServiceResult<PatientProfileResponseDTOs>.Fail("Email đã tồn tại.");

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                PhoneNumber = NormalizeOptional(model.PhoneNumber),
                LockoutEnabled = true
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
                return ServiceResult<PatientProfileResponseDTOs>.Fail(ToErrorMessage(createResult));

            var roleResult = await _userManager.AddToRoleAsync(user, SystemRoles.Patient);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return ServiceResult<PatientProfileResponseDTOs>.Fail(ToErrorMessage(roleResult));
            }

            var profile = new PatientProfile
            {
                UserId = user.Id,
                FullName = model.FullName.Trim(),
                DateOfBirth = model.DateOfBirth,
                Gender = NormalizeOptional(model.Gender),
                Address = NormalizeOptional(model.Address)
            };

            _context.PatientProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return ServiceResult<PatientProfileResponseDTOs>.Ok(
                MapToResponse(profile, user),
                "Tạo bệnh nhân thành công.");
        }

        public async Task<ServiceResult<PatientProfileResponseDTOs>> GetCurrentAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResult<PatientProfileResponseDTOs>.Fail("Không tìm thấy tài khoản.");

            var profile = await _context.PatientProfiles
                .Include(x => x.Appointments)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (profile == null)
                return ServiceResult<PatientProfileResponseDTOs>.Fail("Không tìm thấy hồ sơ bệnh nhân.");

            return ServiceResult<PatientProfileResponseDTOs>.Ok(MapToResponse(profile, user));
        }

        public async Task<ServiceResult<PatientProfileResponseDTOs>> UpdateCurrentAsync(string userId, UpdatePatientProfileDTOs model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResult<PatientProfileResponseDTOs>.Fail("Không tìm thấy tài khoản.");

            var profile = await _context.PatientProfiles
                .Include(x => x.Appointments)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (profile == null)
                return ServiceResult<PatientProfileResponseDTOs>.Fail("Không tìm thấy hồ sơ bệnh nhân.");

            if (model.DateOfBirth.HasValue && model.DateOfBirth.Value.Date > DateTime.Today)
                return ServiceResult<PatientProfileResponseDTOs>.Fail("Ngày sinh không được lớn hơn ngày hiện tại.");

            profile.FullName = model.FullName.Trim();
            profile.DateOfBirth = model.DateOfBirth;
            profile.Gender = NormalizeOptional(model.Gender);
            profile.Address = NormalizeOptional(model.Address);
            user.PhoneNumber = NormalizeOptional(model.PhoneNumber);

            var updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
                return ServiceResult<PatientProfileResponseDTOs>.Fail(ToErrorMessage(updateUserResult));

            await _context.SaveChangesAsync();

            return ServiceResult<PatientProfileResponseDTOs>.Ok(
                MapToResponse(profile, user),
                "Cập nhật hồ sơ bệnh nhân thành công.");
        }

        public async Task<ServiceResult<PatientProfileResponseDTOs>> UpdateByIdAsync(int id, UpdatePatientDTOs model)
        {
            var profile = await _context.PatientProfiles
                .Include(x => x.User)
                .Include(x => x.Appointments)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (profile?.User == null)
                return ServiceResult<PatientProfileResponseDTOs>.Fail("Không tìm thấy bệnh nhân.");

            if (model.DateOfBirth.HasValue && model.DateOfBirth.Value.Date > DateTime.Today)
                return ServiceResult<PatientProfileResponseDTOs>.Fail("Ngày sinh không được lớn hơn ngày hiện tại.");

            var email = model.Email.Trim();
            var emailOwner = await _userManager.FindByEmailAsync(email);
            if (emailOwner != null && emailOwner.Id != profile.User.Id)
                return ServiceResult<PatientProfileResponseDTOs>.Fail("Email đã tồn tại.");

            profile.FullName = model.FullName.Trim();
            profile.DateOfBirth = model.DateOfBirth;
            profile.Gender = NormalizeOptional(model.Gender);
            profile.Address = NormalizeOptional(model.Address);

            profile.User.Email = email;
            profile.User.UserName = email;
            profile.User.NormalizedEmail = _userManager.NormalizeEmail(email);
            profile.User.NormalizedUserName = _userManager.NormalizeName(email);
            profile.User.PhoneNumber = NormalizeOptional(model.PhoneNumber);
            profile.User.LockoutEnabled = true;
            profile.User.LockoutEnd = model.IsActive ? null : DateTimeOffset.MaxValue;

            var updateUserResult = await _userManager.UpdateAsync(profile.User);
            if (!updateUserResult.Succeeded)
                return ServiceResult<PatientProfileResponseDTOs>.Fail(ToErrorMessage(updateUserResult));

            await _context.SaveChangesAsync();

            return ServiceResult<PatientProfileResponseDTOs>.Ok(
                MapToResponse(profile, profile.User),
                "Cập nhật bệnh nhân thành công.");
        }

        public async Task<ServiceResult> DeactivateAsync(int id)
        {
            var profile = await _context.PatientProfiles
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (profile?.User == null)
                return ServiceResult.Fail("Không tìm thấy bệnh nhân.");

            profile.User.LockoutEnabled = true;
            profile.User.LockoutEnd = DateTimeOffset.MaxValue;

            var result = await _userManager.UpdateAsync(profile.User);
            if (!result.Succeeded)
                return ServiceResult.Fail(ToErrorMessage(result));

            return ServiceResult.Ok("Đã vô hiệu hóa bệnh nhân.");
        }

        public async Task<ServiceResult> ActivateAsync(int id)
        {
            var profile = await _context.PatientProfiles
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (profile?.User == null)
                return ServiceResult.Fail("Không tìm thấy bệnh nhân.");

            profile.User.LockoutEnabled = true;
            profile.User.LockoutEnd = null;

            var result = await _userManager.UpdateAsync(profile.User);
            if (!result.Succeeded)
                return ServiceResult.Fail(ToErrorMessage(result));

            return ServiceResult.Ok("Đã kích hoạt lại bệnh nhân.");
        }

        public async Task<ServiceResult> DeactivateCurrentAsync(string userId)
        {
            var profile = await _context.PatientProfiles
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (profile?.User == null)
                return ServiceResult.Fail("Không tìm thấy bệnh nhân.");

            profile.User.LockoutEnabled = true;
            profile.User.LockoutEnd = DateTimeOffset.MaxValue;

            var result = await _userManager.UpdateAsync(profile.User);
            if (!result.Succeeded)
                return ServiceResult.Fail(ToErrorMessage(result));

            return ServiceResult.Ok("Đã vô hiệu hóa tài khoản của bạn.");
        }

        private static PatientProfileResponseDTOs MapToResponse(PatientProfile profile, ApplicationUser user)
        {
            return new PatientProfileResponseDTOs
            {
                Id = profile.Id,
                UserId = profile.UserId,
                FullName = profile.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                Address = profile.Address,
                CreatedAt = profile.CreatedAt,
                IsActive = IsActiveUser(user),
                AppointmentCount = profile.Appointments?.Count ?? 0
            };
        }

        private static bool IsActiveUser(ApplicationUser user)
            => !(user.LockoutEnd.HasValue && user.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow);

        private static string? NormalizeOptional(string? value)
        {
            var trimmed = value?.Trim();
            return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
        }

        private static string ToErrorMessage(IdentityResult result)
            => string.Join("; ", result.Errors.Select(x => x.Description));
    }
}
