using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Reader.Common.Data {
    public class DBContext : IdentityDbContext<IdentityUser> {
        public DbSet<Models.Report> Reports { get; set; }
        public DbSet<Models.ReportPolicy> ReportPolicies { get; set; }

        public DBContext(DbContextOptions<DBContext> options) : base(options) { }
    }
}
