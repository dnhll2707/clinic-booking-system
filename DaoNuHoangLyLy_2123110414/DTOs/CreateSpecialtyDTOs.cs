using System.ComponentModel.DataAnnotations;

namespace DaoNuHoangLyLy_2123110414.DTOs
{
    public class CreateSpecialtyDTOs
    {
        [Required(ErrorMessage = "Vui lòng nhập tên chuyên khoa.")]
        [StringLength(150, ErrorMessage = "Tên chuyên khoa không được vượt quá 150 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string? Description { get; set; }
    }
}
