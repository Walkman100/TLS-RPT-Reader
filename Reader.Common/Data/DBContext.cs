using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Reader.Common.Data {
    public class DBContext : IdentityDbContext<IdentityUser> {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }
    }
}
