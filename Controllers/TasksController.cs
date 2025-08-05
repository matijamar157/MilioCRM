using AFuturaCRMV2.Data;
using AFuturaCRMV2.Interfaces;
using AFuturaCRMV2.Models;
using AFuturaCRMV2.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AFuturaCRMV2.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILookupService _lookupService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(
            ITaskService taskService,
            ICurrentUserService currentUserService,
            ILookupService lookupService,
            ILogger<TasksController> logger)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _lookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUser = _currentUserService.GetCurrentUsername();
                var tasks = await _taskService.GetUserTasksAsync(currentUser);
                return View(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks for user: {User}", _currentUserService.GetCurrentUsername());
                return Problem("An error occurred while retrieving tasks");
            }
        }

        // GET: Tasks/Pending
        public async Task<IActionResult> Pending()
        {
            try
            {
                var currentUser = _currentUserService.GetCurrentUsername();
                var tasks = await _taskService.GetPendingTasksAsync(currentUser);
                return View(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending tasks for user: {User}", _currentUserService.GetCurrentUsername());
                return Problem("An error occurred while retrieving pending tasks");
            }
        }

        // GET: Tasks/Completed
        public async Task<IActionResult> Completed()
        {
            try
            {
                var currentUser = _currentUserService.GetCurrentUsername();
                var tasks = await _taskService.GetCompletedTasksAsync(currentUser);
                return View(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving completed tasks for user: {User}", _currentUserService.GetCurrentUsername());
                return Problem("An error occurred while retrieving completed tasks");
            }
        }

        // GET: Tasks/Overdue
        public async Task<IActionResult> Overdue()
        {
            try
            {
                var currentUser = _currentUserService.GetCurrentUsername();
                var tasks = await _taskService.GetOverdueTasksAsync(currentUser);
                return View(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving overdue tasks for user: {User}", _currentUserService.GetCurrentUsername());
                return Problem("An error occurred while retrieving overdue tasks");
            }
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                var task = await _taskService.GetTaskDetailsAsync(id);
                return View(task);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task details for ID: {Id}", id);
                return Problem("An error occurred while retrieving task details");
            }
        }

        // GET: Tasks/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                await SetupCreateViewData();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up create task view");
                return Problem("An error occurred while setting up the create task page");
            }
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                await SetupCreateViewData();
                return View(dto);
            }

            try
            {
                var currentUser = _currentUserService.GetCurrentUsername();
                var task = new Models.Task
                {
                    Description = dto.Description,
                    DueDate = (DateTime)dto.DueDate,
                    IsCompleted = dto.IsCompleted,
                    Priority = dto.Priority ?? "Medium",
                    Category = dto.Category,
                    CustomerId = dto.CustomerId
                };

                await _taskService.CreateTaskAsync(task, currentUser);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                ModelState.AddModelError("", "An error occurred while creating the task");
                await SetupCreateViewData();
                return View(dto);
            }
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                var task = await _taskService.GetTaskDetailsAsync(id);
                await SetupEditViewData();

                var updateDto = new UpdateTaskDto
                {
                    Id = task.Description,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    IsCompleted = task.IsCompleted,
                    Priority = task.Priority,
                    Category = task.Category,
                    CustomerId = task.CustomerId
                };

                return View(updateDto);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task for edit: {Id}", id);
                return Problem("An error occurred while retrieving task for editing");
            }
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateTaskDto dto)
        {
            if (string.IsNullOrEmpty(id) || id != dto.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await SetupEditViewData();
                return View(dto);
            }

            try
            {
                var taskToUpdate = new Models.Task
                {
                    Description = dto.Description,
                    DueDate = (DateTime)dto.DueDate,
                    IsCompleted = dto.IsCompleted,
                    Priority = dto.Priority ?? "Medium",
                    Category = dto.Category,
                    CustomerId = dto.CustomerId
                };

                await _taskService.UpdateTaskAsync(id, taskToUpdate);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task: {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the task");
                await SetupEditViewData();
                return View(dto);
            }
        }

        // POST: Tasks/Complete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                await _taskService.MarkTaskAsCompletedAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing task: {Id}", id);
                return Problem("An error occurred while completing the task");
            }
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                var task = await _taskService.GetTaskDetailsAsync(id);
                return View(task);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task for deletion: {Id}", id);
                return Problem("An error occurred while retrieving task for deletion");
            }
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                var result = await _taskService.DeleteTaskAsync(id);
                if (!result)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task: {Id}", id);
                return Problem("An error occurred while deleting the task");
            }
        }

        private async System.Threading.Tasks.Task SetupCreateViewData()
        {
            try
            {
                // Priority options
                ViewData["Priority"] = new SelectList(new[]
                {
                    new { Value = "Low", Text = "Low" },
                    new { Value = "Medium", Text = "Medium" },
                    new { Value = "High", Text = "High" },
                    new { Value = "Critical", Text = "Critical" }
                }, "Value", "Text", "Medium");

                // Category options
                ViewData["Category"] = new SelectList(new[]
                {
                    new { Value = "General", Text = "General" },
                    new { Value = "Sales", Text = "Sales" },
                    new { Value = "Marketing", Text = "Marketing" },
                    new { Value = "Support", Text = "Support" },
                    new { Value = "Follow-up", Text = "Follow-up" }
                }, "Value", "Text");

                // Note: Add customer dropdown if needed
                // var currentUser = _currentUserService.GetCurrentUsername();
                // ViewData["CustomerId"] = new SelectList(await _lookupService.GetUserCustomersAsync(currentUser), "Id", "Name");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up create view data for tasks");
                throw;
            }
        }

        private async System.Threading.Tasks.Task SetupEditViewData()
        {
            try
            {
                // Priority options
                ViewData["Priority"] = new SelectList(new[]
                {
                    new { Value = "Low", Text = "Low" },
                    new { Value = "Medium", Text = "Medium" },
                    new { Value = "High", Text = "High" },
                    new { Value = "Critical", Text = "Critical" }
                }, "Value", "Text");

                // Category options
                ViewData["Category"] = new SelectList(new[]
                {
                    new { Value = "General", Text = "General" },
                    new { Value = "Sales", Text = "Sales" },
                    new { Value = "Marketing", Text = "Marketing" },
                    new { Value = "Support", Text = "Support" },
                    new { Value = "Follow-up", Text = "Follow-up" }
                }, "Value", "Text");

                // Note: Add customer dropdown if needed
                // var currentUser = _currentUserService.GetCurrentUsername();
                // ViewData["CustomerId"] = new SelectList(await _lookupService.GetUserCustomersAsync(currentUser), "Id", "Name");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up edit view data for tasks");
                throw;
            }
        }
    }
}
