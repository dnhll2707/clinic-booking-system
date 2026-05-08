using DaoNuHoangLyLy_2123110414.DTOs;
using DaoNuHoangLyLy_2123110414.Models;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public interface IScheduleService
    {
        Task<List<DoctorSchedule>> GetAllAsync(int? doctorId = null, DateTime? date = null, bool activeOnly = true);
        Task<List<DoctorSchedule>> GetByDoctorAsync(int doctorId, DateTime? date = null);
        Task<ServiceResult<DoctorSchedule>> CreateAsync(CreateScheduleDTOs model);
        Task<ServiceResult> DeactivateAsync(int id);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
