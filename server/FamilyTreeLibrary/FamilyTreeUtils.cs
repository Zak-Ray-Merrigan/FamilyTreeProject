using FamilyTreeLibrary.Data.Models;
using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FamilyTreeLibrary
{
    public static class FamilyTreeUtils
    {
        public static ILoggingBuilder AddFamilyTreeLogger(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, FamilyTreeLoggerProvider>(sp =>
            {
                return new(sp.GetRequiredService<FamilyTreeVault>());
            });
            builder.Services.AddTransient(typeof(IExtendedLogger<>), typeof(FamilyTreeLogger<>));
            return builder;
        }

        internal static void ValidateExtendedAttributeAccessibility(IEnumerable<string> requiredAttributes, string attribute)
        {
            if (requiredAttributes.Any((requiredAttribute) => requiredAttribute.Equals(attribute, StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new InvalidAttributeException(attribute);
            }
        }

        internal static Person GetPerson(IExtendedLogger logger, InheritedFamilyName inheritedFamilyName, ISet<Person> people, Person p)
        {
            logger.LogDebug("Since every person is unique, we are ensuring uniqueness while consider re-marriages.");
            if (people.Add(p))
            {
                logger.LogDebug("{person} is part of the {familyName} family.", p, inheritedFamilyName.Name);
                return p;
            }
            logger.LogDebug("Finding existing person.");
            return people.First((temp) => temp == p);
        }
    }
}