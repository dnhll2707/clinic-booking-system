using DaoNuHoangLyLy_2123110414.Data;
using DaoNuHoangLyLy_2123110414.DTOs;
using DaoNuHoangLyLy_2123110414.Models;
using Microsoft.EntityFrameworkCore;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly ApplicationDbContext _context;

        public SpecialtyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Specialty>> GetAllAsync(bool activeOnly = true)
        {
            var query = _context.Specialties.AsQueryable();

            if (activeOnly)
                query = query.Where(x => x.IsActive);

            return await query
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<Specialty?> GetByIdAsync(int id)
        {
            return await _context.Specialties.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ServiceResult<Specialty>> CreateAsync(CreateSpecialtyDTOs model)
        {
            var exists = await _context.Specialties.AnyAsync(x => x.Name == model.Name.Trim());
            if (exists)
                return ServiceResult<Specialty>.Fail("Tên chuyên khoa đã tồn tại.");

            var entity = new Specialty
            {
                Name = model.Name.Trim(),
                Description = model.Description?.Trim(),
                IsActive = true
            };

            _context.Specialties.Add(entity);
            await _context.SaveChangesAsync();

            return ServiceResult<Specialty>.Ok(entity, "Tạo chuyên khoa thành công.");
        }

        public async Task<ServiceResult<Specialty>> UpdateAsync(int id, UpdateSpecialtyDTOs model)
        {
            var entity = await _context.Specialties.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return ServiceResult<Specialty>.Fail("Không tìm thấy chuyên khoa.");

            var duplicate = await _context.Specialties.AnyAsync(x => x.Id != id && x.Name == model.Name.Trim());
            if (duplicate)
                return ServiceResult<Specialty>.Fail("Tên chuyên khoa đã tồn tại.");

            entity.Name = model.Name.Trim();
            entity.Description = model.Description?.Trim();
            entity.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            return ServiceResult<Specialty>.Ok(entity, "Cập nhật chuyên khoa thành công.");
        }

        public async Task<ServiceResult> DeactivateAsync(int id)
        {
            var entity = await _context.Specialties.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return ServiceResult.Fail("Không tìm thấy chuyên khoa.");

            entity.IsActive = false;
            await _context.SaveChangesAsync();

            return ServiceResult.Ok("Đã khóa chuyên khoa.");
        }
    }
}