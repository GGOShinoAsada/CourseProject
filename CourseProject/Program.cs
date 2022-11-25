namespace CourseProject
{
    internal class CourseProject
    {
        private static string path = "D:\\COMPILER\\programs\\program11.pas";

        private static void Main(string[] args)
        {
            //SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
            //LexicAnalyzer lexic = new LexicAnalyzer();
            //TestTree();
            //TestSynaxAnalyzer();
            //TestLexicAnalyzer();
            //TestIOTree();
            //TestRepair();
            TestFormTree();
            Console.WriteLine("done");
            Console.ReadKey();
        }

        private static void TestFormTree()
        {
            SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
            BinaryTree tree = analyzer.FormBinaryTree();
            int k = 0;
        }

        private static void TestRepair()
        {
            SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
            analyzer.PrintRepairProgram();
        }

        private static void TestIOTree()
        {
            BinaryTree tree = new BinaryTree();
            tree.AddLeftChild("2");
            tree.AddRightChild("3");
            tree.SetParent(tree.Root);
            tree.AddLeftChild("4");
            tree.SetHead();
            BinaryTree.SaveTreeTofile(tree);
            tree = BinaryTree.RepairTreeFromFile();
        }

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
            bool flag = analyzer.CheckIdentificators();
            Console.WriteLine("flag=" + flag);
            //analyzer.PrintRepairProgram();
        }

        private static void TestTree()
        {
            BinaryTree tree = new BinaryTree();
            tree.AddLeftChild("1234");
            tree.AddLeftChild("4558");
            tree.SetParent(tree.Root);
            tree.AddRightChild("67655");
            tree.SetHead();
            // tree.ScanTree(tree.Root);
        }

        private static void TestLexicAnalyzer()
        {
            LexicAnalyzer analyzer = new LexicAnalyzer();
            analyzer.ReadProgram(path);
            analyzer.FormTokens();
            analyzer.FormIndentificators();
        }
    }
}