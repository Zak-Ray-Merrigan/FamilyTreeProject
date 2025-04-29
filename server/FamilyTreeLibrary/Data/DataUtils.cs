using FamilyTreeLibrary.Data.Databases;
using FamilyTreeLibrary.Data.Files;
using FamilyTreeLibrary.Infrastructure.Resource;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyTreeLibrary.Data
{
    public static class DataUtils
    {
        public static IServiceCollection AddFamilyDynamicCollection(this IServiceCollection services)
        {
            return services.AddSingleton((provider) =>
            {
                FamilyTreeConfiguration configuration = provider.GetRequiredService<FamilyTreeConfiguration>();
                FamilyTreeVault vault = provider.GetRequiredService<FamilyTreeVault>();
                return new FamilyDynamicCollection(configuration, vault);
            });
        }

        public static IServiceCollection AddFamilyTreeGraph(this IServiceCollection services)
        {
            return services.AddSingleton((provider) =>
            {
                FamilyTreeConfiguration configuration = provider.GetRequiredService<FamilyTreeConfiguration>();
                FamilyTreeVault vault = provider.GetRequiredService<FamilyTreeVault>();
                return new FamilyTreeGraph(configuration, vault);
            });
        }
        public static IServiceCollection AddFamilyTreeStaticStorage(this IServiceCollection services)
        {
            return services.AddSingleton((provider) =>
            {
                FamilyTreeConfiguration configuration = provider.GetRequiredService<FamilyTreeConfiguration>();
                FamilyTreeVault vault = provider.GetRequiredService<FamilyTreeVault>();
                return new FamilyTreeStaticStorage(configuration, vault);
            });
        }
        
        public static IServiceCollection AddPersonCollection(this IServiceCollection services)
        {
            return services.AddSingleton((provider) =>
            {
                FamilyTreeConfiguration configuration = provider.GetRequiredService<FamilyTreeConfiguration>();
                FamilyTreeVault vault = provider.GetRequiredService<FamilyTreeVault>();
                return new PersonCollection(configuration, vault);
            });
        }

        internal static void ValidateRequiredAttribute(IEnumerable<string> attributes, string requiredAttribute)
        {
            if (!attributes.Contains(requiredAttribute))
            {
                throw new MissingRequiredAttributeException(requiredAttribute);
            }
        }

        internal static void ValidateRequiredAttributes(IEnumerable<string> requiredAttributes, IEnumerable<string> attributes)
        {
            IEnumerable<string> missingAttributes = requiredAttributes.Except(attributes);
            if (missingAttributes.Any())
            {
                throw new MissingRequiredAttributeException(missingAttributes);
            }
        }
    }
}