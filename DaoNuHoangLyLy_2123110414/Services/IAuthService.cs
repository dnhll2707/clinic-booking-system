using DaoNuHoangLyLy_2123110414.DTOs;

namespace DaoNuHoangLyLy_2123110414.Services
{
    public interface IAuthService
    {
        Task<ServiceResult> RegisterPatientAsync(RegisterDTOs model);
        Task<ServiceResult<AuthResponseDTOs>> LoginAsync(LoginDTOs model);
    }
}