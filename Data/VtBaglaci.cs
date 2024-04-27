using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BilirkisiMvc.Data
{
    public class VtBaglaci(DbContextOptions<VtBaglaci> options) : IdentityDbContext<IdentityUser>(options)
    {

    }
}