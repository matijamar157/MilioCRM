using AFuturaCRMV2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AFuturaCRMV2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerStatus>().HasData(
                new CustomerStatus { Id = 1, Name = "Active" },
                new CustomerStatus { Id = 2, Name = "Inactive" }
            );

            modelBuilder.Entity<CustomerType>().HasData(
                new CustomerType { Id = 1, Name = "Lead" }, new CustomerType { Id = 2, Name = "Prospect" }, new CustomerType { Id = 3, Name = "Need to contact" },
                new CustomerType { Id = 4, Name = "Finished documentation" }, new CustomerType { Id = 5, Name = "Customer" }
            );
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<CustomerStatus> CustomerStatuses { get; set; }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<AFuturaCRMV2.Models.Company>? Company { get; set; }
        public DbSet<Interaction> Interaction { get; set; }
        public DbSet<InteractionType> InteractionTypes { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<AffiliateUser> AffiliateUsers { get; set; }
        public DbSet<CompanyEmployee> CompanyEmployees { get; set; }
    }
}