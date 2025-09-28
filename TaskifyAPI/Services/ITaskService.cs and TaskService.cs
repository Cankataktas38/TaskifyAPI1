using Microsoft.EntityFrameworkCore;
using TaskifyAPI.Data;
using TaskifyAPI.Models;
using TaskifyAPI.DTOs;
namespace TaskifyAPI.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetTasksAsync(int userId);
        Task<TaskDto?> GetTaskAsync(int id, int userId);
        Task<TaskDto> CreateTaskAsync(TaskCreateDto dto, int userId);
        Task<bool> UpdateTaskAsync(int id, TaskCreateDto dto, int userId);
        Task<bool> DeleteTaskAsync(int id, int userId);
    }
    public class TaskService : ITaskService
    {
        private readonly AppDbContext? _context;
        public TaskService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TaskDto>> GetTasksAsync(int userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId)
                .Select(t => new TaskDto { Id = t.Id, Title = t.Title, Description = t.Description, IsCompleted = t.IsCompleted, UserId = t.UserId })
                .AsNoTracking().ToListAsync();
        }
        public async Task<TaskDto?> GetTaskAsync(int id, int userId)
        {
            var t = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            return t == null ? null : new TaskDto { Id = t.Id, Title = t.Title, Description = t.Description, IsCompleted = t.IsCompleted, UserId = t.UserId };
        }
        public async Task<TaskDto> CreateTaskAsync(TaskCreateDto dto, int userId)
        {
            var entity = new TaskItem { Title = dto.Title, Description = dto.Description, UserId = userId };
            _context.Tasks.Add(entity);
            await _context.SaveChangesAsync();
            return new TaskDto { Id = entity.Id,Title = entity.Title, Description = entity.Description, IsCompleted = entity.IsCompleted,UserId = entity.UserId };
        }
        public async Task<bool> UpdateTaskAsync(int id,TaskCreateDto dto,int userId)
        {
            var t = await _context.Tasks.FirstOrDefaultAsync(x =>x.Id == id && x.UserId == userId);
            if(t == null) return false;
            t.Title = dto.Title;
            t.Description = dto.Description;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteTaskAsync(int id,int userId)
        {
            var t = await _context.Tasks.FirstOrDefaultAsync( x => x.Id == id && x.UserId == userId);
            if(t == null) return false;
            _context.Tasks.Remove(t);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
