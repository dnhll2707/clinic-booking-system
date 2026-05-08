using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DaoNuHoangLyLy_2123110414.Data;
using DaoNuHoangLyLy_2123110414.DTOs;
using DaoNuHoangLyLy_2123110414.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResult> RegisterPatientAsync(RegisterDTOs model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return ServiceResult.Fail("Email đã tồn tại.");

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(x => x.Description));
                return ServiceResult.Fail(errors);
            }

            await _userManager.AddToRoleAsync(user, SystemRoles.Patient);

            var patientProfile = new PatientProfile
            {
                UserId = user.Id,
                FullName = model.FullName,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = model.Address
            };

            _context.PatientProfiles.Add(patientProfile);
            await _context.SaveChangesAsync();

            return ServiceResult.Ok("Đăng ký tài khoản bệnh nhân thành công.");
        }

        public async Task<ServiceResult<AuthResponseDTOs>> LoginAsync(LoginDTOs model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return ServiceResult<AuthResponseDTOs>.Fail("Sai email hoặc mật khẩu.");

            if (await _userManager.IsLockedOutAsync(user))
                return ServiceResult<AuthResponseDTOs>.Fail("Tài khoản đã bị vô hiệu hóa.");

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
                return ServiceResult<AuthResponseDTOs>.Fail("Sai email hoặc mật khẩu.");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;

            var fullName =
                await _context.PatientProfiles.Where(x => x.UserId == user.Id).Select(x => x.FullName).FirstOrDefaultAsync()
                ?? await _context.DoctorProfiles.Where(x => x.UserId == user.Id).Select(x => x.FullName).FirstOrDefaultAsync()
                ?? user.Email
                ?? string.Empty;

            var tokenResult = GenerateJwtToken(user, role, fullName);

            return ServiceResult<AuthResponseDTOs>.Ok(tokenResult, "Đăng nhập thành công.");
        }

        private AuthResponseDTOs GenerateJwtToken(ApplicationUser user, string role, string fullName)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key missing.");
            var issuer = _configuration["Jwt:Issuer"] ?? throw new Exception("Jwt:Issuer missing.");
            var audience = _configuration["Jwt:Audience"] ?? throw new Exception("Jwt:Audience missing.");

            var expires = DateTime.UtcNow.AddHours(8);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Name, fullName),
                new(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return new AuthResponseDTOs
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = user.Email ?? string.Empty,
                FullName = fullName,
                Role = role,
                ExpiresAtUtc = expires
            };
        }
    }
}
