using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class SocialContext: DbContext
    {
        public SocialContext(DbContextOptions<SocialContext> options) : base(options)
        {

        }

        //Veri tabanına tablo ekleme
        public DbSet<Product> Products { get; set; }
    }
}
