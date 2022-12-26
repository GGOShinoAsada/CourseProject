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


        [Test]
        public void CheckCorrectIdentificatorsTest()
        {
            program = new List<string>();
            program.Add("var a,b,c:integer;");
            program.Add("begin");
            List<Error> errors = CheckIdentificators();
            Assert.True(errors.Count == 0);
        }

        [Test]
        public void CheckCorrectAssignTest()
        {
            program = new List<string>();
            program.Add("d:=4+5;");
            program.Add("a[4]:=4;");
            List<Error> errors = CheckCorrectAssign();
            Assert.True(errors.Count == 0);

        }

        [Test]
        public void CheckInitializeVariablesTest()
        {
            program = new List<string>();
            program.Add("var a,b:integer;");
            program.Add("begin");
            program.Add("a:=0;");
            program.Add("read(b);");
            List<Error> errors = CheckInicializeVariables();
            Assert.True(errors.Count == 0);
        }

        [Test]
        public void CheckDelimitersTest()
        {
            program = new List<string>();
            program.Add("d:=0");
            List<Error> errors = CheckDelimiters();
            Assert.True(errors.Count > 0);
        }


        [Test]
        public void CheckPairSymbolsTest()
        {
            program = new List<string>();
            program.Add("d[4]]:=0;");
            program.Add("end");
            program.Add("begin");
            List<Error> errors = CheckPairSymbols();
            Assert.True(errors.Count > 0);
        }

        [Test]
        public void CheckArrayDeclarationTest()
        {
            program = new List<string>();
            program.Add("var a: array [0..10] of integer;");
            List<Error> errors = CheckArrayDeclaration();
            Assert.True(errors.Count == 0);
            program = new List<string>();
            program.Add("var f: array [..10] of integer");
            errors = CheckArrayDeclaration();
            Assert.True(errors.Count > 0);
        }

        [Test]
        public void CheckLanguageConstructionsTest()
        {
            program = new List<string>();
            program.Add("if (a>b)");
            program.Add("then");
            program.Add("if (c>d)");
            program.Add("then");
            program.Add("begin");
            program.Add("a:=12");
            program.Add("end");
            program.Add("else");
            program.Add("a:=10");
            List<Error> errors = CheckLanguageConstructions();
            Assert.True(errors.Count == 0);
            program = new List<string>();
            program.Add("repeat");
            program.Add("until (a<10)");
            errors = CheckLanguageConstructions();
            Assert.True(errors.Count == 0);
        }

    }
}
