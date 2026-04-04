using System.ComponentModel.DataAnnotations;

namespace DaoNuHoangLyLy_2123110414.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }

        [Required]
        public int DoctorScheduleId { get; set; }
        public DoctorSchedule? DoctorSchedule { get; set; }

        [Required]
        public int DoctorProfileId { get; set; }
        public DoctorProfile? DoctorProfile { get; set; }

        [Required]
        public DateTime SlotDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [StringLength(30)]
        public string SlotStatus { get; set; } = SlotStatuses.Available;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Appointment? Appointment { get; set; }
    }
}