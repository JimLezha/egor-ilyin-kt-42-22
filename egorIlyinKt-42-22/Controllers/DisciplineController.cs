using Microsoft.AspNetCore.Mvc;
using egorIlyinKT_42_22.Filters.DisciplineFilters;
using egorIlyinKT_42_22.Models;
using egorIlyinKT_42_22.Models.DTO;
using egorIlyinKT_42_22.Services.DisciplineServices;
using egorIlyinKT_42_22.Services.DepartmentServices;
using egorIlyinKT_42_22.Services.TeacherServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using egorIlyinKT_42_22.Database;
using egorIlyinKT_42_22.Interfaces;

namespace egorIlyinKT_42_22.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisciplinesController : ControllerBase
    {
        private readonly DisciplineService _disciplineService;

        public DisciplinesController(DisciplineService disciplineService)
        {
            _disciplineService = disciplineService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDisciplines([FromQuery] string? firstName, [FromQuery] string? lastName, [FromQuery] int? minHours, [FromQuery] int? maxHours)
        {
            var disciplines = await _disciplineService.GetDisciplinesAsync(firstName, lastName, minHours, maxHours);
            return Ok(disciplines);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDisciplineById(int id)
        {
            var discipline = await _disciplineService.GetDisciplineByIdAsync(id);
            if (discipline == null)
            {
                return NotFound("Дисциплина не найдена.");
            }

            var disciplineFilter = new DisciplineFilter
            {
                Id = discipline.Id,
                Name = discipline.Name,
                TotalHours = discipline.Loads.Sum(l => l.Hours),
                Teachers = discipline.Loads.Select(l => $"{l.Teacher.FirstName} {l.Teacher.LastName}").ToList()
            };

            return Ok(disciplineFilter);
        }

        [HttpPost]
        public async Task<IActionResult> AddDiscipline([FromBody] DisciplineDto disciplineDto)
        {
            if (disciplineDto == null || string.IsNullOrEmpty(disciplineDto.Name))
            {
                return BadRequest("Дисциплина не указана или имя пустое.");
            }

            var createdDiscipline = await _disciplineService.AddDisciplineAsync(disciplineDto);
            return CreatedAtAction(nameof(GetDisciplineById), new { id = createdDiscipline.Id }, createdDiscipline);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscipline(int id, [FromBody] DisciplineDto disciplineDto)
        {
            if (disciplineDto == null || string.IsNullOrEmpty(disciplineDto.Name))
            {
                return BadRequest("Дисциплина не указана или имя пустое.");
            }

            try
            {
                var updatedDiscipline = await _disciplineService.UpdateDisciplineAsync(id, disciplineDto);
                return Ok(updatedDiscipline);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Дисциплина не найдена.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscipline(int id)
        {
            var result = await _disciplineService.DeleteDisciplineAsync(id);
            if (!result)
            {
                return NotFound("Дисциплина не найдена.");
            }

            return NoContent();
        }


        [HttpGet("head/lastname/{headLastName}")]
        public async Task<IActionResult> GetDisciplinesByHeadLastName(string headLastName)
        {
            var disciplines = await _disciplineService.GetDisciplinesByHeadLastNameAsync(headLastName);
            if (disciplines == null || !disciplines.Any())
            {
                return NotFound("Дисциплины не найдены для указанного заведующего.");
            }
            return Ok(disciplines);
        }
    }
}