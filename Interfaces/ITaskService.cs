namespace AFuturaCRMV2.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<Models.Task>> GetAllTasksAsync();
        Task<IEnumerable<Models.Task>> GetUserTasksAsync(string userEmail);
        Task<IEnumerable<Models.Task>> GetPendingTasksAsync(string userEmail);
        Task<IEnumerable<Models.Task>> GetCompletedTasksAsync(string userEmail);
        Task<IEnumerable<Models.Task>> GetOverdueTasksAsync(string userEmail);
        Task<Models.Task> GetTaskDetailsAsync(string id);
        Task<Models.Task> CreateTaskAsync(Models.Task task, string currentUserEmail);
        Task<Models.Task> UpdateTaskAsync(string id, Models.Task task);
        Task<Models.Task> MarkTaskAsCompletedAsync(string id);
        Task<bool> DeleteTaskAsync(string id);
    }
}
