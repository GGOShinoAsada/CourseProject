using System.Text.RegularExpressions;

namespace CourseProject
{
    public class SyntaxisAnalyzer
    {
        private const string IdentificatorsPath = "D:\\COMPILER\\data\\identificators.txt";

        private const string TokensPath = "D:\\COMPILER\\data\\tokens.txt";

        private const string PatternsPath = "D:\\COMPILER\\data\\patterns.txt";

        // const string OutputBinaryTreePath = "D:\\COMPILER\\data\\tree.txt";

        private struct Item
        {
            public string Value { get; set; }

            public int Position { get; set; }
        }

        public BinaryTree FormBinaryTree()
        {
            BinaryTree tree = new BinaryTree();
            List<string> program = ReadProgram();
            int i = 0;
            while (!program[i].Contains("begin"))
            {
                string line = program[i];
                if (line.StartsWith("var"))
                {
                    tree.AddLeftChild("var");
                    line = line.Remove(0, 4);
                }
                string[] arg0 = line.Split(":")[0].Split(",");
                string arg1 = line.Split(":")[1];
                for (int j=0; j<arg0.Length; j++)
                {
                    tree.AddRightChild(arg1);
                    tree.SetParent(tree.Root);
                    tree.AddLeftChild(arg0[j]);
                }
                i++;
            }
            return tree;
        }

        public void ParseOperatorIf(string line, int lvl, ref BinaryTree tree)
        {
            //if find if
            int si = line.IndexOf("(");
            int ei = line.IndexOf(")");
            string exp = line.Substring(si + 1, ei - 1);
            tree.SetRootValue(exp);
            if (line.IndexOf("then") > 0)
            {
                string thenBody = line.Substring(line.IndexOf("then") + 5, line.IndexOf("else") - 1);
                tree.AddLeftChild(thenBody);
                tree.SetParent(tree.Root);
            }
            //if find else
            if (line.IndexOf("else") > 0)
            {
                string newLine = line.Remove(0, line.IndexOf("else"));
                if (newLine.IndexOf("if") > 0)
                {
                    ParseOperatorIf(newLine, lvl + 1, ref tree);
                }
                else
                {
                    string elseBody = line.Substring(line.IndexOf("else") + 1, line.Length);
                    tree.AddRightChild(elseBody);
                }
            }
        }

        public BinaryTree ParseAssignOperator(string line)
        {
            BinaryTree tree = new BinaryTree();
            tree.SetRootValue(":=");
            string arg0 = line.Split(":=")[0];
            string arg1 = line.Split(":=")[1];
            tree.AddLeftChild(arg0);
            tree.SetParent(tree.Root);
            BinaryTree exp = ParseExpression(arg1);
            tree.SetRightChild(exp.Root);
            tree.SetParent(tree.Root);
            return tree;
        }

        public BinaryTree ParseExpression(string line)
        {
            BinaryTree tree = new BinaryTree();
            int st = 0;
            try
            {
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i].Equals('('))
                    {
                        tree.AddLeftChild("");
                    }
                    if (line[i].Equals('+') || line[i].Equals('-') || line[i].Equals('*') || line[i].Equals('/'))
                    {
                        tree.SetRootValue(line[i].ToString());
                        tree.AddRightChild("");
                    }
                    if (Regex.IsMatch(line[i].ToString(), @"^[0-9]*$"))
                    {
                        tree.SetRootValue(line[i].ToString());
                        tree.SetParent(tree.Root);
                    }
                    if (line[i].Equals(')'))
                    {
                        tree.SetParent(tree.Root);
                    }
                }

