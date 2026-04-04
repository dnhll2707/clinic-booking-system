using System.ComponentModel.DataAnnotations;

namespace DaoNuHoangLyLy_2123110414.DTOs
{
    public class BookAppointmentDTOs
    {
        [Required]
        public int TimeSlotId { get; set; }

        [StringLength(1000)]
        public string? Reason { get; set; }

        [StringLength(1000)]
        public string? Note { get; set; }
    }
}