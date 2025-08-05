namespace AFuturaCRMV2.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<Models.Task>> GetAllAsync();
        Task<Models.Task> GetByIdAsync(string id);
        Task<Models.Task> CreateAsync(Models.Task task);
        Task<Models.Task> UpdateAsync(Models.Task task);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<IEnumerable<Models.Task>> GetByUserAsync(string userEmail);
        Task<IEnumerable<Models.Task>> GetPendingTasksAsync(string userEmail);
        Task<IEnumerable<Models.Task>> GetCompletedTasksAsync(string userEmail);
        Task<IEnumerable<Models.Task>> GetOverdueTasksAsync(string userEmail);
    }
}