                tree.SetHead();
                //tree.ScanTree(tree.Root);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return tree;
        }

        public List<string> ReadProgram()
        {
            List<string> program = new List<string>();
            string path = "D:\\COMPILER\\programs\\ex4_mix.pas";
            Console.ForegroundColor = ConsoleColor.Red;
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        program.Add(line);
                    }
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            catch (Exception ex0)
            {
                Console.WriteLine(ex0.StackTrace);
            }
            Console.ForegroundColor = ConsoleColor.White;
            return program;
        }

        public void PrintRepairProgram()
        {
            int ind = 0;
            try
            {
                using (StreamReader reader = new StreamReader(TokensPath))
                {
                    string? line = File.ReadLines(TokensPath).Last();
                    ind = int.Parse(line.Split("##")[1]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            for (int i = 0; i < ind + 1; i++)
            {
                string line = RepairProgramLine(i);
                Console.WriteLine(line);
            }
        }

        public bool CheckIdentificators()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            bool IsCorrect = true;

            List<Item> identificators = new List<Item>();
            Dictionary<int, string> types = new Dictionary<int, string>();
            try
            {
                using (StreamReader reader = new StreamReader(IdentificatorsPath))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        int col = int.Parse(line.Split("##")[1]);
                        string name = line.Split("##")[0];
                        Item item = new Item();
                        item.Position = col;
                        item.Value = name;
                        bool IsExist = false;
                        foreach (Item ident in identificators)
                        {
                            if (ident.Value.Equals(name))
                            {
                                IsExist = true;
                                break;
                            }
                        }
                        if (!IsExist)
                        {
                            identificators.Add(item);
                        }
                    }
                }
                using (StreamReader reader = new StreamReader(TokensPath))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (IsContainsBaseType(line))
                        {
                            int col = int.Parse(line.Split("##")[1]);
                            string val = line.Split("##")[0];
                            types.Add(col, val);
                        }
                    }
                }
                foreach (Item item in identificators)
                {
                    try
                    {
                        IsCorrect = types[item.Position] != null;
                    }
                    catch (KeyNotFoundException ex)
                    {
                        IsCorrect = false;
                        Console.WriteLine(ex.StackTrace);
                    }
                    catch (ArgumentNullException ex1)
                    {
                        Console.WriteLine(ex1.StackTrace);
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine(ex2.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            Console.ForegroundColor = ConsoleColor.White;
            return IsCorrect;
        }

        public bool IsContainsBaseType(string line)
        {
            bool flag = false;
            List<string> baseTypes = new List<string>();
            using (StreamReader reader = new StreamReader(PatternsPath))
            {
                for (int i = 0; i < 17; i++)
                {
                    string? tmp = reader.ReadLine();
                    if (tmp != null)
                    {
                        baseTypes.Add(tmp);
                    }
                }
            }
            foreach (string type in baseTypes)
            {
                if (line.Contains(type))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        public string RepairLine(int col)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            string str = "";
            int n = 0;
            List<Item> items = new List<Item>();
            try
            {
               using (StreamReader reader = new StreamReader(TokensPath))
               {
                    string? line;
                    while ((line=reader.ReadLine())!=null)
                    {
                        int column = int.Parse(line.Split("##")[1]);
                        if (column == col)
                        {
                            string name = line.Split("##")[0];
                            int row = int.Parse(line.Split("##")[2]);
                            Item item = new Item();
                            item.Value = name;
                            item.Position = row;
                            items.Add(item);
                        }
                    }
               }
               using (StreamReader reader = new StreamReader(IdentificatorsPath))
               {
                    string? line;
                    while ((line=reader.ReadLine())!=null)
                    {
                        int column = int.Parse(line.Split("##")[1]);
                        if (col == column)
                        {
                            string name = line.Split("##")[0];
                            int row = int.Parse(line.Split("##")[2]);
                            Item item = new Item();
                            item.Value = name;
                            item.Position = row;
                            items.Add(item);
                        }
                    }
               }
                Console.WriteLine("col #" + col);
               for (int j=0; j<items.Count; j++)
               {
                    Console.WriteLine("#{0}: name {1}, row {2}", j, items[j].Value, items[j].Position);
               }
               
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            Console.ForegroundColor = ConsoleColor.White;
            return str;
        }

        public string RepairProgramLine(int col)
        {
            List<Item> items = new List<Item>();
            string line = "";
            try
            {
                using (StreamReader reader = new StreamReader(TokensPath))
                {
                    string? tmp;
                    while ((tmp = reader.ReadLine()) != null)
                    {
                        string arg0 = tmp.Split("##")[0];
                        int arg1 = int.Parse(tmp.Split("##")[1]);
                        int arg2 = int.Parse(tmp.Split("##")[2]);
                        if (arg1 == col)
                        {
                            Item item = new Item();
                            item.Position = arg2;
                            item.Value = arg0;
                            items.Add(item);
                        }
                    }
                }
                using (StreamReader reader = new StreamReader(IdentificatorsPath))
                {
                    string? tmp;
                    while ((tmp = reader.ReadLine()) != null)
                    {
                        string arg0 = tmp.Split("##")[0];
                        int arg1 = int.Parse(tmp.Split("##")[1]);
                        int arg2 = int.Parse(tmp.Split("##")[2]);
                        if (arg1 == col)
                        {
                            Item item = new Item();
                            item.Position = arg2;
                            item.Value = arg0;
                            items.Add(item);
                        }
                    }
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            for (int i = 0; i < items.Count - 1; i++)
            {
                for (int j = i + 1; j < items.Count; j++)
                {
                    if (items[i].Position > items[j].Position)
                    {
                        Item l = items[i];
                        items[i] = items[j];
                        items[j] = l;
                    }
                }
            }
            foreach (Item item in items)
            {
                line += item.Value;
            }
            return line;
        }
    }
}