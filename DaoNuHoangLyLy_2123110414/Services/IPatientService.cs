using DaoNuHoangLyLy_2123110414.DTOs;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public interface IPatientService
    {
        Task<List<PatientProfileResponseDTOs>> GetAllAsync(bool activeOnly = true);
        Task<ServiceResult<PatientProfileResponseDTOs>> GetByIdAsync(int id);
        Task<ServiceResult<PatientProfileResponseDTOs>> CreateAsync(CreatePatientDTOs model);
        Task<ServiceResult<PatientProfileResponseDTOs>> GetCurrentAsync(string userId);
        Task<ServiceResult<PatientProfileResponseDTOs>> UpdateCurrentAsync(string userId, UpdatePatientProfileDTOs model);
        Task<ServiceResult<PatientProfileResponseDTOs>> UpdateByIdAsync(int id, UpdatePatientDTOs model);
        Task<ServiceResult> DeactivateAsync(int id);
        Task<ServiceResult> ActivateAsync(int id);
        Task<ServiceResult> DeactivateCurrentAsync(string userId);
    }
}
