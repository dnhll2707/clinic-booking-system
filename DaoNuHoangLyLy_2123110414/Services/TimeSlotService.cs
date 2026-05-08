using DaoNuHoangLyLy_2123110414.Data;
using DaoNuHoangLyLy_2123110414.Models;
using Microsoft.EntityFrameworkCore;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly ApplicationDbContext _context;

        public TimeSlotService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TimeSlot>> GetByScheduleAsync(int scheduleId, bool availableOnly = false)
        {
            var query = _context.TimeSlots
                .Include(x => x.DoctorProfile)
                .Where(x => x.DoctorScheduleId == scheduleId)
                .AsQueryable();

            if (availableOnly)
                query = query.Where(x => x.SlotStatus == SlotStatuses.Available);

            return await query
                .OrderBy(x => x.SlotDate)
                .ThenBy(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<List<TimeSlot>> GetAvailableByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            var targetDate = date.Date;

            return await _context.TimeSlots
                .Include(x => x.DoctorSchedule)
                .Include(x => x.DoctorProfile)
                .Where(x =>
                    x.DoctorProfileId == doctorId &&
                    x.DoctorSchedule != null &&
                    x.DoctorSchedule.IsActive &&
                    x.DoctorProfile != null &&
                    x.DoctorProfile.IsActive &&
                    x.SlotDate.Date == targetDate &&
                    x.SlotStatus == SlotStatuses.Available)
                .OrderBy(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<TimeSlot?> GetByIdAsync(int id)
        {
            return await _context.TimeSlots
                .Include(x => x.DoctorProfile)
                .Include(x => x.DoctorSchedule)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ServiceResult> CloseAsync(int id)
        {
            var slot = await _context.TimeSlots.FirstOrDefaultAsync(x => x.Id == id);
            if (slot == null)
                return ServiceResult.Fail("Không tìm thấy khung giờ.");

            var hasActiveAppointment = await HasActiveAppointmentAsync(id);
            if (hasActiveAppointment)
                return ServiceResult.Fail("Không thể đóng khung giờ đang có lịch hẹn Pending hoặc Confirmed.");

            slot.SlotStatus = SlotStatuses.Closed;
            await _context.SaveChangesAsync();

            return ServiceResult.Ok("Đã đóng khung giờ.");
        }

        public async Task<ServiceResult> ReopenAsync(int id)
        {
            var slot = await _context.TimeSlots
                .Include(x => x.DoctorSchedule)
                .Include(x => x.DoctorProfile)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (slot == null)
                return ServiceResult.Fail("Không tìm thấy khung giờ.");

            if (slot.DoctorSchedule == null || !slot.DoctorSchedule.IsActive)
                return ServiceResult.Fail("Không thể mở lại khung giờ thuộc lịch làm việc đã khóa.");

            if (slot.DoctorProfile == null || !slot.DoctorProfile.IsActive)
                return ServiceResult.Fail("Không thể mở lại khung giờ của bác sĩ đang bị khóa.");

            if (slot.SlotDate.Date < DateTime.Today)
                return ServiceResult.Fail("Không thể mở lại khung giờ trong quá khứ.");

            var hasAppointmentHistory = await _context.Appointments.AnyAsync(x => x.TimeSlotId == id);
            if (hasAppointmentHistory)
                return ServiceResult.Fail("Khung giờ đã từng phát sinh lịch hẹn nên không thể mở lại.");

            slot.SlotStatus = SlotStatuses.Available;
            await _context.SaveChangesAsync();

            return ServiceResult.Ok("Đã mở lại khung giờ.");
        }

        private async Task<bool> HasActiveAppointmentAsync(int slotId)
        {
            return await _context.Appointments.AnyAsync(x =>
                x.TimeSlotId == slotId &&
                (x.AppointmentStatus == AppointmentStatuses.Pending ||
                 x.AppointmentStatus == AppointmentStatuses.Confirmed));
        }
    }
}
