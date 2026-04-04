using System.ComponentModel.DataAnnotations;

namespace DaoNuHoangLyLy_2123110414.Models
{
    public class DoctorSchedule
    {
        public int Id { get; set; }

        [Required]
        public int DoctorProfileId { get; set; }
        public DoctorProfile? DoctorProfile { get; set; }

        [Required]
        public DateTime WorkDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Range(5, 240)]
        public int SlotDurationMinutes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
    }
}