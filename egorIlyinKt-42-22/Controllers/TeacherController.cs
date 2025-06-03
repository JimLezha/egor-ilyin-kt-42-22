using Microsoft.AspNetCore.Mvc;
using egorIlyinKT_42_22.Services.TeacherServices;
using egorIlyinKT_42_22.Filters.TeacherFilters;
using egorIlyinKT_42_22.Models;
using egorIlyinKT_42_22.Models.DTO;

namespace egorIlyinKT_42_22.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly TeacherService _teacherService;

        public TeachersController(TeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetTeachers([FromQuery] string? departmentName, [FromQuery] string? degreeName, [FromQuery] string? positionName)
        {
            var teachers = await _teacherService.GetTeachersAsync(departmentName, degreeName, positionName);
            return Ok(teachers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> GetTeacherById(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
            {
                return NotFound($"Учитель с идентификатором {id} не найден.");
            }
            return Ok(teacher);
        }

        [HttpPost]
        public async Task<IActionResult> AddTeacher([FromBody] TeacherDto teacherDto)
        {
            if (teacherDto == null)
            {
                return BadRequest("Некорректные данные учителя.");
            }

            var teacherResponse = await _teacherService.AddTeacherAsync(
                teacherDto.FirstName,
                teacherDto.LastName,
                teacherDto.PositionId,
                teacherDto.DegreeId,
                teacherDto.DepartmentId
            );

            return CreatedAtAction(nameof(GetTeacherById), new { id = teacherResponse.Id }, teacherResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, [FromBody] TeacherDto teacherDto)
        {
            if (teacherDto == null)
            {
                return BadRequest("Некорректные данные учителя.");
            }

            var teacherResponse = await _teacherService.UpdateTeacherAsync(
                id,
                teacherDto.FirstName,
                teacherDto.LastName,
                teacherDto.PositionId,
                teacherDto.DegreeId,
                teacherDto.DepartmentId
            );

            if (teacherResponse == null)
            {
                return NotFound($"Учитель с идентификатором {id} не найден.");
            }

            return Ok(teacherResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeacher(int id)
        {
            var result = await _teacherService.DeleteTeacherAsync(id);
            if (!result)
            {
                return NotFound($"Учитель с идентификатором {id} не найден.");
            }
            return NoContent();
        }
    }
}