using AFuturaCRMV2.Interfaces;
using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICompanyResolutionService _companyResolutionService;
        private readonly ILookupService _lookupService;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(
            IDocumentRepository documentRepository,
            IFileStorageService fileStorageService,
            ICompanyResolutionService companyResolutionService,
            ILookupService lookupService,
            ILogger<DocumentService> logger)
        {
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _companyResolutionService = companyResolutionService ?? throw new ArgumentNullException(nameof(companyResolutionService));
            _lookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Documents>> GetUserDocumentsAsync(string userEmail)
        {
            return await _documentRepository.GetDocumentsByUserAsync(userEmail);
        }

        public async Task<string> GetDocumentUrlAsync(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
            {
                throw new InvalidOperationException($"Document with ID {id} not found");
            }

            // Extract filename from AuthorEmail (legacy format)
            var parts = document.AuthorEmail.Split('-');
            var filename = ExtractFilenameFromParts(parts);

            return _fileStorageService.GetFileUrl(filename, "documents");
        }

        public async Task<Documents> CreateDocumentAsync(Documents document, IFormFile file, string currentUserEmail)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file uploaded");
            }

            var filename = await _fileStorageService.UploadFileAsync(file, "documents");
            document.AuthorEmail = $"{currentUserEmail}-{filename}";
            document.DateUploaded = DateTime.UtcNow;

            return await _documentRepository.CreateAsync(document);
        }

        public async Task<Documents> UpdateDocumentAsync(int id, Documents document)
        {
            var existingDocument = await _documentRepository.GetByIdAsync(id);
            if (existingDocument == null)
            {
                throw new InvalidOperationException($"Document with ID {id} not found");
            }

            existingDocument.Title = document.Title;
            existingDocument.DocumentTypeId = document.DocumentTypeId;
            existingDocument.Version = document.Version;
            existingDocument.Comment = document.Comment;

            return await _documentRepository.UpdateAsync(existingDocument);
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            return await _documentRepository.DeleteAsync(id);
        }

        public async Task<bool> CanCreateDocumentAsync(string userEmail)
        {
            var company = await _companyResolutionService.GetUserCompanyAsync(userEmail);
            if (company == null) return true; // Allow if no company

            var customers = await _lookupService.GetCompanyCustomersAsync(company.Id);
            return customers.Any();
        }

        private string ExtractFilenameFromParts(string[] parts)
        {
            var finalString = "";
            for (int i = 0; i < parts.Length - 2; i++)
            {
                if (!parts[i].EndsWith("eg"))
                {
                    finalString += parts[i + 1] + "-";
                }
            }
            finalString += parts.Last();
            return finalString;
        }
    }
}
