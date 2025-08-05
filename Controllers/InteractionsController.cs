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
    public class InteractionsController : Controller
    {
        private readonly IInteractionService _interactionService;
        private readonly IInteractionTypeService _interactionTypeService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<InteractionsController> _logger;

        public InteractionsController(
            IInteractionService interactionService,
            IInteractionTypeService interactionTypeService,
            ICurrentUserService currentUserService,
            ILogger<InteractionsController> logger)
        {
            _interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
            _interactionTypeService = interactionTypeService ?? throw new ArgumentNullException(nameof(interactionTypeService));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Interactions
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUser = _currentUserService.GetCurrentUsername();
                var interactions = await _interactionService.GetUserInteractionsAsync(currentUser);
                return View(interactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving interactions for user: {User}", _currentUserService.GetCurrentUsername());
                return Problem("An error occurred while retrieving interactions");
            }
        }

        // GET: Interactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var interaction = await _interactionService.GetInteractionDetailsAsync(id.Value);
                return View(interaction);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving interaction details for ID: {Id}", id);
                return Problem("An error occurred while retrieving interaction details");
            }
        }

        // GET: Interactions/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                SetupCreateViewData();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up create interaction view");
                return Problem("An error occurred while setting up the create interaction page");
            }
        }

        // POST: Interactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateInteractionDto dto)
        {
            if (!ModelState.IsValid)
            {
                SetupCreateViewData();
                return View(dto);
            }

            try
            {
                var currentUser = _currentUserService.GetCurrentUsername();
                var interaction = new Interaction
                {
                    Date = dto.Date,
                    InteractionTypeId = dto.InteractionTypeId,
                    CustomerId = (int)dto.CustomerId,
                    Notes = dto.Notes,
                    Subject = dto.Subject,
                    Duration = dto.Duration
                };

                await _interactionService.CreateInteractionAsync(interaction, currentUser);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating interaction");
                ModelState.AddModelError("", "An error occurred while creating the interaction");
                SetupCreateViewData();
                return View(dto);
            }
        }

        // GET: Interactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var interaction = await _interactionService.GetInteractionDetailsAsync(id.Value);
                SetupEditViewData(interaction);

                var updateDto = new UpdateInteractionDto
                {
                    Id = interaction.Id,
                    Date = interaction.Date,
                    InteractionTypeId = interaction.InteractionTypeId,
                    CustomerId = interaction.CustomerId,
                    Notes = interaction.Notes,
                    Subject = interaction.Subject,
                    Duration = interaction.Duration
                };

                return View(updateDto);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving interaction for edit: {Id}", id);
                return Problem("An error occurred while retrieving interaction for editing");
            }
        }

        // POST: Interactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateInteractionDto dto)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    var interaction = await _interactionService.GetInteractionDetailsAsync(id);
                    SetupEditViewData(interaction);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error setting up edit view data for interaction: {Id}", id);
                }
                return View(dto);
            }

            try
            {
                var interactionToUpdate = new Interaction
                {
                    Id = dto.Id,
                    Date = dto.Date,
                    InteractionTypeId = dto.InteractionTypeId,
                    CustomerId = (int)dto.CustomerId,
                    Notes = dto.Notes,
                    Subject = dto.Subject,
                    Duration = dto.Duration
                };

                await _interactionService.UpdateInteractionAsync(id, interactionToUpdate);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating interaction: {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the interaction");
                try
                {
                    var interaction = await _interactionService.GetInteractionDetailsAsync(id);
                    SetupEditViewData(interaction);
                }
                catch (Exception setupEx)
                {
                    _logger.LogError(setupEx, "Error setting up edit view data after update failure for interaction: {Id}", id);
                }
                return View(dto);
            }
        }

        // GET: Interactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var interaction = await _interactionService.GetInteractionDetailsAsync(id.Value);
                return View(interaction);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving interaction for deletion: {Id}", id);
                return Problem("An error occurred while retrieving interaction for deletion");
            }
        }

        // POST: Interactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _interactionService.DeleteInteractionAsync(id);
                if (!result)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting interaction: {Id}", id);
                return Problem("An error occurred while deleting the interaction");
            }
        }

        private async void SetupCreateViewData()
        {
            try
            {
                ViewData["InteractionTypeId"] = new SelectList(await _interactionTypeService.GetInteractionTypesAsync(), "Id", "Name");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up create view data for interactions");
                throw;
            }
        }

        private async void SetupEditViewData(Interaction interaction)
        {
            try
            {
                ViewData["InteractionTypeId"] = new SelectList(await _interactionTypeService.GetInteractionTypesAsync(), "Id", "Name", interaction.InteractionTypeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up edit view data for interaction: {InteractionId}", interaction.Id);
                throw;
            }
        }
    }
}
