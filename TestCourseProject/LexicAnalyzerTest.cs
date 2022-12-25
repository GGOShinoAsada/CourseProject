
using CourseProject;

namespace TestCourseProject
{
    [TestFixture]
    public class LexicAnalyzerTest:LexicAnalyzer
    {
       

        [Test]
        public void IsBinDigitValueTest ()
        {
            string arg = "%0101010";
           
            Assert.True(IsBinDigitValue(arg) == true);
        }

        [Test]
        public void IsOctFigitValueTest()
        {
            string arg = "&025675";
            Assert.True(IsOctDigitValue(arg) == true);
        }

        [Test]
        public void IsHexDigitValue()
        {
            string arg = "$AF756DE";
            Assert.True(IsHexDigitValue(arg) == true);
        }

       
    }
}