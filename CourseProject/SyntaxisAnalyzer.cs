using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CourseProject
{
    public class SyntaxisAnalyzer
    {
        private const string IdentificatorsPath = "D:\\COMPILER\\data\\identificators.txt";

        private const string TokensPath = "D:\\COMPILER\\data\\tokens.txt";

        private const string PatternsPath = "D:\\COMPILER\\data\\patterns.txt";

        private const string DefaultValue = "OP";

        // const string OutputBinaryTreePath = "D:\\COMPILER\\data\\tree.txt";

        private struct Item
        {
            public string Value { get; set; }

            public int Position { get; set; }
        }

        private struct Item2
        {
            public string Value { get; set; }

            public int Column { get; set; }

            public int Row { get; set; }
        }

        private struct IdentTypes
        {
            public string Identificator { get; set; }

            public string Type { get; set; }
        }

        string[] ConditionOperators = { "=", "<>", ">", "<", ">=", "<=" };

        public BinaryTree FormBinaryTree()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            BinaryTree tree = new BinaryTree();
            tree.SetRootValue("start");
            int n = GetProgramSize();
            
            int index = 0;
            try
            {
                string line = RepairProgramLine(index);
                while (!line.Contains("begin"))
                {
                   
                    if (line.StartsWith("var"))
                    {
                        tree.AddLeftChild("var");
                        line = line.Remove(0, 4);
                    }
                    string[] arg0 = line.Split(":")[0].Split(",");

                    string arg1 = line.Split(":")[1];
                    arg1 = arg1.Remove(arg1.IndexOf(';'), 1);
                    for (int j = 0; j < arg0.Length; j++)
                    {
                        tree.AddLeftChild(arg0[j]);
                        tree.AddRightChild(arg1);
                        tree.SetParent(tree.Root);
                        //tree.AddRightChild(arg1);
                        //tree.SetParent(tree.Root);
                        //tree.AddLeftChild(arg0[j]);
                    }
                    index++;
                    line = RepairProgramLine(index);
                }
                tree.SetHead();
                Console.WriteLine("#1");
                tree.PrintTree(tree.Root);

                int opIndex = 0;
                for (int i = index; i < n; i++)
                {
                    //:=, while, if, then, else, until, begin, end;
                    line = RepairProgramLine(i);
                    if (line.Contains(":="))
                    {

                        tree.AddRightChild(DefaultValue + opIndex);
                        tree.AddLeftChild(":=");
                        BinaryTree assTree = ParseAssignOperator(line);
                        tree.SetRightChild(assTree.Root);
                        tree.SetParent(tree.Root);
                        opIndex++;
                    }
                    if (line.Contains("while"))
                    {
                        string expression = line.Substring(line.IndexOf("(") + 1, line.LastIndexOf(")") - 1);
                        foreach (string item in ConditionOperators)
                        {
                            if (line.Contains(item))
                            {
                                string arg0 = expression.Split(item)[0];
                                string arg1 = expression.Split(item)[1];
                                tree.AddRightChild(DefaultValue + opIndex);
                                tree.AddLeftChild(item);
                                tree.SetRightChild(ParseExpression(arg1).Root);
                                tree.SetLeftChild(ParseExpression(arg0).Root);
                                tree.SetParent(tree.Root);
                            }
                        }
                        opIndex++;
                    }
                    if (line.Contains("until"))
                    {
                        string expresssion = line.Substring(line.IndexOf("(") + 1, line.LastIndexOf(")") - 1);
                        foreach (string item in ConditionOperators)
                        {
                            if (expresssion.Contains(item))
                            {
                                string arg0 = expresssion.Split(item)[0];
                                string arg1 = expresssion.Split(item)[1];
                                tree.AddRightChild(DefaultValue + opIndex);
                                tree.AddLeftChild(item);
                                tree.SetRightChild(ParseExpression(arg1).Root);
                                //tree.SetParent(tree.Root);
                                tree.SetLeftChild(ParseExpression(arg0).Root);
                                //tree.SetParent(tree.Root);
                                tree.SetParent(tree.Root);
                            }
                        }
                        opIndex++;
                    }
                    if (line.Contains("write"))
                    {
                        string pattern = "write";
                        string msg = line.Substring(line.IndexOf("(") + 1, line.LastIndexOf(")") - 1);
                        tree.AddRightChild(DefaultValue + n);
                        if (line.Contains("writeln"))
                        {
                            pattern = "writeln";
                        }
                        tree.AddLeftChild(pattern);
                        tree.SetParent(tree.Root);
                        tree.AddRightChild(msg);
                        opIndex++;
                    }
                    if (line.Contains("read"))
                    {
                        string pattern = "read";
                        string msg = line.Substring(line.IndexOf("(") + 1, line.LastIndexOf(")") - 1);
                        tree.AddRightChild(DefaultValue + n);
                        if (line.Contains("readln"))
                        {
                            pattern = "readln";
                        }
                        tree.AddLeftChild(pattern);
                        tree.SetParent(tree.Root);
                        tree.AddRightChild(msg);
                        opIndex++;
                    }
                    if (line.Contains("if"))
                    {

                        string expression = line.Substring(line.IndexOf("(") + 1, line.LastIndexOf(")") - 1);

                        foreach (string item in ConditionOperators)
                        {
                            if (line.Contains(item))
                            {
                                string arg0 = line.Split(item)[0];
                                string arg1 = line.Split(item)[1];
                                tree.AddRightChild(DefaultValue + opIndex);
                                tree.SetRightChild(ParseExpression(arg1).Root);
                                tree.SetLeftChild(ParseExpression(arg1).Root);
                                tree.SetParent(tree.Root);

                            }
                        }
                        opIndex++;
                    }
                    if (line.Contains("else"))
                    {
                        tree.AddRightChild(DefaultValue + opIndex);
                        tree.AddRightChild("else");
                        opIndex++;
                    }
                    if (line.Contains("begin"))
                    {
                        tree.AddRightChild(DefaultValue + opIndex);
                        tree.AddRightChild("begin");
                        opIndex++;
                    }
                    if (line.Contains("end"))
                    {
                        tree.AddRightChild(DefaultValue + opIndex);
                        tree.AddRightChild("end");
                        opIndex++;
                    }
                }
                tree.SetHead();
                Console.WriteLine("#2");
                tree.PrintTree(tree.Root);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            Console.ForegroundColor = ConsoleColor.White;
            return tree;
        }

        

        private int GetProgramSize()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            int n = 0;
            try
            {
                string line = File.ReadAllLines(TokensPath).Last();
                n = int.Parse(line.Split("##")[1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            Console.ForegroundColor = ConsoleColor.White;
            return n;
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
            string path = "C:\\Users\\IRIS\\Documents\\COMP\\COMPILER\\programs\\ex4_mix.pas";
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
            List<Item2> identificators = new List<Item2>();
            List<Item2> tokens = new List<Item2>();
            try
            {
                string? line;
                using (StreamReader reader = new StreamReader(TokensPath))
                {
                    while ((line=reader.ReadLine())!=null)
                    {
                        string arg0 = line.Split("##")[0];
                        string arg1 = line.Split("##")[1];
                        string arg2 = line.Split("##")[2];
                        if (IsContainsBaseType(arg0))
                        {
                            Item2 item = new Item2();
                            item.Value = arg0;
                            item.Column = int.Parse(arg1);
                            item.Row = int.Parse(arg2);
                            tokens.Add(item);
                        }
                        
                    }
                }
                using (StreamReader reader = new StreamReader(IdentificatorsPath))
                {
                    
                    while ((line=reader.ReadLine())!=null)
                    {
                        string arg0 = line.Split("##")[0];
                        string arg1 = line.Split("##")[1];
                        string arg2 = line.Split("##")[2];
                        if (IsCorrectIdentificator(arg0))
                        {
                            bool isExist = false;
                            foreach (Item2 it in identificators)
                            {
                                if (it.Value.Equals(arg0))
                                {
                                    isExist = true;
                                    break;
                                }
                            }
                            if (!isExist)
                            {
                                Item2 item = new Item2();
                                item.Value = arg0;
                                item.Column = int.Parse(arg1);
                                item.Row = int.Parse(arg2);
                                identificators.Add(item);
                            }
                  
                        }
                    }
                }
                List<IdentTypes> dictionary = new List<IdentTypes>();
                foreach (Item2 token in tokens)
                {
                    foreach (Item2 identificator in identificators)
                    {
                        if (identificator.Column == token.Column)
                        {
                            IdentTypes item = new IdentTypes();
                            item.Identificator = identificator.Value;
                            item.Type = token.Value;
                            dictionary.Add(item);
                        }
                    }
                }
                for (int i=0; i<identificators.Count; i++)
                {
                    bool isExist = false;
                    foreach (IdentTypes item in dictionary)
                    {
                        if (item.Identificator.Equals(identificators[i].Value))
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist)
                    {
                        IsCorrect = false;
                        Console.WriteLine("find unknown identioficator, line is {0}, position is {1}", identificators[i].Column, identificators[i].Row);
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
            //List<Item> identificators = new List<Item>();
            //Dictionary<int, string> types = new Dictionary<int, string>();
            //try
            //{
            //    using (StreamReader reader = new StreamReader(IdentificatorsPath))
            //    {
            //        string? line;
            //        while ((line = reader.ReadLine()) != null)
            //        {
            //            int col = int.Parse(line.Split("##")[1]);
            //            string name = line.Split("##")[0];
            //            Item item = new Item();
            //            item.Position = col;
            //            item.Value = name;
            //            bool IsExist = false;
            //            foreach (Item ident in identificators)
            //            {
            //                if (ident.Value.Equals(name))
            //                {
            //                    IsExist = true;
            //                    break;
            //                }
            //            }
            //            if (!IsExist)
            //            {
            //                identificators.Add(item);
            //            }
            //        }
            //    }
            //    using (StreamReader reader = new StreamReader(TokensPath))
            //    {
            //        string? line;
            //        while ((line = reader.ReadLine()) != null)
            //        {
            //            if (IsContainsBaseType(line))
            //            {
            //                int col = int.Parse(line.Split("##")[1]);
            //                string val = line.Split("##")[0];
            //                types.Add(col, val);
            //            }
            //        }
            //    }
            //    int g = 0;
            //    foreach (Item item in identificators)
            //    {
            //        try
            //        {
            //            IsCorrect = types[item.Position] != null;
            //        }
            //        catch (KeyNotFoundException ex)
            //        {
            //            IsCorrect = false;
            //            Console.WriteLine(ex.StackTrace);
            //        }
            //        catch (ArgumentNullException ex1)
            //        {
            //            Console.WriteLine(ex1.StackTrace);
            //        }
            //        catch (Exception ex2)
            //        {
            //            Console.WriteLine(ex2.StackTrace);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.StackTrace);
            //}
            Console.ForegroundColor = ConsoleColor.White;
            return IsCorrect;
        }

        public bool IsCorrectIdentificator(string line)
        {
            
            bool flag = true;
            flag = !Regex.IsMatch(line[0].ToString(), @"^[0-9]*$") || line[0].Equals('_') || Regex.IsMatch(line[0].ToString(), @"^[a-zA-Z]+$");
            string arg = line.Remove(0, 1);
            if (flag)
            {
                flag = Regex.IsMatch(arg, @"^[a-zA-Z0-9]*$") || arg.Contains("_");
            }
            return flag;
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