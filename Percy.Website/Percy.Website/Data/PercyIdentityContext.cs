using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Percy.Website.Data
{
    public class PercyIdentityContext
        : IdentityDbContext<IdentityUser>
    {
        public PercyIdentityContext(
            DbContextOptions<PercyIdentityContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("auth");
        }
    }
}