using DaoNuHoangLyLy_2123110414.Data;
using DaoNuHoangLyLy_2123110414.DTOs;
using DaoNuHoangLyLy_2123110414.Models;
using Microsoft.EntityFrameworkCore;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAllForAdminAsync(string? status, DateTime? date, int? doctorId)
        {
            var query = BaseAppointmentQuery();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.AppointmentStatus == status);

            if (date.HasValue)
            {
                var targetDate = date.Value.Date;
                query = query.Where(x => x.TimeSlot != null && x.TimeSlot.SlotDate.Date == targetDate);
            }

            if (doctorId.HasValue)
                query = query.Where(x => x.DoctorProfileId == doctorId.Value);

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetByPatientUserIdAsync(string userId)
        {
            return await BaseAppointmentQuery()
                .Where(x => x.PatientProfile != null && x.PatientProfile.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetByDoctorUserIdAsync(string userId)
        {
            return await BaseAppointmentQuery()
                .Where(x => x.DoctorProfile != null && x.DoctorProfile.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<ServiceResult<Appointment>> GetDetailForCurrentUserAsync(int appointmentId, string userId, string role)
        {
            var appointment = await BaseAppointmentQuery()
                .FirstOrDefaultAsync(x => x.Id == appointmentId);

            if (appointment == null)
                return ServiceResult<Appointment>.Fail("Không tìm thấy lịch hẹn.");

            if (!CanAccessAppointment(appointment, userId, role))
                return ServiceResult<Appointment>.Fail("Bạn không có quyền xem lịch hẹn này.");

            return ServiceResult<Appointment>.Ok(appointment);
        }

        public async Task<ServiceResult<Appointment>> GetCancelInfoAsync(int appointmentId, string userId, string role)
        {
            var detail = await GetDetailForCurrentUserAsync(appointmentId, userId, role);
            if (!detail.Success || detail.Data == null)
                return detail;

            return ServiceResult<Appointment>.Ok(detail.Data);
        }

        public async Task<ServiceResult> BookAsync(string userId, BookAppointmentDTOs model)
        {
            var patientProfile = await _context.PatientProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (patientProfile == null)
                return ServiceResult.Fail("Không tìm thấy hồ sơ bệnh nhân.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var slot = await _context.TimeSlots
                    .FirstOrDefaultAsync(x => x.Id == model.TimeSlotId);

                if (slot == null)
                    return ServiceResult.Fail("Khung giờ không tồn tại.");

                if (slot.SlotStatus != SlotStatuses.Available)
                    return ServiceResult.Fail("Khung giờ này không còn trống.");

                var exists = await _context.Appointments
                    .AnyAsync(x => x.TimeSlotId == slot.Id);

                if (exists)
                    return ServiceResult.Fail("Khung giờ này vừa được người khác đặt.");

                var appointment = new Appointment
                {
                    PatientProfileId = patientProfile.Id,
                    DoctorProfileId = slot.DoctorProfileId,
                    TimeSlotId = slot.Id,
                    AppointmentCode = GenerateAppointmentCode(),
                    AppointmentStatus = AppointmentStatuses.Pending,
                    Reason = model.Reason?.Trim(),
                    Note = model.Note?.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                slot.SlotStatus = SlotStatuses.Booked;

                _context.Appointments.Add(appointment);
                _context.TimeSlots.Update(slot);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ServiceResult.Ok("Đặt lịch thành công.");
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                return ServiceResult.Fail("Khung giờ này vừa được người khác đặt, vui lòng chọn khung giờ khác.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return ServiceResult.Fail("Có lỗi xảy ra khi đặt lịch.");
            }
        }

        public async Task<ServiceResult> CancelAsync(int appointmentId, string userId, string role)
        {
            var appointment = await BaseAppointmentQuery()
                .FirstOrDefaultAsync(x => x.Id == appointmentId);

            if (appointment == null)
                return ServiceResult.Fail("Không tìm thấy lịch hẹn.");

            if (!CanAccessCancel(appointment, userId, role))
                return ServiceResult.Fail("Bạn không có quyền hủy lịch hẹn này.");

            if (appointment.AppointmentStatus != AppointmentStatuses.Pending &&
                appointment.AppointmentStatus != AppointmentStatuses.Confirmed)
            {
                return ServiceResult.Fail("Chỉ có thể hủy lịch ở trạng thái Pending hoặc Confirmed.");
            }

            if (role == SystemRoles.Patient && appointment.TimeSlot != null)
            {
                var slotStartUtc = appointment.TimeSlot.SlotDate.Date.Add(appointment.TimeSlot.StartTime);
                if (slotStartUtc <= DateTime.UtcNow.AddHours(2))
                    return ServiceResult.Fail("Bạn chỉ được hủy lịch trước giờ khám ít nhất 2 tiếng.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                appointment.AppointmentStatus = AppointmentStatuses.Cancelled;
                appointment.CancelledAt = DateTime.UtcNow;

                if (appointment.TimeSlot != null)
                {
                    appointment.TimeSlot.SlotStatus = SlotStatuses.Available;
                    _context.TimeSlots.Update(appointment.TimeSlot);
                }

                _context.Appointments.Update(appointment);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ServiceResult.Ok("Hủy lịch thành công.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return ServiceResult.Fail("Có lỗi xảy ra khi hủy lịch.");
            }
        }

        public async Task<ServiceResult> ConfirmAsync(int appointmentId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(x => x.Id == appointmentId);

            if (appointment == null)
                return ServiceResult.Fail("Không tìm thấy lịch hẹn.");

            if (appointment.AppointmentStatus != AppointmentStatuses.Pending)
                return ServiceResult.Fail("Chỉ lịch Pending mới được xác nhận.");

            appointment.AppointmentStatus = AppointmentStatuses.Confirmed;
            appointment.ConfirmedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ServiceResult.Ok("Xác nhận lịch hẹn thành công.");
        }

        public async Task<ServiceResult> CompleteAsync(int appointmentId, string userId, string role)
        {
            var appointment = await BaseAppointmentQuery()
                .FirstOrDefaultAsync(x => x.Id == appointmentId);

            if (appointment == null)
                return ServiceResult.Fail("Không tìm thấy lịch hẹn.");

            if (!CanDoctorOrAdminUpdate(appointment, userId, role))
                return ServiceResult.Fail("Bạn không có quyền hoàn tất lịch hẹn này.");

            if (appointment.AppointmentStatus != AppointmentStatuses.Confirmed)
                return ServiceResult.Fail("Chỉ lịch Confirmed mới được hoàn tất.");

            appointment.AppointmentStatus = AppointmentStatuses.Completed;
            appointment.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ServiceResult.Ok("Cập nhật Completed thành công.");
        }

        public async Task<ServiceResult> MarkNoShowAsync(int appointmentId, string userId, string role)
        {
            var appointment = await BaseAppointmentQuery()
                .FirstOrDefaultAsync(x => x.Id == appointmentId);

            if (appointment == null)
                return ServiceResult.Fail("Không tìm thấy lịch hẹn.");

            if (!CanDoctorOrAdminUpdate(appointment, userId, role))
                return ServiceResult.Fail("Bạn không có quyền cập nhật NoShow cho lịch hẹn này.");

            if (appointment.AppointmentStatus != AppointmentStatuses.Confirmed)
                return ServiceResult.Fail("Chỉ lịch Confirmed mới được cập nhật NoShow.");

            appointment.AppointmentStatus = AppointmentStatuses.NoShow;
            appointment.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ServiceResult.Ok("Đã cập nhật trạng thái NoShow.");
        }

        private IQueryable<Appointment> BaseAppointmentQuery()
        {
            return _context.Appointments
                .Include(x => x.PatientProfile)
                .Include(x => x.DoctorProfile)
                    .ThenInclude(x => x!.Specialty)
                .Include(x => x.TimeSlot);
        }

        private static bool CanAccessAppointment(Appointment appointment, string userId, string role)
        {
            if (role == SystemRoles.Admin) return true;
            if (role == SystemRoles.Patient && appointment.PatientProfile?.UserId == userId) return true;
            if (role == SystemRoles.Doctor && appointment.DoctorProfile?.UserId == userId) return true;
            return false;
        }

        private static bool CanAccessCancel(Appointment appointment, string userId, string role)
        {
            if (role == SystemRoles.Admin) return true;
            return role == SystemRoles.Patient && appointment.PatientProfile?.UserId == userId;
        }

        private static bool CanDoctorOrAdminUpdate(Appointment appointment, string userId, string role)
        {
            if (role == SystemRoles.Admin) return true;
            return role == SystemRoles.Doctor && appointment.DoctorProfile?.UserId == userId;
        }

        private static string GenerateAppointmentCode()
        {
            return $"APT-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";
        }
    }
}