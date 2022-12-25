using CourseProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCourseProject
{
    [TestFixture]
    public class SyntaxAnalyzerTest : SyntaxisAnalyzer
    {
        [Test]
        public void ParseExpressionTest()
        {
            string arg = "2+3";
            BinaryTree ttheor = new BinaryTree();
            ttheor.SetRootValue("+");
            ttheor.AddLeftChild("2");
            ttheor.SetParent(ttheor.Root);
            ttheor.AddRightChild("3");
            ttheor.SetParent(ttheor.Root);
            BinaryTree tfact = ParseExpression(arg);
            string arg0 = GetTreeString(ttheor.Root);
            string arg1 = GetTreeString(tfact.Root);
            bool f = arg0.EndsWith(arg1);
            Assert.True(f);
            string GetTreeString(BinaryTree.Item item)
            {
                return "(" + item.LeftChild.Value + item.Value + item.RightChild.Value + ")";
            }
        }

        [Test]
        public void IsValueTest()
        {
            char c = '_';
            Assert.True(IsValue(c));
        }

        [Test]
        public void AddBracketsTest()
        {
            string exp = "2+2*2";
            Assert.True(AddBrackets(exp).Equals("(2+(2*2))"));
        }

        [Test]
        public void IsCorrectNumberTest()
        {
            string arg = "0.456";
            Assert.That(IsCorrectNumber(arg));
        }

        [Test]
        public void IsCorrexctExpressionTest()
        {
            string arg = "0+fg";
            bool f = IsCorrectExpression(arg);
            Assert.True(f);
        }

        [Test]
        public void CheckFirstSymbolInIdentificatorTest()
        {
            Assert.True(CheckFirstSymbolInIdentificator('_'));
        }

        [Test]
        public void CheckLastSymbolInIdentificator()
        {
            Assert.True(CheckLastSymbolInIdentificator('_'));
        }


        [Test]
        public void IsAllowSymbolBeforeNotTest()
        {
            bool f = IsAllowSymbolBeforeNot(' ');
            Assert.True(f);
        }

        [Test]
        public void IsCorrectValueTest()
        {
            Assert.True(IsCorrectValue("_kj5"));
        }



    }
}
