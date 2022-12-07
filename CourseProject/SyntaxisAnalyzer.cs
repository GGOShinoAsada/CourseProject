using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using static CourseProject.BinaryTree;

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

    
        private class Operator
        {
            public string Value { get; set; }
            
            public int Position { get; set; }

            public bool Flag { get; set; } = true;

            public Operator(string val, int pos)
            {
                this.Value = val;
                this.Position = pos;
            }

            public Operator(string val, int pos, bool f)
            {
                this.Value = val;
                this.Position = pos;
                this.Flag = f;
            }

        }

        private class Item
        {
            public string Value { get; set; }

            public int Position { get; set; }
        }

        

        private class Item2
        {
            public string Value { get; set; }

            public int Column { get; set; }

            public int Row { get; set; }
        }

        private class IdentTypes
        {
            public string Identificator { get; set; }

            public string Type { get; set; }
        }

        //private string[] ConditionOperators = { "=", "<>", ">", "<", ">=", "<=" };

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

                int opIndex = 0;

                for (int i=index; i<n; i++)
                {

                }


                //----------trash---------
                // Console.WriteLine("#1");
                //tree.PrintTree(tree.Root);

                //int opIndex = 0;
                //
                //for (int i = index; i < n; i++)
                //{
                //    //only with bool = xor, not; for condition can be use or, and
                //    //:=, while, if, then, else, until, begin, end;
                //    line = RepairProgramLine(i);
                //    if (line.Contains(":="))
                //    {
                //        tree.AddRightChild(DefaultValue + opIndex);
                //        string expression = line.Split(":=")[1];
                //        tree.AddLeftChild(":=");
                //        BinaryTree expTree = ParseExpression(expression);
                //        tree.SetRightChild(expTree.Root);
                //        tree.SetParent(tree.Root);
                //        opIndex++;
                //    }
                //    if (line.Contains("repeat"))
                //    {
                //        tree.AddRightChild(DefaultValue + opIndex);
                //        tree.AddLeftChild("repeat");
                //        tree.SetParent(tree.Root);

                //        opIndex++;
                //    }
                //    if (line.Contains("until"))
                //    {
                //        string expresssion = GetSubsString(line, line.IndexOf("(") + 1, line.LastIndexOf(")") - 1);
                //        foreach (string item in ConditionOperators)
                //        {
                //            if (expresssion.Contains(item))
                //            {
                //                string arg0 = expresssion.Split(item)[0];
                //                string arg1 = expresssion.Split(item)[1];
                //                tree.AddRightChild(DefaultValue + opIndex);
                //                tree.AddLeftChild(item);
                //                tree.SetRightChild(ParseExpression(arg1).Root);
                //                //tree.SetParent(tree.Root);
                //                tree.SetLeftChild(ParseExpression(arg0).Root);
                //                //tree.SetParent(tree.Root);
                //                tree.SetParent(tree.Root);
                //            }
                //        }
                //        opIndex++;
                //    }
                //    if (line.Contains("write"))
                //    {
                //        string pattern = "write";
                //        string msg = GetSubsString(line, line.IndexOf("(") + 1, line.LastIndexOf(")") - 1);
                //        tree.AddRightChild(DefaultValue + n);
                //        if (line.Contains("writeln"))
                //        {
                //            pattern = "writeln";
                //        }
                //        tree.AddLeftChild(pattern);
                //        tree.SetParent(tree.Root);
                //        tree.AddRightChild(msg);
                //        opIndex++;
                //    }
                //    if (line.Contains("read"))
                //    {
                //        string pattern = "read";
                //        string msg = GetSubsString(line, line.IndexOf("(") + 1, line.LastIndexOf(")") - 1);
                //        tree.AddRightChild(DefaultValue + n);
                //        if (line.Contains("readln"))
                //        {
                //            pattern = "readln";
                //        }
                //        tree.AddLeftChild(pattern);
                //        tree.SetParent(tree.Root);
                //        tree.AddRightChild(msg);
                //        opIndex++;
                //    }
                //    if (line.Contains("if"))
                //    {
                //        string expression = GetSubsString(line, line.IndexOf("(") + 1, line.LastIndexOf(')') - 1);

                //        foreach (string item in ConditionOperators)
                //        {
                //            if (line.Contains(item))
                //            {
                //                string arg0 = line.Split(item)[0];
                //                string arg1 = line.Split(item)[1];
                //                tree.AddRightChild(DefaultValue + opIndex);
                //                tree.SetRightChild(ParseExpression(arg1).Root);
                //                tree.SetLeftChild(ParseExpression(arg1).Root);
                //                tree.SetParent(tree.Root);
                //            }
                //        }
                //        opIndex++;
                //    }
                //    if (line.Contains("else"))
                //    {
                //        tree.AddRightChild(DefaultValue + opIndex);
                //        tree.AddRightChild("else");
                //        opIndex++;
                //    }
                //    if (line.Contains("begin"))
                //    {
                //        tree.AddRightChild(DefaultValue + opIndex);
                //        tree.AddRightChild("begin");
                //        opIndex++;
                //    }
                //    if (line.Contains("end"))
                //    {
                //        tree.AddRightChild(DefaultValue + opIndex);
                //        tree.AddRightChild("end");
                //        opIndex++;
                //    }
                //}
                //tree.SetHead();
                //Console.WriteLine("#2");
                //tree.PrintTree(tree.Root);
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
            Console.ForegroundColor = ConsoleColor.Red;
            try
            {
                if (!string.IsNullOrEmpty(line))
                {
                    //if ((line[0] != '(') && (line.Last() != ')'))
                    //{
                    //    line = '(' + line + ')';
                    //}
                    //line = AddBrackets(line);
                    int ind = 0;

                    //number - [0-9,.]
                    //value - a-zA-Z0-9_
                    //and, or, xor, not, <, >, =, <>, <=, >=, +, -, *, /

                    
                    string temp = line;

                    List<Operator> operators = new List<Operator>();

                    List<int> indexes = GetAllContains(line, "*");

                    foreach (int index in indexes)
                    {
                        
                        operators.Add(new Operator("*", index));
                    }

                    indexes = GetAllContains(line, "/");

                    foreach (int index in indexes)
                    {
                                          
                        operators.Add(new Operator("/", index));
                    }

                    indexes = GetAllContains(line, "+");

                    foreach (int index in indexes)
                    {
                        
                        operators.Add(new Operator("+", index));
                    }

                    indexes = GetAllContains(line, "-");

                    foreach (int index in indexes)
                    {
                      
                        operators.Add(new Operator("-", index));
                    }

                    indexes = GetAllContains(line, "<=");

                    foreach (int index in indexes)
                    {
                        operators.Add(new Operator("<=", index));
                    }

                    indexes = GetAllContains(line, ">=");

                    foreach (int index in indexes)
                    {
                        operators.Add(new Operator(">=", index));
                    }

                    indexes = GetAllContains(line, "<>");

                    foreach (int index in indexes)
                    {
                        operators.Add(new Operator("<>", index));
                    }

                    indexes = GetAllContains(line, "=");

                    foreach (int index in indexes)
                    {
                        if (index-1>=0)
                        {
                            if (!line[index-1].Equals('<') && !line[index-1].Equals('>'))
                            {
                                operators.Add(new Operator("=", index));
                            }
                        }
                       
                    }

                    indexes = GetAllContains(line, "<");

                    foreach (int index in indexes)
                    {
                        if (index + 1 < line.Length)
                        {
                            if (!line[index + 1].Equals('>') && !line[index + 1].Equals('='))
                            {
                                operators.Add(new Operator("<", index));
                            }
                        }

                    }

                    indexes = GetAllContains(line, ">");

                    foreach (int index in indexes)
                    {
                        if (index + 1 < line.Length)
                        {
                            if (!line[index + 1].Equals('='))
                            {
                                if (index-1>=0)
                                {
                                    if (!line[index-1].Equals('<'))
                                    {
                                        operators.Add(new Operator(">", index));
                                    }
                                }
                               
                            }
                        }

                    }

                    indexes = GetAllContains(line, "xor");

                    foreach (int index in indexes)
                    {
                        operators.Add(new Operator("xor", index));
                    }

                    indexes = GetAllContains(line, "or");

                    foreach (int index in indexes)
                    {
                        if (index - 1 >=0)
                        {
                            if (!line[index - 1].Equals('x'))
                            {
                                operators.Add(new Operator("or", index));
                            }
                        }

                    }

                    indexes = GetAllContains(line, "and");

                    foreach (int index in indexes)
                    {
                        operators.Add(new Operator("and", index));
                    }

                    indexes = GetAllContains(line, "not");

                    foreach (int index in indexes)
                    {
                        operators.Add(new Operator("not", index));
                    }

                    while (ind < line.Length)
                    {
                        if (line[ind].Equals('('))
                        {
                            tree.AddLeftChild("");
                        }
                        int index = -1;
                        for (int j=0; j<operators.Count; j++)
                        {
                            if (operators[j].Position == ind)
                            {
                                index = j;
                                operators[j].Flag = false;
                                break;
                            }
                        }
                      
                        if (index>=0)
                        {
                            //operator
                            string op = operators[index].Value;
                            ind += op.Length-1;
                            tree.SetRootValue(op);
                            tree.AddRightChild("");
                           
                        }             
                        if (IsValue(line[ind]) && (index==-1))
                        {
                            //value
                            string value = "";
                          

                            while (IsValue(line[ind]))
                            {
                                value += line[ind];
                                ind++;
                            }
                            ind--;
                            tree.SetRootValue(value);
                            tree.SetParent(tree.Root);

                        }
                       
                        
                        if (line[ind].Equals(')'))
                        {
                            tree.SetParent(tree.Root);
                        }
                        ind++;
                    }

                    
                }
                tree.SetHead();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

           

            Console.ForegroundColor = ConsoleColor.White;
            return tree;
        }


        private bool IsValue(char c)
        {
            return Regex.IsMatch(c.ToString(), "^[0-9]+$") || Regex.IsMatch(c.ToString(), "^[a-zA-Z]+$") || c.Equals('.') || c.Equals('_');
        }

        public string AddBrackets(string line)
        {
            //xor=#, not=#_, and=#__, or=#___

            //replase 

            line = ChangeSymbols(line);

            string[] array = new string[] { "*", "/", "+", "-", "<", ">", "<=", ">=", "=", "<>", "#", "#_", "#__", "#___" };

            char[] patterns = new char[] {/*' ',*/ '*', '/', '+', '-', '(', ')', '<', '>', '=', '#' };

            List<Item> indexes = FormIndexes();

            int n = indexes.Count;

            foreach (string op in array)
            {
                for (int i = 0; i < n; i++)
                {
                    if (indexes[i].Value.Equals(op))
                    {
                        indexes = FormIndexes();
                        int ind = indexes[i].Position;
                        bool f1 = true; bool f2 = true;
                        int lp = -1; int rp = -1;
                        int tmp = ind - 1;
                        sbyte br = 0;

                        while (tmp >= 0)
                        {
                            if (line[tmp].Equals(')'))
                                br++;
                            if (line[tmp].Equals('('))
                                br--;
                            if (br == 0)
                            {
                                if (patterns.Contains(line[tmp]))
                                    break;
                            }
                            tmp--;
                        }
                        if (tmp == -1)
                        {
                            f1 = false;
                            tmp++;
                        }
                        if (IsSpecificSymbol(tmp))
                            tmp++;
                        lp = tmp;
                        if (tmp - 1 >= 0)
                        {
                            f1 = line[tmp - 1].Equals('(');
                        }



                        indexes = FormIndexes();
                        ind = indexes[i].Position;
                        tmp = ind + op.Length;
                        br = 0;
                        while (tmp < line.Length)
                        {

                            if (line[tmp].Equals(')'))
                                br++;
                            if (line[tmp].Equals('('))
                                br--;
                            if (br == 0)
                            {
                                if (patterns.Contains(line[tmp]))
                                    break;
                            }
                            tmp++;
                        }
                        if (tmp == line.Length)
                        {
                            f2 = false;
                            tmp--;
                        }
                        if (IsSpecificSymbol(tmp))
                            tmp--;
                        rp = tmp;
                        if (tmp + 1 < line.Length)
                        {
                            f2 = line[tmp + 1].Equals(')');
                        }

                        if (!(f1 && f2))
                        {
                            string arg0 = GetSubsString(line, 0, lp - 1);
                            string arg1 = GetSubsString(line, lp, rp);
                            string arg2 = GetSubsString(line, rp + 1, line.Length - 1);
                            line = arg0 + "(" + arg1 + ")" + arg2;
                        }

                    }
                }
            }

            line = ChangeSymbols(line, false);

            bool IsSpecificSymbol(int index)
            {
                bool f = line[index].Equals('+') || line[index].Equals('-') || line[index].Equals('*')
                    || line[index].Equals('/') /*|| line[index].Equals(' ')*/ || line[index].Equals('<')
                    || line[index].Equals('>') || line[index].Equals('=') || line[index].Equals('#');
                return f;
            }

            List<Item> FormIndexes()
            {
                List<Item> indexes = new List<Item>();
                List<int> data = new List<int>();
                foreach (string delimiter in array)
                {
                    if (delimiter.Equals("<") || delimiter.Equals(">") || delimiter.Equals("=") || delimiter.Equals("#") || delimiter.Equals("#_") || delimiter.Equals("#__"))
                    {
                        int ind = 0;
                        switch (delimiter)
                        {
                            case "<":
                                ind = line.IndexOf(delimiter);
                                while (ind >= 0)
                                {
                                    bool f = ind + 1 < line.Length;
                                    if (f)
                                    {
                                        f = !line[ind + 1].Equals('=') && !line[ind + 1].Equals('>');
                                    }
                                    if (f)
                                    {
                                        Item item = new Item();
                                        item.Position = ind;
                                        item.Value = delimiter;
                                        indexes.Add(item);
                                    }
                                    ind = line.IndexOf(delimiter, ind + delimiter.Length);
                                }

                                break;
                            case ">":
                                ind = line.IndexOf(delimiter);
                                while (ind >= 0)
                                {
                                    bool f = ind + 1 < line.Length;
                                    if (f)
                                    {
                                        f = !line[ind + 1].Equals('=');

                                    }
                                    bool f1 = ind - 1 >= 0;
                                    if (f1)
                                    {
                                        f1 = !line[ind - 1].Equals('<');
                                    }
                                    if (f && f1)
                                    {
                                        Item item = new Item();
                                        item.Value = delimiter;
                                        item.Position = ind;
                                        indexes.Add(item);
                                    }
                                    ind = line.IndexOf(delimiter, ind + delimiter.Length);
                                }
                                break;
                            case "=":
                                ind = line.IndexOf(delimiter);
                                while (ind >= 0)
                                {
                                    bool f = ind - 1 >= 0;
                                    if (f)
                                    {
                                        f = !line[ind - 1].Equals('<') && !line[ind - 1].Equals('>');
                                    }
                                    if (f)
                                    {
                                        Item item = new Item();
                                        item.Position = ind;
                                        item.Value = delimiter;
                                        indexes.Add(item);
                                    }
                                    ind = line.IndexOf(delimiter, ind + delimiter.Length);
                                }

                                break;
                            case "#":
                                data = GetAllContains(line, delimiter);

                                foreach (int temp in data)
                                {
                                    if (ReturnCountOfLattice(temp)==1)
                                    {
                                        Item item = new Item();
                                        item.Position = temp;
                                        item.Value = delimiter;
                                        indexes.Add(item);
                                    }
                                   

                                }


                                break;
                            case "#_":
                                data = GetAllContains(line, delimiter);

                                foreach (int temp in data)
                                {
                                    if (ReturnCountOfLattice(temp)==2)
                                    {
                                        Item item = new Item();
                                        item.Position = temp;
                                        item.Value = delimiter;
                                        indexes.Add(item);
                                    }                                  

                                }

                                break;
                            case "#__":
                                data = GetAllContains(line, delimiter);

                                foreach (int temp in data)
                                {
                                    if (ReturnCountOfLattice(temp)==3)
                                    {
                                        Item item = new Item();
                                        item.Position = temp;
                                        item.Value = delimiter;
                                        indexes.Add(item);
                                    }                                 

                                }

                                break;
                            

                        }
                    }
                    else
                    {
                        int ind = line.IndexOf(delimiter);
                        while (ind >= 0)
                        {
                            Item item = new Item();
                            item.Position = ind;
                            item.Value = delimiter;
                            indexes.Add(item);
                            ind = line.IndexOf(delimiter, ind + delimiter.Length);
                        }
                    }
                }
                return indexes;
            }

            int ReturnCountOfLattice(int index)
            {
                int n = 0;
                int si = 0;
                for (int i=index; i>=0; i--)
                {
                    if (!line[i].Equals('#'))
                    {
                        si = i + 1;
                        break;
                    }
                }
                for (int i=si; i<line.Length; i++)
                {
                    if (!line[i].Equals('#'))
                    {                        
                        break;
                    }
                    else
                    {
                        n++;
                    }
                }

                return n;
            }

            //bool CheckLattice(string value, int index)
            //{
            //    bool f = true;
            //    switch (value)
            //    {
            //        case "###":
            //            if (index - 1 >= 0)
            //            {
            //                f = !line[index - 1].Equals('#');
            //            }
            //            if (f)
            //            {
            //                if (index + 3 < line.Length)
            //                {
            //                    f = !line[index + 3].Equals('#');
            //                }
            //            }
            //            break;
            //        case "##":
            //            if (index - 1 >= 0)
            //            {
            //                f = !line[index - 1].Equals('#');
            //            }
            //            if (f)
            //            {
            //                if (index + 2 < line.Length)
            //                {
            //                    f = !line[index + 2].Equals('#');
            //                }
            //            }
            //            break;
            //        case "#":
            //            if (index - 1 >= 0)
            //            {
            //                f = !line[index - 1].Equals('#');
            //            }
            //            if (f)
            //            {
            //                if (index + 1 < line.Length)
            //                {
            //                    f = !line[index + 1].Equals('#');
            //                }
            //            }
            //            break;
            //    }
            //    return f;
            //}
           
            return line;
        }

        public string AddBracketsLog(string line)
        {
            //xor=#, not=##, and=###, or=####

            //replase 

            //line = ChangeSymbols(line);

            string[] array = new string[] { "xor", "not", "and", "or" };

            char[] patterns = new char[] { ' ' };

            List<Item> indexes = FormIndexes();

            int n = indexes.Count;

            foreach (string op in array)
            {
                for (int i = 0; i < n; i++)
                {
                    if (indexes[i].Value.Equals(op))
                    {
                        indexes = FormIndexes();
                        int ind = indexes[i].Position;
                        bool f1 = true; bool f2 = true;
                        int lp = -1; int rp = -1;
                        int tmp = ind - 1;
                        sbyte br = 0;

                        while (tmp >= 0)
                        {
                            if (line[tmp].Equals(')'))
                                br++;
                            if (line[tmp].Equals('('))
                                br--;
                            if (br == 0)
                            {
                                if (patterns.Contains(line[tmp]))
                                    break;
                            }
                            tmp--;
                        }
                        if (tmp == -1)
                        {
                            f1 = false;
                            tmp++;
                        }
                        if (IsSpecificSymbol(tmp))
                            tmp++;
                        lp = tmp;
                        if (tmp - 1 >= 0)
                        {
                            f1 = line[tmp - 1].Equals('(');
                        }



                        indexes = FormIndexes();
                        ind = indexes[i].Position;
                        tmp = ind + op.Length;
                        br = 0;
                        while (tmp < line.Length)
                        {

                            if (line[tmp].Equals(')'))
                                br++;
                            if (line[tmp].Equals('('))
                                br--;
                            if (br == 0)
                            {
                                if (patterns.Contains(line[tmp]))
                                    break;
                            }
                            tmp++;
                        }
                        if (tmp == line.Length)
                        {
                            f2 = false;
                            tmp--;
                        }
                        if (IsSpecificSymbol(tmp))
                            tmp--;
                        rp = tmp;
                        if (tmp + 1 < line.Length)
                        {
                            f2 = line[tmp + 1].Equals(')');
                        }

                        if (!(f1 && f2))
                        {
                            string arg0 = GetSubsString(line, 0, lp - 1);
                            string arg1 = GetSubsString(line, lp, rp);
                            string arg2 = GetSubsString(line, rp + 1, line.Length - 1);
                            line = arg0 + "(" + arg1 + ")" + arg2;
                        }

                    }
                }
            }

            line = ChangeSymbols(line, false);

            bool IsSpecificSymbol(int index)
            {
                bool f = line[index].Equals('+') || line[index].Equals('-') || line[index].Equals('*')
                    || line[index].Equals('/') /*|| line[index].Equals(' ')*/ || line[index].Equals('<')
                    || line[index].Equals('>') || line[index].Equals('=') || line[index].Equals('#');
                return f;
            }

            List<Item> FormIndexes()
            {
                List<Item> indexes = new List<Item>();
                List<int> data = new List<int>();
                foreach (string delimiter in array)
                {
                    if (delimiter.Equals("<") || delimiter.Equals(">") || delimiter.Equals("=") || delimiter.Equals("#") || delimiter.Equals("##") || delimiter.Equals("###"))
                    {
                        int ind = 0;
                        switch (delimiter)
                        {
                            case "<":
                                ind = line.IndexOf(delimiter);
                                while (ind >= 0)
                                {
                                    bool f = ind + 1 < line.Length;
                                    if (f)
                                    {
                                        f = !line[ind + 1].Equals('=') && !line[ind + 1].Equals('>');
                                    }
                                    if (f)
                                    {
                                        Item item = new Item();
                                        item.Position = ind;
                                        item.Value = delimiter;
                                        indexes.Add(item);
                                    }
                                    ind = line.IndexOf(delimiter, ind + delimiter.Length);
                                }

                                break;
                            case ">":
                                ind = line.IndexOf(delimiter);
                                while (ind >= 0)
                                {
                                    bool f = ind + 1 < line.Length;
                                    if (f)
                                    {
                                        f = !line[ind + 1].Equals('=');

                                    }
                                    bool f1 = ind - 1 >= 0;
                                    if (f1)
                                    {
                                        f1 = !line[ind - 1].Equals('<');
                                    }
                                    if (f && f1)
                                    {
                                        Item item = new Item();
                                        item.Value = delimiter;
                                        item.Position = ind;
                                        indexes.Add(item);
                                    }
                                    ind = line.IndexOf(delimiter, ind + delimiter.Length);
                                }
                                break;
                            case "=":
                                ind = line.IndexOf(delimiter);
                                while (ind >= 0)
                                {
                                    bool f = ind - 1 >= 0;
                                    if (f)
                                    {
                                        f = !line[ind - 1].Equals('<') && !line[ind - 1].Equals('>');
                                    }
                                    if (f)
                                    {
                                        Item item = new Item();
                                        item.Position = ind;
                                        item.Value = delimiter;
                                        indexes.Add(item);
                                    }
                                    ind = line.IndexOf(delimiter, ind + delimiter.Length);
                                }

                                break;
                            case "or":
                                ind = line.IndexOf(delimiter);
                                while (ind >= 0)
                                {
                                    bool f = ind - 2 >= 0;
                                    if (f)
                                    {
                                        f = !line[ind - 2].Equals('x');
                                    }
                                    if (f)
                                    {
                                        Item item = new Item();
                                        item.Position = ind;
                                        item.Value = delimiter;
                                    }
                                    ind = line.IndexOf(delimiter, ind + delimiter.Length);
                                }
                                break;


                        }
                    }
                    else
                    {
                        int ind = line.IndexOf(delimiter);
                        while (ind >= 0)
                        {
                            Item item = new Item();
                            item.Position = ind;
                            item.Value = delimiter;
                            indexes.Add(item);
                            ind = line.IndexOf(delimiter, ind + delimiter.Length);
                        }
                    }
                }
                return indexes;
            }

          
            return line;
        }
        private string ChangeSymbols(string line, bool flag=true)
        {

            if (flag)
            {
                string[] array = new string[] { "xor", "not", "and", "or" };
                foreach (string value in array)
                {
                    string temp = "";
                    if (!value.Equals("not"))
                        temp = " " + value + " ";
                    else
                        temp = value + " ";
                    switch (value)
                    {
                        case "xor":
                            line = line.Replace(temp, "_");
                            break;
                        case "not":
                            line = line.Replace(temp, "_#");
                            break;
                        case "and":
                            line = line.Replace(temp, "#__");
                            break;
                        case "or":
                            line = line.Replace(temp, "#___");
                            break;
                    }
                }              

            }
            else
            {
                string[] array = new string[] { "_", "#_", "#__", "#___" };
                foreach (string value in array)
                {
                    
                    switch (value)
                    {
                        case "xor":
                            line = line.Replace(value, " xor ");
                            break;
                        case "not":
                            line = line.Replace(value, "not ");
                            break;
                        case "and":
                            line = line.Replace(value, " and ");
                            break;
                        case "or":
                            line = line.Replace(value, " or ");
                            break;
                    }
                }
            }
            

           
           
            return line;
        }

        public void PrintRepairProgram()
        {
            int ind = 0;
            try
            {
                bool tc = string.IsNullOrEmpty(File.ReadAllText(IdentificatorsPath));
                bool ic = string.IsNullOrEmpty(File.ReadAllText(TokensPath));
                if (!tc && !ic)
                {
                    using (StreamReader reader = new StreamReader(TokensPath))
                    {
                        string? line = File.ReadLines(TokensPath).Last();
                        ind = int.Parse(line.Split("##")[1]);
                        ind += 1;
                    }
                    for (int i = 0; i < ind; i++)
                    {
                        string line = RepairProgramLine(i);
                        Console.WriteLine(line);
                    }
                }

             
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
           
        }

        
        /// <summary>
        /// group G1
        /// </summary>
        /// <returns></returns>
        public List<string> ReplaseValues()
        {
            return null;
        }
        /// <summary>
        /// group G1
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public double CalculateExpression(string exp)
        {
            double result = 0;

            return result;
        }

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
            //errors = CheckCorrectAssign();
            //if (errors.Count > 0)
            //{
            //    flag = false;
            //}
            errors = CheckInicializeVariables();
            if (errors.Count>0)
            {
                flag = false;
                foreach (Error error in errors)
                {
                    Console.WriteLine(error.Message);
                }
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
            errors = CheckDelimiters();
            if (errors.Count > 0)
            {
                flag = false;
                foreach (Error error in errors)
                {
                    Console.WriteLine(error.Message);
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

        private List<Error> CheckDelimiters()
        {
            List<Error> errors = new List<Error>();
            int n = GetProgramSize();
            for (int i=0; i<n; i++)
            {
                string line = RepairProgramLine(i);
                if (!(line.Contains("begin") || line.Equals("if") || line.Equals("else") || line.Equals("while") ))
                {
                    if (!line.Last().Equals(";"))
                    {
                        Error error = new Error();
                        error.Col = i;
                        error.Message = "please add symbol \";\" in the end line " + i;
                        errors.Add(error);
                    }

                }
            }
            return errors;
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
                    bool IsExist = false;
                    foreach (IdentTypes item in dictionary)
                    {
                        if (item.Identificator.Equals(identificators[i].Value))
                        {
                            IsExist = true;
                            break;
                            //break;
                        }
                    }
                    if (!IsExist)
                    {
                        Error error = new Error();
                        error.Col = identificators[i].Column;
                        error.Row = identificators[i].Row;
                        errors.Add(error);
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
       
        private List<Error> CheckInicializeVariables()
        {
            List<Error> errors = new List<Error>();
            int n = GetProgramSize();
            int body = 0;
            List<IdentTypes> variables = new List<IdentTypes>();
            for (int i = 0; i < n; i++)
            {
                string line = RepairProgramLine(i);
                if (line.Equals("begin"))
                {
                    body = i + 1;
                    break;
                }
                else
                {
                    if (line.Contains("var"))
                    {
                        line = line.Remove(line.IndexOf("var"), 4);
                    }

                    string[] args = line.Split(':')[0].Split(',');
                    string type = line.Split(':')[1];
                    if (type.IndexOf(';') >= 0)
                    {
                        type = type.Remove(type.IndexOf(';', 1));
                    }
                    for (int j = 0; j < args.Length; j++)
                    {
                        args[j] = args[j].Trim();
                        IdentTypes item = new IdentTypes();
                        item.Identificator = args[j];
                        item.Type = type;
                        variables.Add(item);
                    }
                }
               
            }
            bool[] flags = new bool[variables.Count];
            for (int j = 0; j < flags.Length; j++)
            {
                flags[j] = false;
            }
            for (int i=body; i<n; i++)
            {
                string line = RepairProgramLine(i);
                line = line.Trim();
                if (line.StartsWith("write"))
                {
                    if (line.Contains(":="))
                    {
                        string arg = line.Split(":=")[0];

                        for (int j = 0; j < variables.Count; j++)
                        {
                            if (arg.Equals(variables[j].Identificator))
                            {
                                flags[j] = true;
                            }
                        }
                    }
                    if (line.Contains("read"))
                    {
                        string[] args = GetSubsString(line, line.IndexOf("("), line.IndexOf(")")).Split(',');
                        for (int k = 0; k < args.Length; k++)
                        {
                            args[k] = args[k].Trim();
                            for (int j = 0; j < variables.Count; j++)
                            {
                                if (args[k].Equals(variables[j].Identificator))
                                {
                                    flags[j] = true;
                                }
                            }
                        }


                    }
                }

            }
            for (int j=0; j<flags.Length; j++)
            {
                if (!flags[j])
                {
                    Error error = new Error();
                    error.Message = "find non inicialize variable " + variables[j].Identificator;
                    errors.Add(error);
                }
            }
            
            return errors;
        }

        /// <summary>
        /// group G1
        /// </summary>
        /// <returns></returns>
        private List<Error> CheckCorrectDiapason()
        {
            //check out of range for array and numbers, for example
            //a: array [0..12] of integer;
            //b: byte;
            //begin
            //a[45]:=65;
            //b:=4558;
            //end
            List<Error> errors = new List<Error>();
            int n = GetProgramSize();
            int body = 0;
            List<IdentTypes> variables = new List<IdentTypes>();
            for (int i=0; i<n; i++)
            {
                string line = RepairProgramLine(i);
                if (line.Contains("var"))
                {
                    line = line.Remove(line.IndexOf("var"), 4);                    
                }
                string[] args = line.Split(':')[0].Split(',');
                string type = line.Split(':')[1];
                if (type.IndexOf(';') >= 0)
                {
                    type = type.Remove(type.IndexOf(';', 1));
                }
                for (int j = 0; j < args.Length; j++)
                {
                    args[j] = args[j].Trim();
                    IdentTypes item = new IdentTypes();
                    item.Identificator = args[j];
                    item.Type = type;
                    variables.Add(item);
                }
                if (line.Equals("begin"))
                {
                    body = i + 1;
                    break;
                }  
            }
            for (int i=body; i<n; i++)
            {
                string line = RepairProgramLine(i);
                foreach (IdentTypes item in variables)
                {
                    List<int> indexes = GetAllContains(line, item.Identificator);
                    foreach (int index in indexes)
                    {
                        
                        if (IsAllowSymbol(line[index-1]) && IsAllowSymbol(line[item.Identificator.Length+1]))
                        {
                            if (line.Contains(":="))
                            {
                                string value = line.Split(":=")[1];
                                List<string> tmp = new List<string>();
                                string temp = "";
                                for (int j=0; j<value.Length; j++)
                                {
                                    
                                    if (value[j].Equals('+') || value[j].Equals('-') || value[j].Equals('/') || value[j].Equals('*'))
                                    {
                                        tmp.Add(temp);
                                    }
                                    else
                                    {
                                        if (!value[j].Equals('(') || !value[j].Equals(')'))
                                        {
                                            temp += value[j];
                                        }
                                    }                                  
                                }
                              
                                foreach (string numb in tmp)
                                {
                                    int arg0 = 0;
                                    double arg1 = 0;
                                    bool arg2 = true;
                                    switch (item.Type)
                                    {
                                        case "shortint":
                                            bool f = int.TryParse(numb, out arg0);
                                            bool IsCorrect = false;
                                            if (f)
                                            {
                                                if (!(arg0 >= -128 && arg0 <= 127))
                                                {
                                                    IsCorrect = true;
                                                }
                                            }
                                            else
                                            {
                                                IsCorrect = true;                                                                                            
                                            }
                                            if (IsCorrect)
                                            {
                                                Error error = new Error();
                                                error.Message = "uncorrect assign";
                                                error.Col = i;
                                                if (errors.Count>0)
                                                {
                                                    if (errors.Last().Col!=i)
                                                    {
                                                        errors.Add(error);
                                                    }
                                                }
                                                else
                                                {
                                                    errors.Add(error);
                                                }
                                                
                                            }
                                            break;
                                        case "smallint":
                                            f = int.TryParse(numb, out arg0);
                                            IsCorrect = false;
                                            if (f)
                                            {
                                                if (!(arg0 >= -32768 && arg0 <= 32767))
                                                {
                                                    IsCorrect = true;
                                                }
                                            }
                                            else
                                            {
                                                IsCorrect = true;
                                            }
                                            if (IsCorrect)
                                            {
                                                Error error = new Error();
                                                error.Message = "uncorrect assign";
                                                error.Col = i;
                                                if (errors.Count > 0)
                                                {
                                                    if (errors.Last().Col != i)
                                                    {
                                                        errors.Add(error);
                                                    }
                                                }
                                                else
                                                {
                                                    errors.Add(error);
                                                }

                                            }
                                            break;
                                        case "integer":
                                            f = int.TryParse(numb, out arg0);
                                            IsCorrect = false;
                                            if (f)
                                            {
                                                if (!(arg0 >= int.MinValue && arg0 <= int.MaxValue))
                                                {
                                                    IsCorrect = true;
                                                }
                                            }
                                            else
                                            {
                                                IsCorrect = true;
                                            }
                                            if (IsCorrect)
                                            {
                                                Error error = new Error();
                                                error.Message = "uncorrect assign";
                                                error.Col = i;
                                                if (errors.Count > 0)
                                                {
                                                    if (errors.Last().Col != i)
                                                    {
                                                        errors.Add(error);
                                                    }
                                                }
                                                else
                                                {
                                                    errors.Add(error);
                                                }

                                            }
                                            break;
                                        case "longint":
                                            //-2 147 483 648 - 2 147 483 647
                                            break;
                                        case "int64":
                                            //-9223372036854775808..9223372036854775807;
                                            break;
                                        case "byte":
                                            //0-255
                                            break;
                                        case "word":
                                            //0-65536
                                            break;
                                        case "longword":
                                            // 0..4294967295
                                            break;
                                        case "cardinal":
                                            // type Cardinal = 0..4294967295;
                                            break;
                                        case "uint64":
                                            //0..18446744073709551615
                                            break;
                                        case "BigInteger":
                                            //dynamic 
                                            break;
                                        case "real":
                                        case "double":
                                            //	-1.8∙10^308 .. 1.8∙10^308
                                            break;
                                        //case "double":
                                        //    //	-1.8∙10^308 .. 1.8∙10^308
                                        //    break;
                                        case "single":
                                            //-3.4∙10^38..3.4∙10^38
                                            break;
                                        case "decimal":
                                            //	-79228162514264337593543950335 .. 79228162514264337593543950335
                                            break;
                                        case "boolean":
                                            IsCorrect = bool.TryParse(numb, out arg2);
                                            if (!IsCorrect)
                                            {
                                                Error error = new Error();
                                                error.Message = "uncorrect assign";
                                                error.Col = i;
                                                if (errors.Count > 0)
                                                {
                                                    if (errors.Last().Col != i)
                                                    {
                                                        errors.Add(error);
                                                    }
                                                }
                                                else
                                                {
                                                    errors.Add(error);
                                                }
                                            }
                                            break;
                                        //case "string":
                                            
                                        //    break;
                                        case "char":
                                            IsCorrect = numb.Length == 1;
                                            if (!IsCorrect)
                                            {
                                                Error error = new Error();
                                                error.Message = "uncorrect assign";
                                                error.Col = i;
                                                if (errors.Count > 0)
                                                {
                                                    if (errors.Last().Col != i)
                                                    {
                                                        errors.Add(error);
                                                    }
                                                }
                                                else
                                                {
                                                    errors.Add(error);
                                                }
                                            }
                                            break;
                                    }
                                }
                               
                            }
                            //if (line.Contains(":="))
                            //{
                            //    int j = line.IndexOf(":=");
                            //    while (IsDecNumber(line[j+1]))
                            //    {

                            //    }
                                
                                
                            //}
                        }
                       
                                          
                    }
                }
            }
            //логические операторы and, or, not, xor
            //для целых/вещественных +,-,*,/
            //для целых div, mod
            //проверить корректное присвоение переменных и операции с ними
            bool IsAllowSymbol(char c)
            {
                return c.Equals(':') || c.Equals('*') || c.Equals('/') || c.Equals('+') || c.Equals('-') || c.Equals(' ');
            }
            bool IsDecNumber(char c)
            {
                char[] patterns = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };
                return patterns.Contains(c);
            }
            bool IsOctValue(char c)
            {
                char[] patterns = new char[] { '0', '1', '3', '4', '5', '6', '7' };
                return patterns.Contains(c);
            }
            bool IsHexValue(char c)
            {
                char[] patterns = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
                return patterns.Contains(c);
            }
            bool IsIntNumber(string val, string type)
            {
                bool flag = true;
                int number;
                if (int.TryParse(val, out number))
                {
                    switch (type)
                    {
                       
                    }
                }
                
                return flag;
            }
            bool IsFloatNumber(string val, string type)
            {
                bool flag = true;
                switch (type)
                {
                    case "real":
                        break;
                    case "double":
                        break;
                    case "single":
                        break;
                    case "decimal":
                        break;
                }
                return flag;
            }
            bool IsBooleanNumber(string val)
            {
                bool f;
                return bool.TryParse(val,out f);
            }
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


       private List<int> GetAllEnters(string line, string value)
       {
            List<int> indexes = new List<int>();
            List<int> result = new List<int>();
            int ind = line.IndexOf(value);
            while (ind >= 0)
            {
                indexes.Add(ind);
                ind = line.IndexOf(value, ind + value.Length);
            }
            foreach (int index in indexes)
            {
                bool f = false;
                if (ind-1>=0)
                {
                    if (IsAllowSymbol(line[ind-1]))
                    {
                        f = true;
                    }
                    
                }
                if (f)
                {
                    if (ind + 1 < line.Length)
                    {
                        if (IsAllowSymbol(line[ind + 1]))
                        {
                            result.Add(index);
                        }
                    }
                }
                
            }

            bool IsAllowSymbol(char c)
            {
                return Regex.IsMatch(c.ToString(), @"^[0-9]*$") || line[0].Equals('_')
                    || Regex.IsMatch(c.ToString(), @"^[a-zA-Z]+$")
                    || c.Equals('$') || c.Equals('&') || c.Equals('&') ||c.Equals(' ');
            }

            return indexes;
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
            flag = !Regex.IsMatch(line[0].ToString(), @"^[0-9]*$");
            if (flag)
            {
                flag = line[0].Equals('_') || Regex.IsMatch(line[0].ToString(), @"^[a-zA-Z]+$");
            }
            if (flag)
            {
                flag = !line[0].Equals("&") && !line[0].Equals("$") && !line[0].Equals("%");
            }
            if (flag)
            {
                string arg = line.Remove(0, 1);
                if (flag)
                {
                    flag = Regex.IsMatch(arg, @"^[a-zA-Z0-9]*$") || arg.Contains("_");
                }
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

        public List<string> ReadProgramFromFile(string path)
        {
            List<string> program = new List<string>();
            try{
                using (StreamReader reader = new StreamReader(path))
                {
                    string? line;
                    while (!string.IsNullOrEmpty(line=reader.ReadLine()))
                    {
                        program.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return program;
        }
    }
}