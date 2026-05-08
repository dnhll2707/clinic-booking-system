using DaoNuHoangLyLy_2123110414.DTOs;
using DaoNuHoangLyLy_2123110414.Models;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public interface ISpecialtyService
    {
        Task<List<Specialty>> GetAllAsync(bool activeOnly = true);
        Task<Specialty?> GetByIdAsync(int id);
        Task<ServiceResult<Specialty>> CreateAsync(CreateSpecialtyDTOs model);
        Task<ServiceResult<Specialty>> UpdateAsync(int id, UpdateSpecialtyDTOs model);
        Task<ServiceResult> DeactivateAsync(int id);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
