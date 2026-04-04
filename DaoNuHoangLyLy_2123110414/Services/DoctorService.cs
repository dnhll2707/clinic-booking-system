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

        public async Task<List<DoctorProfile>> GetAllAsync(int? specialtyId = null)
        {
            var query = _context.DoctorProfiles
                .Include(x => x.Specialty)
                .Where(x => x.IsActive)
                .AsQueryable();

            if (specialtyId.HasValue)
                query = query.Where(x => x.SpecialtyId == specialtyId.Value);

            return await query
                .OrderBy(x => x.FullName)
                .ToListAsync();
        }

        public async Task<DoctorProfile?> GetByIdAsync(int id)
        {
            return await _context.DoctorProfiles
                .Include(x => x.Specialty)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ServiceResult<DoctorProfile>> CreateAsync(CreateDoctorDTOs model)
        {
            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(x => x.Id == model.SpecialtyId && x.IsActive);

            if (specialty == null)
                return ServiceResult<DoctorProfile>.Fail("Chuyên khoa không hợp lệ.");

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return ServiceResult<DoctorProfile>.Fail("Email bác sĩ đã tồn tại.");

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(x => x.Description));
                return ServiceResult<DoctorProfile>.Fail(errors);
            }

            await _userManager.AddToRoleAsync(user, SystemRoles.Doctor);

            var doctor = new DoctorProfile
            {
                UserId = user.Id,
                FullName = model.FullName.Trim(),
                SpecialtyId = model.SpecialtyId,
                PhoneNumber = model.PhoneNumber,
                Description = model.Description?.Trim(),
                IsActive = true
            };

            _context.DoctorProfiles.Add(doctor);
            await _context.SaveChangesAsync();

            await _context.Entry(doctor).Reference(x => x.Specialty).LoadAsync();

            return ServiceResult<DoctorProfile>.Ok(doctor, "Tạo bác sĩ thành công.");
        }
    }
}