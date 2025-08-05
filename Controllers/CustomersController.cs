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
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILookupService _lookupService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(
            ICustomerService customerService,
            ICurrentUserService currentUserService,
            ILookupService lookupService,
            ILogger<CustomersController> logger)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _lookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUser = _currentUserService.GetCurrentUsername();

                // Check customer limit (business rule: max 10 customers)
                var exceedsLimit = await _customerService.CheckCustomerLimitAsync(currentUser);
                if (exceedsLimit)
                {
                    return RedirectToAction("Packages", "Home");
                }

                var customers = await _customerService.GetUserCustomersAsync(currentUser);
                return View(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers for user: {User}", _currentUserService.GetCurrentUsername());
                return Problem("An error occurred while retrieving customers");
            }
        }

        // GET: Customers/IndexPriority
        public async Task<IActionResult> IndexPriority()
        {
            try
            {
                var currentUser = _currentUserService.GetCurrentUsername();
                var customers = await _customerService.GetUserCustomersAsync(currentUser, priorityOnly: true);
                return View(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving priority customers for user: {User}", _currentUserService.GetCurrentUsername());
                return Problem("An error occurred while retrieving priority customers");
            }
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var customer = await _customerService.GetCustomerDetailsAsync(id.Value);
                return View(customer);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer details for ID: {Id}", id);
                return Problem("An error occurred while retrieving customer details");
            }
        }

        // GET: Customers/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                SetupCreateViewData();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up create customer view");
                return Problem("An error occurred while setting up the create customer page");
            }
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCustomerDto dto)
        {
            if (!ModelState.IsValid)
            {
                SetupCreateViewData();
                return View(dto);
            }

            try
            {
                var currentUser = _currentUserService.GetCurrentUsername();
                var customer = new Customer
                {
                    Name = dto.Name,
                    Country = dto.Country,
                    Owner = dto.Owner,
                    TelephoneNumber = dto.TelephoneNumber,
                    CustomerTypeId = dto.CustomerTypeId,
                    CustomerStatusId = dto.CustomerStatusId,
                    DealValue = (int)dto.DealValue,
                    Website = dto.Website,
                    DateOfLatestMeeting = (DateTime)dto.DateOfLatestMeeting,
                    PriorityLead = dto.PriorityLead
                };

                await _customerService.CreateCustomerAsync(customer, currentUser);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                ModelState.AddModelError("", "An error occurred while creating the customer");
                SetupCreateViewData();
                return View(dto);
            }
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var customer = await _customerService.GetCustomerDetailsAsync(id.Value);
                SetupEditViewData(customer);

                var updateDto = new UpdateCustomerDto
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Country = customer.Country,
                    Owner = customer.Owner,
                    TelephoneNumber = customer.TelephoneNumber,
                    EmailAddress = customer.EmailAddress,
                    CustomerTypeId = customer.CustomerTypeId,
                    CustomerStatusId = customer.CustomerStatusId,
                    DealValue = customer.DealValue,
                    Website = customer.Website,
                    DateOfLatestMeeting = customer.DateOfLatestMeeting,
                    PriorityLead = customer.PriorityLead
                };

                return View(updateDto);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer for edit: {Id}", id);
                return Problem("An error occurred while retrieving customer for editing");
            }
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCustomerDto dto, string companyId)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    var customer = await _customerService.GetCustomerDetailsAsync(id);
                    SetupEditViewData(customer);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error setting up edit view data for customer: {Id}", id);
                }
                return View(dto);
            }

            try
            {
                var customerToUpdate = new Customer
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Country = dto.Country,
                    Owner = dto.Owner,
                    TelephoneNumber = dto.TelephoneNumber,
                    EmailAddress = dto.EmailAddress,
                    CustomerTypeId = dto.CustomerTypeId,
                    CustomerStatusId = dto.CustomerStatusId,
                    DealValue = (int)dto.DealValue,
                    Website = dto.Website,
                    DateOfLatestMeeting = (DateTime)dto.DateOfLatestMeeting,
                    PriorityLead = dto.PriorityLead
                };

                var companyIdInt = Convert.ToInt32(companyId);
                await _customerService.UpdateCustomerAsync(id, customerToUpdate, companyIdInt);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer: {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the customer");
                try
                {
                    var customer = await _customerService.GetCustomerDetailsAsync(id);
                    SetupEditViewData(customer);
                }
                catch (Exception setupEx)
                {
                    _logger.LogError(setupEx, "Error setting up edit view data after update failure for customer: {Id}", id);
                }
                return View(dto);
            }
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            try
            {
                var customer = await _customerService.GetCustomerDetailsAsync(id.Value);
                return View(customer);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer for deletion: {Id}", id);
                return Problem("An error occurred while retrieving customer for deletion");
            }
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _customerService.DeleteCustomerAsync(id);
                if (!result)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer: {Id}", id);
                return Problem("An error occurred while deleting the customer");
            }
        }

        private async void SetupCreateViewData()
        {
            ViewData["CustomerStatusId"] = new SelectList(await _lookupService.GetCustomerStatusesAsync(), "Id", "Name");
            ViewData["CustomerTypeId"] = new SelectList(await _lookupService.GetCustomerTypesAsync(), "Id", "Name");
        }

        private async void SetupEditViewData(Customer customer)
        {
            ViewBag.CompanyId = customer.CompanyId;
            ViewData["CustomerStatusId"] = new SelectList(await _lookupService.GetCustomerStatusesAsync(), "Id", "Name", customer.CustomerStatusId);
            ViewData["CustomerTypeId"] = new SelectList(await _lookupService.GetCustomerTypesAsync(), "Id", "Name", customer.CustomerTypeId);
        }
    }
}
