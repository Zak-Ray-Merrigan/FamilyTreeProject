using FamilyTreeLibrary;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Service;

namespace FamilyTreeLibraryTest.Service
{
    public class FamilyTreeServiceTest
    {
        private FamilyTreeService service;
        private Exception problem;
        [SetUp]
        public void Setup()
        {
            try
            {
                FamilyTreeUtils.InitializeLogger();
                string familyName = "Pfingsten";
                service = new(familyName);
                problem = null;
            }
            catch (Exception ex)
            {
                problem = ex;
            }
        }

        [Test]
        public void TestGetNumberOfGenerations()
        {
            Assert.That(service.NumberOfGenerations, Is.EqualTo(6));
        }

        [Test]
        public void TestGetNumberOfFamilies()
        {
            Assert.That(service.NumberOfFamilies, Is.EqualTo(777));
        }

        [Test]
        public void TestAppendTree()
        {
            try
            {
                if (problem is not null)
                {
                    throw problem;
                }
                FileInfo templateFile = new(Path.Combine(FamilyTreeUtils.GetRootDirectory(), @"resources\2023PfingstenBookAlternate.pdf"));
                service.AppendTree(templateFile);
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
        public void TestGenerationNumberOf1()
        {
            int result = 0;
            try
            {
                if (problem is not null)
                {
                    throw problem;
                }
                Family family = new("[Cade Alan Merrigan (29 Oct 2000 - Present)]-[Abigail Faleide (25 Dec 2001 - Present)]: 18 Aug 2023");
                result = service.GenerationNumberOf(family);
            }
            catch (Exception ex)
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Fatal, $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                Assert.That(result, Is.EqualTo(4));
            }
        }

        [Test]
        public void TestReportMarried()
        {
            try
            {
                if (problem is not null)
                {
                    throw problem;
                }
                Person member = new("Cade Alan Merrigan", new FamilyTreeDate("29 Oct 2000"), FamilyTreeDate.DefaultDate);
                Person inLaw = new("Abigail Faleide", new FamilyTreeDate("25 Dec 2001"), FamilyTreeDate.DefaultDate);
                FamilyTreeDate marriageDate = new("18 Aug 2023");
                service.ReportMarried(member, inLaw, marriageDate);
            }
            catch (Exception ex)
            {
                if (ex != problem)
                {
                    FamilyTreeUtils.LogMessage(LoggingLevels.Fatal, $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                    problem = ex;
                }
            }
            finally
            {
                Assert.That(problem, Is.Null);
            }
        }

        [Test]
        public void TestRetrieveParentOf1()
        {
            Family element = new("Lillian Pfingsten (12 Nov 1922 - 9 Oct 1924)");
            Family expectedParent = new("[August Fred Pfingsten (26 Jun 1896 - 24 Aug 1980)]-[Frieda nee Schobinger (10 Nov 1902 - 13 Jul 1938)]: 14 Sep 1921");
            Assert.That(service.RetrieveParentOf(element), Is.EqualTo(expectedParent));
        }
    }
}