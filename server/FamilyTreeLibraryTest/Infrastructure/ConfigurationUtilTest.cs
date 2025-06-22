using FamilyTreeLibrary.Infrastructure;
using FamilyTreeLibrary.Infrastructure.Resource;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyTreeLibraryTest.Infrastructure
{
    public class ConfigurationUtilTest
    {
        private IServiceCollection services;
        [SetUp]
        public void Setup()
        {
            services = new ServiceCollection().AddFamilyTreeConfiguration();
        }

        [Test]
        public void TestAppInsightsName()
        {
            ServiceProvider provider = services.BuildServiceProvider();
            FamilyTreeConfiguration configuration = provider.GetRequiredService<FamilyTreeConfiguration>();
            Assert.That(configuration["ApplicationInsights:Name"], Is.EqualTo("familyTreeInsights"));
        }
    }
}