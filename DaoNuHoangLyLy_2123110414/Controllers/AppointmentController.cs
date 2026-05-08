using System.Security.Claims;
using DaoNuHoangLyLy_2123110414.DTOs;
using DaoNuHoangLyLy_2123110414.Models;
using DaoNuHoangLyLy_2123110414.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DaoNuHoangLyLy_2123110414.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        private string? GetCurrentUserId()
            => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private string GetCurrentRole()
        {
            if (User.IsInRole(SystemRoles.Admin)) return SystemRoles.Admin;
            if (User.IsInRole(SystemRoles.Doctor)) return SystemRoles.Doctor;
            return SystemRoles.Patient;
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("my")]
        public async Task<IActionResult> MyAppointments()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var data = await _appointmentService.GetByPatientUserIdAsync(userId);
            return Ok(data);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("doctor/my")]
        public async Task<IActionResult> DoctorAppointments()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var data = await _appointmentService.GetByDoctorUserIdAsync(userId);
            return Ok(data);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAllForAdmin([FromQuery] string? status, [FromQuery] DateTime? date, [FromQuery] int? doctorId)
        {
            var data = await _appointmentService.GetAllForAdminAsync(status, date, doctorId);
            return Ok(data);
        }

        [HttpGet("{appointmentId:int}")]
        public async Task<IActionResult> GetDetail(int appointmentId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var role = GetCurrentRole();
            var result = await _appointmentService.GetDetailForCurrentUserAsync(appointmentId, userId, role);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Patient")]
        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] BookAppointmentDTOs model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _appointmentService.BookAsync(userId, model);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Patient,Admin")]
        [HttpPatch("{appointmentId:int}/cancel")]
        public async Task<IActionResult> Cancel(int appointmentId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var role = GetCurrentRole();
            var result = await _appointmentService.CancelAsync(appointmentId, userId, role);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{appointmentId:int}/confirm")]
        public async Task<IActionResult> Confirm(int appointmentId)
        {
            var result = await _appointmentService.ConfirmAsync(appointmentId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPatch("{appointmentId:int}/complete")]
        public async Task<IActionResult> Complete(int appointmentId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var role = GetCurrentRole();
            var result = await _appointmentService.CompleteAsync(appointmentId, userId, role);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPatch("{appointmentId:int}/noshow")]
        public async Task<IActionResult> NoShow(int appointmentId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var role = GetCurrentRole();
            var result = await _appointmentService.MarkNoShowAsync(appointmentId, userId, role);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
