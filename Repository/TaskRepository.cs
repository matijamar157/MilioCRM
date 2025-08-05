using AFuturaCRMV2.Data;
using AFuturaCRMV2.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AFuturaCRMV2.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Models.Task>> GetAllAsync()
        {
            return await _context.Tasks
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Task>> GetByUserAsync(string userEmail)
        {
            return await _context.Tasks
                .Where(t => t.UserEmail == userEmail)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Task>> GetPendingTasksAsync(string userEmail)
        {
            return await _context.Tasks
                .Where(t => t.UserEmail == userEmail && !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Task>> GetCompletedTasksAsync(string userEmail)
        {
            return await _context.Tasks
                .Where(t => t.UserEmail == userEmail && t.IsCompleted)
                .OrderByDescending(t => t.CompletedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Task>> GetOverdueTasksAsync(string userEmail)
        {
            var today = DateTime.Today;
            return await _context.Tasks
                .Where(t => t.UserEmail == userEmail &&
                           !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<Models.Task> GetByIdAsync(string id)
        {
            return await _context.Tasks
                .FirstOrDefaultAsync(t => t.Description == id);
        }

        public async Task<Models.Task> CreateAsync(Models.Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<Models.Task> UpdateAsync(Models.Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Tasks.AnyAsync(t => t.Description == id);
        }
    }
}
