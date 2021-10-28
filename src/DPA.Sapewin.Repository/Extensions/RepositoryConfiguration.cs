using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository.Internals.Event;

namespace DPA.Sapewin.Repository.Extensions
{ 
    public interface IRepositoryConfiguration
    {
        IRepositoryConfiguration AddEntityDomain<TEntity>() where TEntity : Entity;
    }

    internal class RepositoryConfiguration : IRepositoryConfiguration
    {
        public RepositoryConfiguration() => 
            Injections = new List<(Type, Type)>();

        internal IList<(Type i, Type c)> Injections { get; }
        
        public IRepositoryConfiguration AddEntityDomain<TEntity>() where TEntity : Entity
        {
            Injections.Add((typeof(IUnityOfWork<TEntity>), typeof(UnityOfWork<TEntity>)));
            Injections.Add((typeof(IRepository<TEntity>), typeof(Internals.Repository<TEntity>)));
            return this;
        }
    }
}