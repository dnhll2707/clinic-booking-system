using DaoNuHoangLyLy_2123110414.Data;
using DaoNuHoangLyLy_2123110414.DTOs;
using DaoNuHoangLyLy_2123110414.Models;
using Microsoft.EntityFrameworkCore;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly ApplicationDbContext _context;

        public ScheduleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DoctorSchedule>> GetByDoctorAsync(int doctorId, DateTime? date = null)
        {
            var query = _context.DoctorSchedules
                .Include(x => x.TimeSlots)
                .Where(x => x.DoctorProfileId == doctorId && x.IsActive)
                .AsQueryable();

            if (date.HasValue)
            {
                var targetDate = date.Value.Date;
                query = query.Where(x => x.WorkDate.Date == targetDate);
            }

            return await query
                .OrderBy(x => x.WorkDate)
                .ThenBy(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<ServiceResult<DoctorSchedule>> CreateAsync(CreateScheduleDTOs model)
        {
            var workDate = model.WorkDate.Date;

            if (workDate < DateTime.UtcNow.Date)
                return ServiceResult<DoctorSchedule>.Fail("Không thể tạo lịch trong quá khứ.");

            if (model.StartTime >= model.EndTime)
                return ServiceResult<DoctorSchedule>.Fail("Giờ bắt đầu phải nhỏ hơn giờ kết thúc.");

            var doctor = await _context.DoctorProfiles
                .FirstOrDefaultAsync(x => x.Id == model.DoctorProfileId && x.IsActive);

            if (doctor == null)
                return ServiceResult<DoctorSchedule>.Fail("Bác sĩ không tồn tại hoặc đang bị khóa.");

            var overlapExists = await _context.DoctorSchedules.AnyAsync(x =>
                x.DoctorProfileId == model.DoctorProfileId &&
                x.IsActive &&
                x.WorkDate.Date == workDate &&
                x.StartTime < model.EndTime &&
                model.StartTime < x.EndTime);

            if (overlapExists)
                return ServiceResult<DoctorSchedule>.Fail("Ca làm việc bị trùng với lịch đã có.");

            var totalMinutes = (int)(model.EndTime - model.StartTime).TotalMinutes;
            if (model.SlotDurationMinutes <= 0 || totalMinutes < model.SlotDurationMinutes)
                return ServiceResult<DoctorSchedule>.Fail("SlotDurationMinutes không hợp lệ.");

            var schedule = new DoctorSchedule
            {
                DoctorProfileId = model.DoctorProfileId,
                WorkDate = workDate,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                SlotDurationMinutes = model.SlotDurationMinutes,
                IsActive = true
            };

            _context.DoctorSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            var slots = GenerateSlots(schedule);
            _context.TimeSlots.AddRange(slots);

            await _context.SaveChangesAsync();

            await _context.Entry(schedule).Collection(x => x.TimeSlots).LoadAsync();

            return ServiceResult<DoctorSchedule>.Ok(schedule, "Tạo lịch và sinh slot thành công.");
        }

        private static List<TimeSlot> GenerateSlots(DoctorSchedule schedule)
        {
            var slots = new List<TimeSlot>();
            var current = schedule.StartTime;

            while (current + TimeSpan.FromMinutes(schedule.SlotDurationMinutes) <= schedule.EndTime)
            {
                var end = current + TimeSpan.FromMinutes(schedule.SlotDurationMinutes);

                slots.Add(new TimeSlot
                {
                    DoctorScheduleId = schedule.Id,
                    DoctorProfileId = schedule.DoctorProfileId,
                    SlotDate = schedule.WorkDate.Date,
                    StartTime = current,
                    EndTime = end,
                    SlotStatus = SlotStatuses.Available
                });

                current = end;
            }

            return slots;
        }
    }
}