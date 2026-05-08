using DaoNuHoangLyLy_2123110414.Models;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public interface ITimeSlotService
    {
        Task<List<TimeSlot>> GetByScheduleAsync(int scheduleId, bool availableOnly = false);
        Task<List<TimeSlot>> GetAvailableByDoctorAndDateAsync(int doctorId, DateTime date);
        Task<TimeSlot?> GetByIdAsync(int id);
        Task<ServiceResult> CloseAsync(int id);
        Task<ServiceResult> ReopenAsync(int id);
    }
}
