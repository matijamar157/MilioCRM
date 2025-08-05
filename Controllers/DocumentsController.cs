using AFuturaCRMV2.Data;
using AFuturaCRMV2.Interfaces;
using AFuturaCRMV2.Models;
using AFuturaCRMV2.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AFuturaCRMV2.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILookupService _lookupService;
        private readonly ICompanyResolutionService _companyResolutionService;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(
            IDocumentService documentService,
            ICurrentUserService currentUserService,
            ILookupService lookupService,
            ICompanyResolutionService companyResolutionService,
            ILogger<DocumentsController> logger)
        {
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _lookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            _companyResolutionService = companyResolutionService ?? throw new ArgumentNullException(nameof(companyResolutionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUser = _currentUserService.GetCurrentUsername();
                var documents = await _documentService.GetUserDocumentsAsync(currentUser);
                return View(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents for user: {User}", _currentUserService.GetCurrentUsername());
                return Problem("An error occurred while retrieving documents");
            }
        }

        // GET: Documents/Details/5 - Returns document URL for download/view
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var documentUrl = await _documentService.GetDocumentUrlAsync(id.Value);
                return Redirect(documentUrl);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document URL for ID: {Id}", id);
                return Problem("An error occurred while retrieving the document");
            }
        }

        // GET: Documents/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var currentUser = _currentUserService.GetCurrentUsername();

                // Business rule: Must have customers before creating documents
                var canCreate = await _documentService.CanCreateDocumentAsync(currentUser);
                if (!canCreate)
                {
                    return RedirectToAction("Create", "Customers");
                }

                SetupCreateViewData(currentUser);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up create document view for user: {User}", _currentUserService.GetCurrentUsername());
                return Problem("An error occurred while setting up the create document page");
            }
        }

        // POST: Documents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDocumentDto dto, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                var currentUser = _currentUserService.GetCurrentUsername();
                SetupCreateViewData(currentUser);
                return View(dto);
            }

            try
            {
                if (file == null || file.Length == 0)
                {
                    ModelState.AddModelError("", "Please select a file to upload");
                    var currentUser = _currentUserService.GetCurrentUsername();
                    SetupCreateViewData(currentUser);
                    return View(dto);
                }

                var currentUserEmail = _currentUserService.GetCurrentUsername();
                var document = new Documents
                {
                    Title = dto.Title,
                    CustomerId = dto.CustomerId,
                    DocumentTypeId = dto.DocumentTypeId,
                    Version = Convert.ToInt32(dto.Version),
                    Comment = dto.Comment
                };

                await _documentService.CreateDocumentAsync(document, file, currentUserEmail);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document");
                ModelState.AddModelError("", "An error occurred while creating the document");
                var currentUser = _currentUserService.GetCurrentUsername();
                SetupCreateViewData(currentUser);
                return View(dto);
            }
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var documents = await _documentService.GetUserDocumentsAsync(_currentUserService.GetCurrentUsername());
                var document = documents.FirstOrDefault(d => d.Id == id.Value);

                if (document == null)
                    return NotFound();

                SetupEditViewData(document);

                var updateDto = new UpdateDocumentDto
                {
                    Title = document.Title,
                    DocumentTypeId = document.DocumentTypeId,
                    Version = document.Version.ToString(),
                    Comment = document.Comment
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document for edit: {Id}", id);
                return Problem("An error occurred while retrieving document for editing");
            }
        }

        // POST: Documents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateDocumentDto dto)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    var documents = await _documentService.GetUserDocumentsAsync(_currentUserService.GetCurrentUsername());
                    var document = documents.FirstOrDefault(d => d.Id == id);
                    if (document != null)
                    {
                        SetupEditViewData(document);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error setting up edit view data for document: {Id}", id);
                }
                return View(dto);
            }

            try
            {
                var documentToUpdate = new Documents
                {
                    Title = dto.Title,
                    DocumentTypeId = dto.DocumentTypeId,
                    Version = Convert.ToInt32(dto.Version),
                    Comment = dto.Comment
                };

                await _documentService.UpdateDocumentAsync(id, documentToUpdate);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document: {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the document");
                try
                {
                    var documents = await _documentService.GetUserDocumentsAsync(_currentUserService.GetCurrentUsername());
                    var document = documents.FirstOrDefault(d => d.Id == id);
                    if (document != null)
                    {
                        SetupEditViewData(document);
                    }
                }
                catch (Exception setupEx)
                {
                    _logger.LogError(setupEx, "Error setting up edit view data after update failure for document: {Id}", id);
                }
                return View(dto);
            }
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var documents = await _documentService.GetUserDocumentsAsync(_currentUserService.GetCurrentUsername());
                var document = documents.FirstOrDefault(d => d.Id == id.Value);

                if (document == null)
                    return NotFound();

                return View(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document for deletion: {Id}", id);
                return Problem("An error occurred while retrieving document for deletion");
            }
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _documentService.DeleteDocumentAsync(id);
                if (!result)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document: {Id}", id);
                return Problem("An error occurred while deleting the document");
            }
        }

        private async void SetupCreateViewData(string currentUser)
        {
            try
            {
                var company = await _companyResolutionService.GetUserCompanyAsync(currentUser);
                if (company != null)
                {
                    var customers = await _lookupService.GetCompanyCustomersAsync(company.Id);
                    ViewData["CustomerId"] = new SelectList(customers, "Id", "Name");
                }

                ViewData["DocumentTypeId"] = new SelectList(await _lookupService.GetDocumentTypesAsync(), "Id", "Type");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up create view data for user: {User}", currentUser);
                throw;
            }
        }

        private async void SetupEditViewData(Documents document)
        {
            try
            {
                ViewData["DocumentTypeId"] = new SelectList(await _lookupService.GetDocumentTypesAsync(), "Id", "Type", document.DocumentTypeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up edit view data for document: {DocumentId}", document.Id);
                throw;
            }
        }
    }
}