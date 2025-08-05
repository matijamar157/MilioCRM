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
    public class CompanyEmployeesController : Controller
    {
        private readonly ICompanyEmployeeService _companyEmployeeService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CompanyEmployeesController> _logger;

        public CompanyEmployeesController(
            ICompanyEmployeeService companyEmployeeService,
            ICurrentUserService currentUserService,
            ILogger<CompanyEmployeesController> logger)
        {
            _companyEmployeeService = companyEmployeeService ?? throw new ArgumentNullException(nameof(companyEmployeeService));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: CompanyEmployees
        public async Task<IActionResult> Index()
        {
            try
            {
                var companyEmployees = await _companyEmployeeService.GetAllCompanyEmployeesAsync();
                return View(companyEmployees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company employees");
                return Problem("An error occurred while retrieving company employees");
            }
        }

        // GET: CompanyEmployees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var companyEmployee = await _companyEmployeeService.GetCompanyEmployeeDetailsAsync(id.Value);
                return View(companyEmployee);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company employee details for ID: {Id}", id);
                return Problem("An error occurred while retrieving company employee details");
            }
        }

        // GET: CompanyEmployees/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: CompanyEmployees/WriteTheBoss
        public IActionResult WriteTheBoss()
        {
            return View();
        }

        // POST: CompanyEmployees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCompanyEmployeeDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var currentUser = _currentUserService.GetCurrentUsername();

                // Business logic: If company but no boss email, redirect to create company
                if (dto.Company && string.IsNullOrEmpty(dto.BossEmail))
                {
                    return RedirectToAction("Create", "Companies");
                }

                var companyEmployee = new CompanyEmployee
                {
                    Company = dto.Company,
                    Employee = dto.Employee,
                    BossEmail = dto.BossEmail
                };

                await _companyEmployeeService.CreateCompanyEmployeeAsync(companyEmployee, currentUser);
                return RedirectToAction("Index", "Customers");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company employee");
                ModelState.AddModelError("", "An error occurred while creating the company employee");
                return View(dto);
            }
        }

        // POST: CompanyEmployees/WriteTheBoss
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WriteTheBoss(CreateBossRequestDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var currentUser = _currentUserService.GetCurrentUsername();
                var companyEmployee = new CompanyEmployee
                {
                    Company = dto.Company,
                    Employee = dto.Employee,
                    BossEmail = dto.BossEmail
                };

                await _companyEmployeeService.CreateBossRequestAsync(companyEmployee, currentUser);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating boss request");
                ModelState.AddModelError("", "An error occurred while creating the boss request");
                return View(dto);
            }
        }

        // GET: CompanyEmployees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var companyEmployee = await _companyEmployeeService.GetCompanyEmployeeDetailsAsync(id.Value);
                var updateDto = new UpdateCompanyEmployeeDto
                {
                    Company = companyEmployee.Company,
                    Employee = companyEmployee.Employee,
                    BossEmail = companyEmployee.BossEmail,
                    Email = companyEmployee.Email
                };
                return View(updateDto);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company employee for edit: {Id}", id);
                return Problem("An error occurred while retrieving company employee for editing");
            }
        }

        // POST: CompanyEmployees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCompanyEmployeeDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var companyEmployee = new CompanyEmployee
                {
                    Company = dto.Company,
                    Employee = dto.Employee,
                    BossEmail = dto.BossEmail,
                    Email = dto.Email
                };

                await _companyEmployeeService.UpdateCompanyEmployeeAsync(id, companyEmployee);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company employee: {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the company employee");
                return View(dto);
            }
        }

        // GET: CompanyEmployees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var companyEmployee = await _companyEmployeeService.GetCompanyEmployeeDetailsAsync(id.Value);
                return View(companyEmployee);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company employee for deletion: {Id}", id);
                return Problem("An error occurred while retrieving company employee for deletion");
            }
        }

        // POST: CompanyEmployees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _companyEmployeeService.DeleteCompanyEmployeeAsync(id);
                if (!result)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company employee: {Id}", id);
                return Problem("An error occurred while deleting the company employee");
            }
        }
    }
}
