using GI.ControlePonto.Domain.Entities;
using GI.ControlePonto.Domain.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GI.ControlePonto.Repository.Contexts
{
    public class DpaRepContext : ContextBase<DpaRepContext>
    {
        public DpaRepContext(DbContextOptionsBuilder<DpaRepContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.Entity<ApontamentoSujo>().HasKey(x=> x.chaveUniqueMarc);
        }

        public virtual DbSet<ApontamentoSujo> ApontamentosSujos { get; set; }
    }
}