using AFuturaCRMV2.Interfaces;
using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Services
{
    public class InteractionService : IInteractionService
    {
        private readonly IInteractionRepository _interactionRepository;
        private readonly ILogger<InteractionService> _logger;

        public InteractionService(
            IInteractionRepository interactionRepository,
            ILogger<InteractionService> logger)
        {
            _interactionRepository = interactionRepository ?? throw new ArgumentNullException(nameof(interactionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Interaction>> GetAllInteractionsAsync()
        {
            _logger.LogInformation("Retrieving all interactions");
            return await _interactionRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Interaction>> GetUserInteractionsAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new ArgumentException("User email is required", nameof(userEmail));

            _logger.LogInformation("Retrieving interactions for user: {UserEmail}", userEmail);
            return await _interactionRepository.GetByUserAsync(userEmail);
        }

        public async Task<IEnumerable<Interaction>> GetCustomerInteractionsAsync(int customerId)
        {
            _logger.LogInformation("Retrieving interactions for customer: {CustomerId}", customerId);
            return await _interactionRepository.GetByCustomerAsync(customerId);
        }

        public async Task<Interaction> GetInteractionDetailsAsync(int id)
        {
            _logger.LogInformation("Retrieving interaction details for ID: {InteractionId}", id);
            var interaction = await _interactionRepository.GetByIdAsync(id);

            if (interaction == null)
            {
                _logger.LogWarning("Interaction not found with ID: {InteractionId}", id);
                throw new InvalidOperationException($"Interaction with ID {id} not found");
            }

            return interaction;
        }

        public async Task<Interaction> CreateInteractionAsync(Interaction interaction, string currentUserEmail)
        {
            if (interaction == null)
                throw new ArgumentNullException(nameof(interaction));

            if (string.IsNullOrEmpty(currentUserEmail))
                throw new ArgumentException("Current user email is required", nameof(currentUserEmail));

            _logger.LogInformation("Creating new interaction for user: {UserEmail}", currentUserEmail);
            return await _interactionRepository.CreateAsync(interaction);
        }

        public async Task<Interaction> UpdateInteractionAsync(int id, Interaction interaction)
        {
            if (interaction == null)
                throw new ArgumentNullException(nameof(interaction));

            var existingInteraction = await _interactionRepository.GetByIdAsync(id);
            if (existingInteraction == null)
            {
                _logger.LogWarning("Attempted to update non-existent interaction with ID: {InteractionId}", id);
                throw new InvalidOperationException($"Interaction with ID {id} not found");
            }

            // Update properties
            existingInteraction.Date = interaction.Date;
            existingInteraction.InteractionTypeId = interaction.InteractionTypeId;
            existingInteraction.CustomerId = interaction.CustomerId;
            existingInteraction.Notes = interaction.Notes;
            existingInteraction.Subject = interaction.Subject;
            existingInteraction.Duration = interaction.Duration;

            _logger.LogInformation("Updating interaction: {InteractionId}", id);
            return await _interactionRepository.UpdateAsync(existingInteraction);
        }

        public async Task<bool> DeleteInteractionAsync(int id)
        {
            _logger.LogInformation("Deleting interaction with ID: {InteractionId}", id);
            var result = await _interactionRepository.DeleteAsync(id);

            if (!result)
            {
                _logger.LogWarning("Attempted to delete non-existent interaction with ID: {InteractionId}", id);
            }

            return result;
        }
    }
}
