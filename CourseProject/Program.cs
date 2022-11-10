namespace CourseProject
{
    class CourseProject
    {
        static string path = "D:\\COMPILER\\programs\\ex4_mix.pas";

        static void Main(string[] args)
        {
            Console.WriteLine("hello world");
            
           SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
           LexicAnalyzer lexic = new LexicAnalyzer();
            //analyzer.ReadProgram(path);
            // analyzer.Test11();
            //TestSyntaxisAnalyzer();
            TestLexicAnalyzer();
            //TestTree();
        }

        static void TestLexicAnalyzer()
        {
            LexicAnalyzer la = new LexicAnalyzer();
            string line = "(3+(4*5))";
            la.ParseProgramLine(line);
            //bool flag = la.CheckIdentificators();
            //Console.WriteLine("flag="+flag);
            //la.PrintRepairProgram();
        }

        static void TestTree()
        {
            BinaryTree tree = new BinaryTree();
            tree.AddLeftChild("1234");
            tree.AddLeftChild("4558");
            tree.SetParent(tree.Root);
            tree.AddRightChild("67655");
            tree.SetHead();
            tree.ScanTree(tree.Root);
        }

        static void TestSyntaxisAnalyzer()
        {
           
            SyntaxisAnalyzer sa = new SyntaxisAnalyzer();
            sa.ReadProgram(path);
            sa.FormTokens();
            sa.FormIndentificators();
        }
    }
}
