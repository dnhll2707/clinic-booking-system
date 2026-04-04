using DaoNuHoangLyLy_2123110414.DTOs;
using DaoNuHoangLyLy_2123110414.Models;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> GetAllForAdminAsync(string? status, DateTime? date, int? doctorId);
        Task<List<Appointment>> GetByPatientUserIdAsync(string userId);
        Task<List<Appointment>> GetByDoctorUserIdAsync(string userId);
        Task<ServiceResult<Appointment>> GetDetailForCurrentUserAsync(int appointmentId, string userId, string role);
        Task<ServiceResult<Appointment>> GetCancelInfoAsync(int appointmentId, string userId, string role);
        Task<ServiceResult> BookAsync(string userId, BookAppointmentDTOs model);
        Task<ServiceResult> CancelAsync(int appointmentId, string userId, string role);
        Task<ServiceResult> ConfirmAsync(int appointmentId);
        Task<ServiceResult> CompleteAsync(int appointmentId, string userId, string role);
        Task<ServiceResult> MarkNoShowAsync(int appointmentId, string userId, string role);
    }
}