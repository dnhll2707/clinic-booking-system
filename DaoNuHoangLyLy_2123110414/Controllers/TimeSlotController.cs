using DaoNuHoangLyLy_2123110414.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DaoNuHoangLyLy_2123110414.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeSlotController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;

        public TimeSlotController(ITimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }

        [HttpGet("schedule/{scheduleId:int}")]
        public async Task<IActionResult> GetBySchedule(int scheduleId, [FromQuery] bool availableOnly = false)
        {
            var data = await _timeSlotService.GetByScheduleAsync(scheduleId, availableOnly);
            return Ok(data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{slotId:int}/close")]
        public async Task<IActionResult> Close(int slotId)
        {
            var result = await _timeSlotService.CloseAsync(slotId);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{slotId:int}/reopen")]
        public async Task<IActionResult> Reopen(int slotId)
        {
            var result = await _timeSlotService.ReopenAsync(slotId);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
