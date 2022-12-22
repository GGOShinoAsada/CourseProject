using static CourseProject.LexicAnalyzer;

namespace CourseProject
{
    public class CourseProject
    {
        private const string PATH = "D:\\COMPILER\\programs\\program_tree_demonstration.pas";//"D:\\\\COMPILER\\\\programs\\\\ex1-operatorCompare.pas"; //"D:\\COMPILER\\programs\\ex5_different_types.pas";

        

        private static void Main(string[] args)
        {

            //TestMethod();
            //TestAddBrackets();
            //------------------------//
            //ExecuteLexicAnalyzer(PATH);
            //PrintProgram();
            ExecuteSyntaxAnalyzer();
            // ExecuteCodeGenerator();
            //-----------------------//
            //TestLexicAnalyzer();
            //PrintProgram();
            //TestAddBracket();
            //TestParseExpression();
            Console.WriteLine("done");
            Console.ReadKey();
        }

        //{ "not#", "^", "*", "/", "#div#", "#mod#", "#and#", "+", "-", "#or#", "#xor#", "=", "<>", "<", ">", "<=", ">=", "#in#" };

        static void TestAddBracket()
        {
            string val = "2+2*2";
            SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
            val = analyzer.AddBrackets(val);
            Console.WriteLine(val);
            //string val = "not 67";
            //SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "j^5";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "h*8";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "d/9";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "g67 div 5";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "df mod 8";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "fg and 85";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "5+9";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "fh-532";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "fg or fdff";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "d xor d56";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "f=78";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "g<>78";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "gt<7";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "th>0";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "g<=7";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "g>=0";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
            //val = "arg in arr";
            //val = analyzer.AddBrackets(val);
            //Console.WriteLine(val);
        }

        static void TestParseExpression()
        {
            string val = "(j^5)";//"(not 67)";
            SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
            BinaryTree tree = analyzer.ParseExpression(val);
           // tree.PrintTree(tree.Root);
            //val = "(j^5)";
            //tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
            val = "h*8";
            tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
            val = "(d/9)";
            tree = analyzer.ParseExpression(val);
            tree.PrintTree(tree.Root);
            val = "(g67 div 5)";
            tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
            val = "(df mod 8)";
            tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
            val = "(fg and 85)";
            tree = analyzer.ParseExpression(val);
            tree.PrintTree(tree.Root);
            val = "(5+9)";
            tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
            val = "(fh-532)";
            tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
            val = "(fg or fdff)";
            tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
            val = "(d xor d56)";
            tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
            val = "(f=78)";
            tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
            val = "(g<>78)";
            tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
            val = "(gt<7)";
            tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
            val = "(th>0)";
            tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
            val = "(g<=7)";
            tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
            val = "(g>=0)";
            tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
            val = "(arg in arr)";
            tree = analyzer.ParseExpression(val);
            //tree.PrintTree(tree.Root);
        }

        //static void TestAddBrackets()
        //{
        //    string expr = "a and b or not 2*(2+2)";
        //    //string expr = "2+2";
        //    //string expr = "(2-2)*2/2+68*3";
        //    //string expr = "(2+2) xor (3*5) and c and  not (a or b) and (8/9)";
        //    //string expr = "(2+5) xor (8*(7+9)) and b";
        //    //xor, not, and, or
        //    //
        //    //(((a xor b) and c) and ((not (a or b)) and f))
        //    //{"*","/", "+", "-", "<", ">", "<=", ">=", "=", "<>", "xor", "not", "and", "or" };
        //    SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
        //    string result = analyzer.AddBrackets(expr);
        //    int t = 0;
        //}

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
            //analyzer.PrintProgram();
            analyzer.RemoveComments();
            analyzer.RemoveSpacesAndEmptySymbols();
            analyzer.FormIndentificators();
            analyzer.FormTokens();
            //analyzer.RemoveSpacesAndEmptySymbols();
            //List<string> errors = analyzer.CheckProgram();
            //if (errors.Count == 0)
            //{
            //    analyzer.RemoveSpacesAndEmptySymbols();
            //    analyzer.FormIndentificators();
            //    analyzer.FormTokens();
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    foreach (string error in errors)
            //    {
            //        Console.WriteLine(error);
            //    }
            //    Console.ForegroundColor = ConsoleColor.White;
            //}
        }
        /// <summary>
        /// syntax analyzer
        /// </summary>
        private static void ExecuteSyntaxAnalyzer()
        {
            SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
            analyzer.RepairProgram();
           // analyzer.FormatProgram(); not realized (split by key words)
            if (analyzer.CheckProgram())
            {
                analyzer.Optimizeprogram();
                //analyzer.PrintProgram();
                BinaryTree tree = analyzer.FormBinaryTree();
                //tree.PrintTree(tree.Root);
                //analyzer.PrintProgram();
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