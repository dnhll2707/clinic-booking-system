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

        public async Task<List<TimeSlot>> GetAvailableByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            var targetDate = date.Date;

            return await _context.TimeSlots
                .Where(x =>
                    x.DoctorProfileId == doctorId &&
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
    }
}