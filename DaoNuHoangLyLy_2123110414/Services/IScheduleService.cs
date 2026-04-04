using DaoNuHoangLyLy_2123110414.DTOs;
using DaoNuHoangLyLy_2123110414.Models;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public interface IScheduleService
    {
        Task<List<DoctorSchedule>> GetByDoctorAsync(int doctorId, DateTime? date = null);
        Task<ServiceResult<DoctorSchedule>> CreateAsync(CreateScheduleDTOs model);
    }
}