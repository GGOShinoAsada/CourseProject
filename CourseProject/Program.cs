using static CourseProject.LexicAnalyzer;

namespace CourseProject
{
    public class CourseProject
    {
        private const string PATH = "C:\\Users\\IRIS\\Documents\\COMP\\COMPILER\\programs\\program_tree_demonstration.pas";//"D:\\\\COMPILER\\\\programs\\\\ex1-operatorCompare.pas"; //"D:\\COMPILER\\programs\\ex5_different_types.pas";

        

        private static void Main(string[] args)
        {


            //------------------------//
            ExecuteLexicAnalyzer(PATH);
            PrintProgram();
            ExecuteSyntaxAnalyzer();
            //ExecuteCodeGenerator();
            //-----------------------//

            Console.WriteLine("done");
            Console.ReadKey();
        }


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


        /// <summary>
        /// print repair program
        /// </summary>
        private static void PrintProgram()
        {
            SyntaxisAnalyzer analyzer = new SyntaxisAnalyzer();
            analyzer.PrintRepairProgram();
        }
        

        private static void TestLexicAnalyzer()
        {
            LexicAnalyzer analyzer = new LexicAnalyzer();
            analyzer.ReadProgram(PATH);
            analyzer.RemoveComments();
            analyzer.FormTokens();
            analyzer.FormIndentificators();
        }

        
    }
}