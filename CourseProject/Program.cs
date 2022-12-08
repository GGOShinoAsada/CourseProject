namespace CourseProject
{
    public class CourseProject
    {
        private const string PATH = "D:\\\\COMPILER\\\\programs\\\\program_tree_demonstration.pas";//"D:\\\\COMPILER\\\\programs\\\\ex1-operatorCompare.pas"; //"D:\\COMPILER\\programs\\ex5_different_types.pas";

        private static void Main(string[] args)
        {
            //TestMethod();
            TestAddBrackets();
            //------------------------//
            //ExecuteLexicAnalyzer(PATH);
            //PrintProgram();
            //ExecuteSyntaxAnalyzer();
           // ExecuteCodeGenerator();
            //-----------------------//
            //TestLexicAnalyzer();
            //PrintProgram();
            Console.WriteLine("done");
            Console.ReadKey();
        }

      
        static void TestMethod()
        {
            string exp = "(1+5)";
            for (int i=0; i<exp.Length; i++)
            {
                if (exp[i].Equals('+'))
                {
                    string arg0 = exp.Substring(0, i);
                    string arg1 = exp.Substring(i + 1, exp.Length - i-1);
                    exp = arg0 + " + " + arg1;
                }
            }
        }

        static void TestAddBrackets()
        {
            string expr = "2+2 xor b*(3+8)<=9";
            //string expr = "2+2";
            //string expr = "(2-2)*2/2+68*3";
            //string expr = "(2+2) xor (3*5) and c and  not (a or b) and (8/9)";
            //string expr = "(2+5) xor (8*(7+9)) and b";
            //xor, not, and, or
            //
            //(((a xor b) and c) and ((not (a or b)) and f))
            //{"*","/", "+", "-", "<", ">", "<=", ">=", "=", "<>", "xor", "not", "and", "or" };
            SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
            string result = analyzer.AddBrackets(expr);
            int t = 0;
        }

        //static void TestParseExpression()
        //{
        //    string exp = "((12*5) or (4*(3+9)))";
        //    SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
        //    BinaryTree tree = analyzer.ParseExpression(exp);
        //    int f = 0;
        //}


        /// <summary>
        /// lexic analyzer
        /// </summary>
        /// <param name="path"></param>
        private static void ExecuteLexicAnalyzer(string path)
        {
            LexicAnalyzer analyzer = new LexicAnalyzer();
            analyzer.ReadProgram(path);
            analyzer.RemoveComments();
            //analyzer.RemoveSpacesAndEmptySymbols();
            List<string> errors = analyzer.CheckProgram();
            if (errors.Count == 0)
            {
                analyzer.FormIndentificators();
                analyzer.FormTokens();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (string error in errors)
                {
                    Console.WriteLine(error);
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        /// <summary>
        /// syntax analyzer
        /// </summary>
        private static void ExecuteSyntaxAnalyzer()
        {
            SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
            if (analyzer.CheckProgram())
            {
                BinaryTree tree = analyzer.FormBinaryTree();
                analyzer.PrintRepairProgram();
                BinaryTree.SaveTreeTofile(tree);
            }
        }

        /// <summary>
        /// code generator
        /// </summary>
        private static void ExecuteCodeGenerator()
        {
            BinaryTree tree = BinaryTree.RepairTreeFromFile();
            tree.PrintTree(tree.Root);
        }

        //private static void TestFormTree()
        //{
        //    SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
        //    BinaryTree tree = analyzer.FormBinaryTree();
        //    int k = 0;
        //}

        /// <summary>
        /// print repair program
        /// </summary>
        private static void PrintProgram()
        {
            SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
            analyzer.PrintRepairProgram();
        }

        //private static void TestIOTree()
        //{
        //    BinaryTree tree = new BinaryTree();
        //    tree.AddLeftChild("2");
        //    tree.AddRightChild("3");
        //    tree.SetParent(tree.Root);
        //    tree.AddLeftChild("4");
        //    tree.SetHead();
        //    BinaryTree.SaveTreeTofile(tree);
        //    tree = BinaryTree.RepairTreeFromFile();
        //}

        private static void TestSynaxAnalyzer()
        {
            SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
            //string line = "(3+(4*5))";
            BinaryTree tree = new BinaryTree();
            string line = "if (a>10) then b:=b+52; else if (d>10) then s:=45+45; else c:=45;";
            //analyzer.ParseOperatorIf(line, 0, ref tree);
            line = "a:=(3+(4*5))";
            //BinaryTree assign = analyzer.ParseAssignOperator(line);
            int f = 0;
            //analyzer.ParseExpression(line);
            //analyzer.ParseProgramLine(line);
            bool flag = analyzer.CheckProgram();
            Console.WriteLine("flag=" + flag);
            //analyzer.PrintRepairProgram();
        }

        //private static void TestTree()
        //{
        //    BinaryTree tree = new BinaryTree();
        //    tree.AddLeftChild("1234");
        //    tree.AddLeftChild("4558");
        //    tree.SetParent(tree.Root);
        //    tree.AddRightChild("67655");
        //    tree.SetHead();
        //    // tree.ScanTree(tree.Root);
        //}

        private static void TestLexicAnalyzer()
        {
            LexicAnalyzer analyzer = new LexicAnalyzer();
            analyzer.ReadProgram(PATH);
            analyzer.RemoveComments();
            analyzer.FormTokens();
            analyzer.FormIndentificators();
        }

        //SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
        //LexicAnalyzer lexic = new LexicAnalyzer();
        //TestTree();
        //TestSynaxAnalyzer();
        //TestLexicAnalyzer();
        //TestIOTree();
        //TestRepair();
        //TestMethod();
        //TestFormTree();
        //--------------------//
        //ExecuteLexicAnalyzer();
        //ExecuteSyntaxAnalyzer();
    }
}