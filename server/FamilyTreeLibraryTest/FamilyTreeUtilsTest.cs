using FamilyTreeLibrary;
using FamilyTreeLibrary.Models;
using Microsoft.Extensions.Configuration;

namespace FamilyTreeLibraryTest
{
    public class FamilyTreeUtilsTest
    {
        internal const string PDF_FILE = @"C:\FamilyTreeProject\resources\PfingstenBook2023.pdf";

        [Test]
        public void TestGetConfiguration()
        {
            FamilyTreeDbSettings result;
            try
            {
                IConfigurationRoot configuration = FamilyTreeUtils.GetConfiguration();
                result = configuration.GetSection("FamilyTreeDb").Get<FamilyTreeDbSettings>();
            }
            catch (Exception ex)
            {
                result = null;
                Assert.Fail($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            }
            Assert.Pass(result.ToString());
        }

        [Test]
        public void TestInitializeLogger()
        {
            try
            {
                FamilyTreeUtils.InitializeLogger();
                FamilyTreeUtils.LogMessage(LoggingLevels.Information,"Logging Something.");
                string expectedLogPath = @"C:\FamilyTreeProject\resources\Logs\log20240316.txt";
                if (!File.Exists(expectedLogPath))
                {
                    throw new FileNotFoundException("The logger was unable to generate a text file.");
                }
            }
            catch (Exception)
            {
                Assert.Fail("Unable to initialize logger.");
            }
            Assert.Pass("Logger detected.");
        }

        [Test]
        public void TestGetRootDirectory()
        {
            string expected = @"C:\FamilyTreeProject";
            Assert.That(FamilyTreeUtils.GetRootDirectory(), Is.EqualTo(expected));
        }
    }
}