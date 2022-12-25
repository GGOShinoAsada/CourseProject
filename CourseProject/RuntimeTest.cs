using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject
{
    public class RuntimeTest:SyntaxisAnalyzer
    {
        public void Test1()
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
            string GetTreeString(BinaryTree.Item item)
            {
                return "(" + item.LeftChild.Value + item.Value + item.RightChild.Value + ")";
            }
        }

        public void Test2()
        {
            string arg = "(2+2)";

             arg = AddBrackets(arg);
        }
    }
}

