using GuestSystemBack.Models;
using Microsoft.EntityFrameworkCore;

namespace GuestSystemBack.Data
{
    public class GuestSystemContext : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<FormSubmission> FormSubmissions { get; set; }
        public DbSet<VisitableEmployee> VisitableEmployees { get; set; }
        public DbSet<ExtraDocument> ExtraDocuments { get; set; }
        public DbSet<FormDocument> FormDocuments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GuestSystem;Integrated Security=True;");
        }
    }
}
