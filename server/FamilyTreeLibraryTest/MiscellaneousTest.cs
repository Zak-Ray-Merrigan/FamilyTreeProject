using System.Text.RegularExpressions;

namespace FamilyTreeLibraryTest
{
    public class MiscellaneousTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            string subText = "B. Elaine Louise Pfingsten 8 Oct 1931 1950 28 Mar 1991 Garhardt Dachtler 14 Jan 1928 10 Jun 1987";
            string templateText = ReadTextFile();
            Assert.That(templateText, Does.Contain(subText));
        }
        private static string FilePath
        {
            get
            {
                DirectoryInfo directory = new(Directory.GetCurrentDirectory());
                while(directory.Name != "FamilyTreeProject")
                {
                    directory = directory.Parent!;
                }
                return Path.Combine(directory.FullName, "resources\\PfingstenFamilyAlternative.txt");
            }
        }

        private string ReadTextFile()
        {
            string temp = File.ReadAllText(FilePath);
            string templateText = Regex.Replace(temp, @"\s+", " ");
            return templateText;
        }
    }
}