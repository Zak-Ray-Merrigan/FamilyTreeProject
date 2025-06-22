using FamilyTreeLibrary;
using FamilyTreeLibrary.Data.PDF;

namespace FamilyTreeLibraryTest.Data.PDF
{
    public class PdfClientTest
    {
        private PdfClient client;
        private Exception problem;
        [SetUp]
        public void Setup()
        {
            try
            {
                FamilyTreeUtils.InitializeLogger();
                FileInfo file = new(@"C:\FamilyTreeProject\resources\PDFInputs\2023PfingstenBookAlternate.pdf");
                client = new(file);
                problem = null;
            }
            catch (Exception ex)
            {
                problem = ex;
            }
        }

        [Test]
        public void TestLoadNodes()
        {
            try
            {
                if (problem is not null)
                {
                    throw problem;
                }
                client.LoadNodes();
                problem = null;
            }
            catch (Exception ex)
            {
                problem = ex;
                FamilyTreeUtils.LogMessage(LoggingLevels.Fatal, $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                Assert.That(problem, Is.Null);
            }
        }

        [Test]
        public void TestAttachNodes()
        {
            try
            {
                if (problem is not null)
                {
                    throw problem;
                }
                client.LoadNodes();
                client.AttachNodes();
                problem = null;
            }
            catch (Exception ex)
            {
                problem = ex;
                FamilyTreeUtils.LogMessage(LoggingLevels.Fatal, $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                Assert.That(problem, Is.Null);
            }
        }
    }
}