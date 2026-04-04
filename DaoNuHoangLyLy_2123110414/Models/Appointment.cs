using System.ComponentModel.DataAnnotations;

namespace DaoNuHoangLyLy_2123110414.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public int PatientProfileId { get; set; }
        public PatientProfile? PatientProfile { get; set; }

        [Required]
        public int DoctorProfileId { get; set; }
        public DoctorProfile? DoctorProfile { get; set; }

        [Required]
        public int TimeSlotId { get; set; }
        public TimeSlot? TimeSlot { get; set; }

        [Required]
        [StringLength(50)]
        public string AppointmentCode { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string AppointmentStatus { get; set; } = AppointmentStatuses.Pending;

        [StringLength(1000)]
        public string? Reason { get; set; }

        [StringLength(1000)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}