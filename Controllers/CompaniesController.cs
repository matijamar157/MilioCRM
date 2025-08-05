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
    public class CompaniesController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(
            ICompanyService companyService,
            ICurrentUserService currentUserService,
            ILogger<CompaniesController> logger)
        {
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            try
            {
                var username = _currentUserService.GetCurrentUsername();
                var companies = await _companyService.GetUserCompaniesAsync(username);
                return View(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving companies for user");
                return Problem("An error occurred while retrieving companies");
            }
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var company = await _companyService.GetCompanyDetailsAsync(id.Value);
                return View(company);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company details for ID: {CompanyId}", id);
                return Problem("An error occurred while retrieving company details");
            }
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCompanyDto companyDto)
        {
            if (!ModelState.IsValid)
                return View(companyDto);

            try
            {
                var username = _currentUserService.GetCurrentUsername();
                var company = new Company
                {
                    Name = companyDto.Name,
                    Address = companyDto.Address,
                    Country = companyDto.Country
                };

                await _companyService.CreateCompanyAsync(company, username);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                ModelState.AddModelError("", "An error occurred while creating the company");
                return View(companyDto);
            }
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var company = await _companyService.GetCompanyDetailsAsync(id.Value);
                var updateDto = new UpdateCompanyDto
                {
                    Name = company.Name,
                    Address = company.Address,
                    Country = company.Country
                };
                return View(updateDto);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company for edit: {CompanyId}", id);
                return Problem("An error occurred while retrieving company for editing");
            }
        }

        // POST: Companies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCompanyDto companyDto)
        {
            if (!ModelState.IsValid)
                return View(companyDto);

            try
            {
                var username = _currentUserService.GetCurrentUsername();
                var company = new Company
                {
                    Name = companyDto.Name,
                    Address = companyDto.Address,
                    Country = companyDto.Country
                };

                await _companyService.UpdateCompanyAsync(id, company, username);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company: {CompanyId}", id);
                ModelState.AddModelError("", "An error occurred while updating the company");
                return View(companyDto);
            }
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var company = await _companyService.GetCompanyDetailsAsync(id.Value);
                return View(company);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company for deletion: {CompanyId}", id);
                return Problem("An error occurred while retrieving company for deletion");
            }
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _companyService.DeleteCompanyAsync(id);
                if (!result)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company: {CompanyId}", id);
                return Problem("An error occurred while deleting the company");
            }
        }
    }
}
