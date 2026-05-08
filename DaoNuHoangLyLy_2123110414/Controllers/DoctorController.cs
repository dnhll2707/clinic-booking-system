using DaoNuHoangLyLy_2123110414.DTOs;
using DaoNuHoangLyLy_2123110414.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DaoNuHoangLyLy_2123110414.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? specialtyId, [FromQuery] bool activeOnly = true)
        {
            var data = await _doctorService.GetAllAsync(specialtyId, activeOnly);
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _doctorService.GetByIdAsync(id);
            if (data == null)
                return NotFound(ServiceResult.Fail("Không tìm thấy bác sĩ."));

            return Ok(data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDoctorDTOs model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResult.Fail(GetModelStateMessage()));

            var result = await _doctorService.CreateAsync(model);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDoctorDTOs model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResult.Fail(GetModelStateMessage()));

            var result = await _doctorService.UpdateAsync(id, model);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _doctorService.DeactivateAsync(id);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _doctorService.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        private string GetModelStateMessage()
        {
            var messages = ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? "Dữ liệu không hợp lệ."
                    : error.ErrorMessage)
                .Distinct();

            return string.Join(" ", messages);
        }
    }
}
