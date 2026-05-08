using System.ComponentModel.DataAnnotations;

namespace DaoNuHoangLyLy_2123110414.DTOs
{
    public class UpdateDoctorDTOs
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [StringLength(150, ErrorMessage = "Họ tên không được vượt quá 150 ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số.")]
        public string? PhoneNumber { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn chuyên khoa.")]
        public int SpecialtyId { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
