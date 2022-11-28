using System.Text.RegularExpressions;

namespace CourseProject
{
    public class SyntaxisAnalyzer : Service
    {
        private class Error
        {
            public string Message { get; set; }

            public int Col { get; set; }

            public int Row { get; set; }

            public Error()
            {
                Col = -1;
                Row = -1;
                Message = "undefined";
            }

            public Error(int col, int row)
            {
                this.Col = col;
                this.Row = row;
                Message = "undefined";
            }

            public Error(int col, int row, string message)
            {
                this.Col = col;
                this.Row = row;
                this.Message = message;
            }
        }

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

        private string[] ConditionOperators = { "=", "<>", ">", "<", ">=", "<=" };

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
                // Console.WriteLine("#1");
                //tree.PrintTree(tree.Root);

                int opIndex = 0;
                for (int i = index; i < n; i++)
                {
                    //only with bool = xor, not; for condition can be use or, and
                    //:=, while, if, then, else, until, begin, end;
                    line = RepairProgramLine(i);
                    if (line.Contains(":="))
                    {
                        tree.AddRightChild(DefaultValue + opIndex);
                        string expression = line.Split(":=")[1];
                        tree.AddLeftChild(":=");
                        BinaryTree expTree = ParseExpression(expression);
                        tree.SetRightChild(expTree.Root);
                        tree.SetParent(tree.Root);
                        opIndex++;
                    }
                    if (line.Contains("repeat"))
                    {
                        tree.AddRightChild(DefaultValue + opIndex);
                        tree.AddLeftChild("repeat");
                        tree.SetParent(tree.Root);

                        opIndex++;
                    }
                    if (line.Contains("until"))
                    {
                        string expresssion = GetSubsString(line, line.IndexOf("(") + 1, line.LastIndexOf(")") - 1);
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
                        string msg = GetSubsString(line, line.IndexOf("(") + 1, line.LastIndexOf(")") - 1);
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
                        string msg = GetSubsString(line, line.IndexOf("(") + 1, line.LastIndexOf(")") - 1);
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
                        string expression = GetSubsString(line, line.IndexOf("(") + 1, line.LastIndexOf(')') - 1);

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

        private string GetSubsString(string line, int sp, int ep)
        {
            string result = string.Empty;
            for (int i = sp; i <= ep; i++)
            {
                result += line[i];
            }
            return result;
        }

        private int GetProgramSize()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            int n = 0;
            try
            {
                if (File.ReadAllLines(TokensPath).Length > 0)
                {
                    string line = File.ReadAllLines(TokensPath).Last();
                    n = int.Parse(line.Split("##")[1]) + 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            Console.ForegroundColor = ConsoleColor.White;
            return n;
        }

        private BinaryTree ParseExpression(string line)
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

        //public List<string> ReadProgram()
        //{
        //    List<string> program = new List<string>();
        //    string path = "C:\\Users\\IRIS\\Documents\\COMP\\COMPILER\\programs\\ex4_mix.pas";
        //    Console.ForegroundColor = ConsoleColor.Red;
        //    try
        //    {
        //        using (StreamReader reader = new StreamReader(path))
        //        {
        //            string? line;
        //            while ((line = reader.ReadLine()) != null)
        //            {
        //                program.Add(line);
        //            }
        //        }
        //    }
        //    catch (DirectoryNotFoundException ex)
        //    {
        //        Console.WriteLine(ex.StackTrace);
        //    }
        //    catch (Exception ex0)
        //    {
        //        Console.WriteLine(ex0.StackTrace);
        //    }
        //    Console.ForegroundColor = ConsoleColor.White;
        //    return program;
        //}

        public bool CheckProgram()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            bool flag = true;
            List<Error> errors = new List<Error>();
            errors = CheckIdentificators();
            if (errors.Count > 0)
            {
                flag = false;
                foreach (Error error in errors)
                {
                    Console.WriteLine("find unknown identificator, location: col {0}, row {1}", error.Col, error.Row);
                }
            }
            errors = CheckCorrectAssign();
            if (errors.Count > 0)
            {
                flag = false;
            }
            errors = CheckRepeatedDeclare();
            if (errors.Count > 0)
            {
                flag = false;
                foreach (Error error in errors)
                {
                    Console.WriteLine("find duplicaded variable declare: col {0}", error.Col);
                }
            }
            errors = CheckPairSymbols();
            if (errors.Count > 0)
            {
                flag = false;
                foreach (Error error in errors)
                {
                    switch (error.Message)
                    {
                        case "BEGIN_END":
                            Console.WriteLine("please check count of begin and end");
                            break;

                        case "CIRCLE_BRACKET":
                            Console.WriteLine("find different count of pair stymbols \"(\" and \"end\", line is {0}", error.Col);
                            break;

                        case "RECTANGLE_BRACKET":
                            Console.WriteLine("find different count of pair symbols \"[\" and \"]\", line is {0}", error.Col);
                            break;
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            return flag;
        }

        private List<Error> CheckIdentificators()
        {
            List<Error> errors = new List<Error>();
            List<Item2> identificators = new List<Item2>();
            List<Item2> tokens = new List<Item2>();
            try
            {
                string? line;
                using (StreamReader reader = new StreamReader(TokensPath))
                {
                    while ((line = reader.ReadLine()) != null)
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
                    while ((line = reader.ReadLine()) != null)
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
                for (int i = 0; i < identificators.Count; i++)
                {
                    bool isExist = false;
                    foreach (IdentTypes item in dictionary)
                    {
                        if (item.Identificator.Equals(identificators[i].Value))
                        {
                            isExist = true;
                            Error error = new Error();
                            error.Col = identificators[i].Column;
                            error.Row = identificators[i].Row;
                            errors.Add(error);
                            //break;
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

            Console.ForegroundColor = ConsoleColor.White;
            return errors;
        }

        private List<Error> CheckCorrectAssign()
        {
            List<Error> errors = new List<Error>();
            //логические операторы and, or, not, xor
            //для целых/вещественных +,-,*,/
            //для целых div, mod
            //проверить корректное присвоение переменных и операции с ними

            return errors;
        }

        private List<Error> CheckRepeatedDeclare()
        {
            List<Error> errors = new List<Error>();
            //int a =; bool(int) a = false;
            List<string> variables = new List<string>();
            int n = GetProgramSize();
            if (n > 0)
            {
                for (int i = 0; i < n; i++)
                {
                    string line = RepairProgramLine(i);
                    if (line.Equals("begin"))
                        break;
                    if (line.Contains("var"))
                        line = line.Remove(0, 4);
                    string[] arg0 = line.Split(":")[0].Split(",");
                    foreach (string arg in arg0)
                    {
                        foreach (string variable in variables)
                        {
                            if (variable.Equals(arg))
                            {
                                Error error = new Error();
                                error.Col = i;
                                errors.Add(error);
                            }
                        }
                    }
                    //variables.Add()
                }
            }

            return errors;
        }

        private List<Error> CheckPairSymbols()
        {
            List<Error> errors = new List<Error>();
            //check correct for pair symbols
            //symbols: ( and ), [ and ], begin and end
            int n = GetProgramSize();
            int f1 = 0; short f2 = 0; short f3 = 0;
            for (int i = 0; i < n; i++)
            {
                string line = RepairProgramLine(i);
                List<int> indexes = GetAllContains(line, "begin");
                f1 += indexes.Count;
                indexes = GetAllContains(line, "end");
                f1 -= indexes.Count;
                for (int j = 0; j < line.Length; j++)
                {
                    if (line[j].Equals('('))
                    {
                        f2++;
                    }
                    if (line[j].Equals(')'))
                    {
                        f2--;
                    }
                    if (line[j].Equals('['))
                    {
                        f3++;
                    }
                    if (line[j].Equals(']'))
                    {
                        f3--;
                    }
                }
                if (f2 != 0)
                {
                    Error error = new Error();
                    error.Col = i;
                    error.Message = "CURCLE_BRACKET";
                    errors.Add(error);
                }
                if (f3 != 0)
                {
                    Error error = new Error();
                    error.Col = i;
                    error.Message = "RECTANGLE_BRACKET";
                    errors.Add(error);
                }
                f2 = 0; f3 = 0;
            }
            if (f1 != 0)
            {
                Error e = new Error();
                e.Message = "BEGIN_END";
                errors.Add(new Error());
            }
            return errors;
        }

        private List<int> GetAllContains(string line, string value)
        {
            List<int> indexes = new List<int>();
            int ind = line.IndexOf(value);
            while (ind >= 0)
            {
                indexes.Add(ind);
                ind = line.IndexOf(value, ind + value.Length);
            }
            return indexes;
        }

        private bool IsCorrectIdentificator(string line)
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

        private bool IsContainsBaseType(string line)
        {
            bool flag = false;
            List<string> baseTypes = new List<string>();
            using (StreamReader reader = new StreamReader(PatternsPath))
            {
                for (int i = 0; i < 18; i++)
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

        private string RepairProgramLine(int col)
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