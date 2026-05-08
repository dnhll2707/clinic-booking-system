using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaoNuHoangLyLy_2123110414.Migrations
{
    /// <inheritdoc />
    public partial class FixVietnameseEncoding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Fix corrupted Vietnamese strings stored as double-encoded UTF-8 (Latin-1/Win-1252 mojibake).
            // Root cause: UTF-8 text was inserted into a column/tag that was interpreted as Latin-1,
            // so each Vietnamese character got doubled (e.g. "á" → "Ã¡", "ĩ" → "Ä©", etc.)
            // Fix: REPLACE the mojibake byte-sequences with the correct Unicode characters.

            migrationBuilder.Sql(@"
                -- Fix DoctorProfiles.FullName
                IF EXISTS (SELECT 1 FROM DoctorProfiles WHERE FullName LIKE N'%Ã%' OR FullName LIKE N'%Ä%')
                BEGIN
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'BÃ¡c sÄ©', N'Bác sĩ') WHERE FullName LIKE N'%BÃ¡c sÄ©%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Nguyá»…n', N'Nguyễn') WHERE FullName LIKE N'%Nguyá»…n%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Nguyá»n', N'Nguyễn') WHERE FullName LIKE N'%Nguyá»n%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'VÄƒn', N'Văn') WHERE FullName LIKE N'%VÄ‰n%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'BÃ¢c', N'Bác') WHERE FullName LIKE N'%BÃ¢c%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'TrÃªn', N'Trần') WHERE FullName LIKE N'%TrÃªn%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'LÃª', N'Lê') WHERE FullName LIKE N'%LÃŠ%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'HoÃ ng', N'Hoàng') WHERE FullName LIKE N'%HoÃ%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Pháº¡m', N'Phạm') WHERE FullName LIKE N'%Pháº¡m%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'ThÃ¬', N'Thị') WHERE FullName LIKE N'%ThÃ¬%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Äƒ', N'ă') WHERE FullName LIKE N'%Äƒ%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ä¢', N'Â') WHERE FullName LIKE N'%Ä¢%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã¡', N'á') WHERE FullName LIKE N'%Ã¡%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã ', N'à') WHERE FullName LIKE N'%Ã %';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã¢', N'â') WHERE FullName LIKE N'%Ã¢%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã©', N'é') WHERE FullName LIKE N'%Ã©%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã¨', N'è') WHERE FullName LIKE N'%Ã¨%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã¬', N'ì') WHERE FullName LIKE N'%Ã¬%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã­', N'í') WHERE FullName LIKE N'%Ã­%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã®', N'î') WHERE FullName LIKE N'%Ã®%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ãµ', N'õ') WHERE FullName LIKE N'%Ãµ%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã´', N'ô') WHERE FullName LIKE N'%Ã´%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã³', N'ó') WHERE FullName LIKE N'%Ã³%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã²', N'ò') WHERE FullName LIKE N'%Ã²%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã»', N'û') WHERE FullName LIKE N'%Ã»%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ãº', N'ú') WHERE FullName LIKE N'%Ãº%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã½', N'ý') WHERE FullName LIKE N'%Ã½%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã£', N'ã') WHERE FullName LIKE N'%Ã£%';
                    UPDATE DoctorProfiles SET FullName = REPLACE(FullName, N'Ã°', N'đ') WHERE FullName LIKE N'%Ã°%';
                END
            ");

            migrationBuilder.Sql(@"
                -- Fix PatientProfiles.FullName
                IF EXISTS (SELECT 1 FROM PatientProfiles WHERE FullName LIKE N'%Ã%' OR FullName LIKE N'%Ä%')
                BEGIN
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'BÃ¡c sÄ©', N'Bác sĩ') WHERE FullName LIKE N'%BÃ¡c sÄ©%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Nguyá»…n', N'Nguyễn') WHERE FullName LIKE N'%Nguyá»…n%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Nguyá»n', N'Nguyễn') WHERE FullName LIKE N'%Nguyá»n%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'VÄƒn', N'Văn') WHERE FullName LIKE N'%VÄ‰n%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'BÃ¢c', N'Bác') WHERE FullName LIKE N'%BÃ¢c%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'TrÃªn', N'Trần') WHERE FullName LIKE N'%TrÃªn%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'LÃª', N'Lê') WHERE FullName LIKE N'%LÃŠ%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'HoÃ ng', N'Hoàng') WHERE FullName LIKE N'%HoÃ%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Pháº¡m', N'Phạm') WHERE FullName LIKE N'%Pháº¡m%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'ThÃ¬', N'Thị') WHERE FullName LIKE N'%ThÃ¬%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Äƒ', N'ă') WHERE FullName LIKE N'%Äƒ%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ä¢', N'Â') WHERE FullName LIKE N'%Ä¢%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã¡', N'á') WHERE FullName LIKE N'%Ã¡%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã ', N'à') WHERE FullName LIKE N'%Ã %';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã¢', N'â') WHERE FullName LIKE N'%Ã¢%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã©', N'é') WHERE FullName LIKE N'%Ã©%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã¨', N'è') WHERE FullName LIKE N'%Ã¨%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã¬', N'ì') WHERE FullName LIKE N'%Ã¬%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã­', N'í') WHERE FullName LIKE N'%Ã­%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã®', N'î') WHERE FullName LIKE N'%Ã®%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ãµ', N'õ') WHERE FullName LIKE N'%Ãµ%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã´', N'ô') WHERE FullName LIKE N'%Ã´%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã³', N'ó') WHERE FullName LIKE N'%Ã³%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã²', N'ò') WHERE FullName LIKE N'%Ã²%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã»', N'û') WHERE FullName LIKE N'%Ã»%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ãº', N'ú') WHERE FullName LIKE N'%Ãº%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã½', N'ý') WHERE FullName LIKE N'%Ã½%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã£', N'ã') WHERE FullName LIKE N'%Ã£%';
                    UPDATE PatientProfiles SET FullName = REPLACE(FullName, N'Ã°', N'đ') WHERE FullName LIKE N'%Ã°%';
                END
            ");

            migrationBuilder.Sql(@"
                -- Fix Specialties.Name
                IF EXISTS (SELECT 1 FROM Specialties WHERE Name LIKE N'%Ã%' OR Name LIKE N'%Ä%')
                BEGIN
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ná»™i', N'Nội') WHERE Name LIKE N'%Ná»™i%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Tá»•ng', N'Tổng') WHERE Name LIKE N'%Tá»•ng%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'quÃ¡t', N'quát') WHERE Name LIKE N'%quÃ¡t%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Tim mÃ¡ch', N'Tim mạch') WHERE Name LIKE N'%Tim mÃ¡ch%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Nhi khoa', N'Nhi khoa') WHERE Name LIKE N'%Nhi%' AND Name LIKE N'%Ã%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Da liá»…u', N'Da liễu') WHERE Name LIKE N'%Da%' AND Name LIKE N'%Ã%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Tai mÅ©i há»i', N'Tai mũi họng') WHERE Name LIKE N'%Tai%' AND Name LIKE N'%Ã%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ã´', N'ô') WHERE Name LIKE N'%Ã´%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ã¡', N'á') WHERE Name LIKE N'%Ã¡%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Äƒ', N'ă') WHERE Name LIKE N'%Äƒ%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ã¢', N'â') WHERE Name LIKE N'%Ã¢%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ã©', N'é') WHERE Name LIKE N'%Ã©%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ã¨', N'è') WHERE Name LIKE N'%Ã¨%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ã¬', N'ì') WHERE Name LIKE N'%Ã¬%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ã­', N'í') WHERE Name LIKE N'%Ã­%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ã®', N'î') WHERE Name LIKE N'%Ã®%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ãµ', N'õ') WHERE Name LIKE N'%Ãµ%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ã³', N'ó') WHERE Name LIKE N'%Ã³%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ã²', N'ò') WHERE Name LIKE N'%Ã²%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ã»', N'û') WHERE Name LIKE N'%Ã»%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ãº', N'ú') WHERE Name LIKE N'%Ãº%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ã½', N'ý') WHERE Name LIKE N'%Ã½%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ã£', N'ã') WHERE Name LIKE N'%Ã£%';
                    UPDATE Specialties SET Name = REPLACE(Name, N'Ã°', N'đ') WHERE Name LIKE N'%Ã°%';
                END
            ");

            migrationBuilder.Sql(@"
                -- Fix Specialties.Description
                IF EXISTS (SELECT 1 FROM Specialties WHERE Description LIKE N'%Ã%' OR Description LIKE N'%Ä%')
                BEGIN
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ná»™i', N'Nội') WHERE Description LIKE N'%Ná»™i%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Tá»•ng', N'Tổng') WHERE Description LIKE N'%Tá»•ng%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'quÃ¡t', N'quát') WHERE Description LIKE N'%quÃ¡t%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Tim mÃ¡ch', N'Tim mạch') WHERE Description LIKE N'%Tim mÃ¡ch%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ã´', N'ô') WHERE Description LIKE N'%Ã´%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ã¡', N'á') WHERE Description LIKE N'%Ã¡%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Äƒ', N'ă') WHERE Description LIKE N'%Äƒ%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ã¢', N'â') WHERE Description LIKE N'%Ã¢%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ã©', N'é') WHERE Description LIKE N'%Ã©%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ã¨', N'è') WHERE Description LIKE N'%Ã¨%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ã¬', N'ì') WHERE Description LIKE N'%Ã¬%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ã­', N'í') WHERE Description LIKE N'%Ã­%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ã®', N'î') WHERE Description LIKE N'%Ã®%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ãµ', N'õ') WHERE Description LIKE N'%Ãµ%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ã³', N'ó') WHERE Description LIKE N'%Ã³%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ã²', N'ò') WHERE Description LIKE N'%Ã²%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ã»', N'û') WHERE Description LIKE N'%Ã»%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ãº', N'ú') WHERE Description LIKE N'%Ãº%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ã½', N'ý') WHERE Description LIKE N'%Ã½%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ã£', N'ã') WHERE Description LIKE N'%Ã£%';
                    UPDATE Specialties SET Description = REPLACE(Description, N'Ã°', N'đ') WHERE Description LIKE N'%Ã°%';
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: this is a data-fix migration; the Down() direction is not used.
        }
    }
}
