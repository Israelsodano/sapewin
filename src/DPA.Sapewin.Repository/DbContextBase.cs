using DPA.Sapewin.Repository.Internals.Event;
using Microsoft.EntityFrameworkCore;

namespace DPA.Sapewin.Repository
{
    public class DbContextBase<TContext> : DbContext where TContext : DbContextBase<TContext>
    {
        public DbContextBase(DbContextOptions<TContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HeadEvent>()
                        .HasMany(x=> x.Fields)
                            .WithOne(x=> x.HeadEvent)
                            .HasForeignKey(x=> x.HeadEventId);
        }
    }
}