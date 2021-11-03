using System.Data;
using System;
using Microsoft.EntityFrameworkCore;

namespace GI.ControlePonto.Domain.Repository.Base
{
     public abstract class ContextBase<TContext> : DbContext, ICloneable where TContext : ContextBase<TContext>
    {
        private readonly DbContextOptionsBuilder<TContext> options;

        public ContextBase(DbContextOptionsBuilder<TContext> options) : base(options.Options) {
            this.options = options;
        }

        public object Clone() =>
            Activator.CreateInstance(typeof(TContext), options);
        

        public override void Dispose(){
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}