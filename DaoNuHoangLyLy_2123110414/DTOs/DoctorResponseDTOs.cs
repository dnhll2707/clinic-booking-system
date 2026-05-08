namespace DaoNuHoangLyLy_2123110414.DTOs
{
    public class DoctorResponseDTOs
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int SpecialtyId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DoctorSpecialtyResponseDTOs? Specialty { get; set; }
    }

    public class DoctorSpecialtyResponseDTOs
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
