using System;
using DPA.Sapewin.Repository.Internals.Event;
using Microsoft.Extensions.DependencyInjection;

namespace DPA.Sapewin.Repository.Extenssions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection ConfigureRepository(IServiceCollection s, Action<IRepositoryConfiguration> ac)
        {
            var r = new RepositoryConfiguration();
            ac(r);
            s.AddScoped<IEventStore, EventStore>();
            foreach (var i in r.Injections) s.AddScoped(i.i, i.c);

            return s;
        } 
    }
}