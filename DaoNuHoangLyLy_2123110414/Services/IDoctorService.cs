using DaoNuHoangLyLy_2123110414.DTOs;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public interface IDoctorService
    {
        Task<List<DoctorResponseDTOs>> GetAllAsync(int? specialtyId = null, bool activeOnly = true);
        Task<DoctorResponseDTOs?> GetByIdAsync(int id);
        Task<ServiceResult<DoctorResponseDTOs>> CreateAsync(CreateDoctorDTOs model);
        Task<ServiceResult<DoctorResponseDTOs>> UpdateAsync(int id, UpdateDoctorDTOs model);
        Task<ServiceResult> DeactivateAsync(int id);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
