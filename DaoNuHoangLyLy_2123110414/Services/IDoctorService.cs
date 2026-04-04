using DaoNuHoangLyLy_2123110414.DTOs;
using DaoNuHoangLyLy_2123110414.Models;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public interface IDoctorService
    {
        Task<List<DoctorProfile>> GetAllAsync(int? specialtyId = null);
        Task<DoctorProfile?> GetByIdAsync(int id);
        Task<ServiceResult<DoctorProfile>> CreateAsync(CreateDoctorDTOs model);
    }
}