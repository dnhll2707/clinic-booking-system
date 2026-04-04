using DaoNuHoangLyLy_2123110414.DTOs;
using DaoNuHoangLyLy_2123110414.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DaoNuHoangLyLy_2123110414.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ITimeSlotService _timeSlotService;

        public ScheduleController(
            IScheduleService scheduleService,
            ITimeSlotService timeSlotService)
        {
            _scheduleService = scheduleService;
            _timeSlotService = timeSlotService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateScheduleDTOs model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _scheduleService.CreateAsync(model);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("doctor/{doctorId:int}")]
        public async Task<IActionResult> GetByDoctor(int doctorId, [FromQuery] DateTime? date)
        {
            var data = await _scheduleService.GetByDoctorAsync(doctorId, date);
            return Ok(data);
        }

        [HttpGet("doctor/{doctorId:int}/available-slots")]
        public async Task<IActionResult> GetAvailableSlots(int doctorId, [FromQuery] DateTime date)
        {
            var data = await _timeSlotService.GetAvailableByDoctorAndDateAsync(doctorId, date);
            return Ok(data);
        }
    }
}