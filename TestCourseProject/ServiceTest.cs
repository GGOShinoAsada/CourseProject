using CourseProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCourseProject
{
    [TestFixture]
    internal class ServiceTest:Service
    {

        [Test]
        public void RemoveLeftSpacesTest()
        {
            string arg = "   trisha is a fucking noob";
            Assert.True(RemoveLeftSpaces(arg).Equals("trisha is a fucking noob"));
        }

        [Test]
        public void RemoveRigthSpacesTest()
        {
            string arg = "nerenenne   ";
            Assert.True(RemoveRigthSpaces(arg).Equals("nerenenne"));
        }

        [Test]
        public void IsCorrectIdentificatorTest()
        {
            string arg = "_fgdhjg654";
            Assert.True(IsCorrectIdentificator(arg));
        }

        [Test]
        public void GetSubstringTest()
        {
            string arg = "2+(2+2)";
            Assert.True(GetSubsString(arg, arg.IndexOf('('), arg.IndexOf(')')).Equals("(2+2)"));
        }

    }
}
