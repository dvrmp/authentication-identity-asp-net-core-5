using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TodoApp.Models
{
    public class ApplicationDatabaseContext: IdentityDbContext<ApplicationsUser>
    {
        public ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options): base(options)
        {
            
        }
    }
}