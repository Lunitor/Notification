using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Lunitor.Notification.Shared
{
    public static class DependencyRegistrarExtensions
    {
        public static void AddApplicationDependencies(this IServiceCollection services)
        {
            var dependencyRegistrarTypes = new List<Type>();

            var assemblies = new List<Assembly>();

            foreach (string assemblyPath in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.AllDirectories))
            {
                var assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                assemblies.Add(assembly);
            }


            foreach (var assembly in assemblies)
            {
                dependencyRegistrarTypes.AddRange(assembly.GetTypes()
                    .Where(type => type.BaseType == typeof(DependencyRegistrar)));
            }

            foreach (var dependencyRegistrarType in dependencyRegistrarTypes)
            {
                var dependencyRegistrar = Activator.CreateInstance(dependencyRegistrarType) as DependencyRegistrar;
                dependencyRegistrar.RegisterDependencies(services);
            }
        }
    }
}
