using Microsoft.AspNetCore.Mvc;
using egorIlyinKT_42_22.Models.DTO;
using egorIlyinKT_42_22.Models;
using egorIlyinKT_42_22.Services.LoadServices;

namespace egorIlyinKT_42_22.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoadsController : ControllerBase
    {
        private readonly LoadService _loadService;

        public LoadsController(LoadService loadService)
        {
            _loadService = loadService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLoads([FromQuery] string? teacherFirstName, [FromQuery] string? teacherLastName, [FromQuery] string? departmentName, [FromQuery] string? disciplineName)
        {
            var loads = await _loadService.GetLoadsAsync(teacherFirstName, teacherLastName, departmentName, disciplineName);
            return Ok(loads);
        }

        [HttpPost]
        public async Task<IActionResult> AddLoad([FromBody] LoadDto loadDto)
        {
            if (loadDto == null || loadDto.Hours <= 0)
            {
                return BadRequest("Некорректные данные нагрузки.");
            }

            var load = await _loadService.AddLoadAsync(loadDto.TeacherId, loadDto.DisciplineId, loadDto.Hours);
            return CreatedAtAction(nameof(GetLoads), new { id = load.Id }, load);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLoad(int id, [FromBody] LoadDto loadDto)
        {
            if (loadDto == null || loadDto.Hours <= 0)
            {
                return BadRequest("Некорректные данные нагрузки.");
            }

            try
            {
                var updatedLoad = await _loadService.UpdateLoadAsync(id, loadDto.TeacherId, loadDto.DisciplineId, loadDto.Hours);
                return Ok(updatedLoad);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Нагрузка с id {id} не найдена.");
            }
        }
    }
}