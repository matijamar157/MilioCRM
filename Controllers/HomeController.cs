using AFuturaCRMV2.Data;
using AFuturaCRMV2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Xml.Linq;

namespace AFuturaCRMV2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult ActivateBasicStripe()
        {
            return Redirect("https://buy.stripe.com/6oEcQd4cA8jq8ZWbKl");
        }

        public IActionResult ActivatePremiumStripe()
        {
            return Redirect("https://buy.stripe.com/14k7vT4cA0QY4JG9Ce");
        }

        public IActionResult ActivateGoldStripe()
        {
            return Redirect("https://buy.stripe.com/aEU4jH38w43a5NK15J");
        }

        public IActionResult AllUsers()
        {
            var users = _context.Users.ToList();
            var clients = _context.Customers.ToList();
            return View(users);
        }

        public IActionResult Index()
        {
            if (User.Identity.Name != null)
            {
                var companyExist = _context.Company.Where(x => x.Username == User.Identity.Name).ToList();
                List<CompanyEmployee> listOfCompanyEmployees = new List<CompanyEmployee>();
                if (companyExist.Count == 0)
                {
                    listOfCompanyEmployees = _context.CompanyEmployees.Where(x => x.Email == User.Identity.Name).ToList();
                }

                if (listOfCompanyEmployees.Count == 0 && companyExist.Count == 0)
                {
                    return RedirectToAction("Create", "CompanyEmployees");
                }
                else
                {
                    return RedirectToAction("Index", "Customers");
                }
            }
              
            bool doesCustomerStatusExist = _context.CustomerStatuses.Any();
            if (!doesCustomerStatusExist)
            {
                List<CustomerStatus> lista = new List<CustomerStatus>();

                CustomerStatus active = new CustomerStatus()
                {
                    Name = "Active"
                };

                CustomerStatus inactive = new CustomerStatus()
                {
                    Name = "Inactive"
                };

                lista.Add(active);
                lista.Add(inactive);
                _context.CustomerStatuses.AddRange(lista);
                _context.SaveChanges();
            }

            bool doesCustomerTypeExist = _context.CustomerTypes.Any();
            if (!doesCustomerTypeExist)
            {
                List<CustomerType> lista = new List<CustomerType>();

                CustomerType lead = new CustomerType()
                {
                    Name = "Lead"
                };

                CustomerType prospect = new CustomerType()
                {
                    Name = "Prospect"
                };

                CustomerType finishedDocumentation = new CustomerType()
                {
                    Name = "Finished documentation"
                };

                CustomerType customer = new CustomerType()
                {
                    Name = "Customer"
                };

                lista.Add(lead);
                lista.Add(prospect);
                lista.Add(finishedDocumentation);
                lista.Add(customer);
                _context.CustomerTypes.AddRange(lista);
                _context.SaveChanges();
            }

            bool doesDocumentTypeExist = _context.DocumentTypes.Any();
            if (!doesDocumentTypeExist)
            {
                List<DocumentType> lista = new List<DocumentType>();

                DocumentType contract = new DocumentType()
                {
                    Type = "Contract"
                };

                DocumentType proposal = new DocumentType()
                {
                    Type = "Proposal"
                };

                DocumentType invoice = new DocumentType()
                {
                    Type = "Invoice"
                };

                DocumentType other = new DocumentType()
                {
                    Type = "Other"
                };

                lista.Add(contract);
                lista.Add(proposal);
                lista.Add(invoice);
                lista.Add(other);
                _context.DocumentTypes.AddRange(lista);
                _context.SaveChanges();
            }

            bool doesInteractionTypeExist = _context.InteractionTypes.Any();
            if (!doesInteractionTypeExist)
            {
                List<InteractionType> lista = new List<InteractionType>();

                InteractionType call = new InteractionType()
                {
                    Name = "Call"
                };

                InteractionType email = new InteractionType()
                {
                    Name = "Email"
                };

                InteractionType meeting = new InteractionType()
                {
                    Name = "Meeting"
                };

                lista.Add(call);
                lista.Add(email);
                lista.Add(meeting);
                _context.InteractionTypes.AddRange(lista);
                _context.SaveChanges();
            }

            return View();
        }

        public IActionResult Packages()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}