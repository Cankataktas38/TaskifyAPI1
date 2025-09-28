using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskifyAPI.DTOs;
using TaskifyAPI.Services;
namespace TaskifyAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        [HttpGet]
        public async Task<IActionResult> GetTasks() => Ok(await _taskService.GetTasksAsync(GetUserId()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _taskService.GetTasksAsync(GetUserId());
            return task == null ? NotFound() : Ok(task);
        }
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto dto)
        {
            var created = await _taskService.CreateTaskAsync(dto, GetUserId());
            return CreatedAtAction(nameof(GetTask), new { id = created.Id }, created);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskCreateDto dto)
        {
            var ok = await _taskService.UpdateTaskAsync(id, dto, GetUserId());
            return ok ? NoContent() : NotFound();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var ok = await _taskService.DeleteTaskAsync(id, GetUserId());
            return ok ? NoContent() : NotFound() ;
        }
    }
}
