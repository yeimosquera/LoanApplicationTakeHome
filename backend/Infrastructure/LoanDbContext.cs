using Microsoft.EntityFrameworkCore;
using LoanApplication.Api.Domain.Customers;
using DomainLoan = LoanApplication.Api.Domain.Loans.Application;

namespace LoanApplication.Api.Infrastructure;

public sealed class LoanDbContext : DbContext
{
    public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<DomainLoan> Applications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración mínima: relación Application -> Customer
        modelBuilder.Entity<DomainLoan>()
            .HasOne(a => a.Customer)
            .WithMany()
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}