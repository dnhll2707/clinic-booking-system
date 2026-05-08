using System.ComponentModel.DataAnnotations;

namespace DaoNuHoangLyLy_2123110414.DTOs
{
    public class UpdatePatientDTOs
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên bệnh nhân.")]
        [StringLength(150, ErrorMessage = "Họ tên bệnh nhân không được vượt quá 150 ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số.")]
        public string? PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(20, ErrorMessage = "Giới tính không được vượt quá 20 ký tự.")]
        public string? Gender { get; set; }

        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự.")]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
