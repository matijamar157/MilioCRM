using AFuturaCRMV2.Interfaces;

namespace AFuturaCRMV2.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(
            ITaskRepository taskRepository,
            ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Models.Task>> GetAllTasksAsync()
        {
            _logger.LogInformation("Retrieving all tasks");
            return await _taskRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Models.Task>> GetUserTasksAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new ArgumentException("User email is required", nameof(userEmail));

            _logger.LogInformation("Retrieving tasks for user: {UserEmail}", userEmail);
            return await _taskRepository.GetByUserAsync(userEmail);
        }

        public async Task<IEnumerable<Models.Task>> GetPendingTasksAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new ArgumentException("User email is required", nameof(userEmail));

            _logger.LogInformation("Retrieving pending tasks for user: {UserEmail}", userEmail);
            return await _taskRepository.GetPendingTasksAsync(userEmail);
        }

        public async Task<IEnumerable<Models.Task>> GetCompletedTasksAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new ArgumentException("User email is required", nameof(userEmail));

            _logger.LogInformation("Retrieving completed tasks for user: {UserEmail}", userEmail);
            return await _taskRepository.GetCompletedTasksAsync(userEmail);
        }

        public async Task<IEnumerable<Models.Task>> GetOverdueTasksAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new ArgumentException("User email is required", nameof(userEmail));

            _logger.LogInformation("Retrieving overdue tasks for user: {UserEmail}", userEmail);
            return await _taskRepository.GetOverdueTasksAsync(userEmail);
        }

        public async Task<Models.Task> GetTaskDetailsAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Task ID is required", nameof(id));

            _logger.LogInformation("Retrieving task details for ID: {TaskId}", id);
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Task not found with ID: {TaskId}", id);
                throw new InvalidOperationException($"Task with ID {id} not found");
            }

            return task;
        }

        public async Task<Models.Task> CreateTaskAsync(Models.Task task, string currentUserEmail)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (string.IsNullOrEmpty(currentUserEmail))
                throw new ArgumentException("Current user email is required", nameof(currentUserEmail));

            task.UserEmail = currentUserEmail;
            task.CreatedDate = DateTime.UtcNow;

            _logger.LogInformation("Creating new task for user: {UserEmail}", currentUserEmail);
            return await _taskRepository.CreateAsync(task);
        }

        public async Task<Models.Task> UpdateTaskAsync(string id, Models.Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Task ID is required", nameof(id));

            var existingTask = await _taskRepository.GetByIdAsync(id);
            if (existingTask == null)
            {
                _logger.LogWarning("Attempted to update non-existent task with ID: {TaskId}", id);
                throw new InvalidOperationException($"Task with ID {id} not found");
            }

            // Update properties
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.IsCompleted = task.IsCompleted;
            existingTask.Priority = task.Priority;
            existingTask.Category = task.Category;
            existingTask.CustomerId = task.CustomerId;

            _logger.LogInformation("Updating task: {TaskId}", id);
            return await _taskRepository.UpdateAsync(existingTask);
        }

        public async Task<Models.Task> MarkTaskAsCompletedAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Task ID is required", nameof(id));

            var existingTask = await _taskRepository.GetByIdAsync(id);
            if (existingTask == null)
            {
                _logger.LogWarning("Attempted to complete non-existent task with ID: {TaskId}", id);
                throw new InvalidOperationException($"Task with ID {id} not found");
            }

            existingTask.IsCompleted = true;
            existingTask.CompletedDate = DateTime.UtcNow;
            existingTask.ModifiedDate = DateTime.UtcNow;

            _logger.LogInformation("Marking task as completed: {TaskId}", id);
            return await _taskRepository.UpdateAsync(existingTask);
        }

        public async Task<bool> DeleteTaskAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Task ID is required", nameof(id));

            _logger.LogInformation("Deleting task with ID: {TaskId}", id);
            var result = await _taskRepository.DeleteAsync(id);

            if (!result)
            {
                _logger.LogWarning("Attempted to delete non-existent task with ID: {TaskId}", id);
            }

            return result;
        }
    }
}
