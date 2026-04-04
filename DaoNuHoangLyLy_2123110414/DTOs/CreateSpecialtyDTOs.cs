using System.ComponentModel.DataAnnotations;

namespace DaoNuHoangLyLy_2123110414.DTOs
{
    public class CreateSpecialtyDTOs
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}