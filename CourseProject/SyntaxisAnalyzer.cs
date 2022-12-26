
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using static CourseProject.BinaryTree;

namespace CourseProject
{
    public class SyntaxisAnalyzer : Service
    {

        protected List<string> program = new();

        private class Element
        {
            public string Name { get; set; }

            public int StartIndex { get; set; }
            public Element(int si, string val)
            {
                this.StartIndex = si;
                this.Name = val;
            }

        }

        protected class Error
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

        public BinaryTree FormBinaryTree()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            BinaryTree tree = new BinaryTree();
            tree.SetRootValue("start");
            int n = program.Count;

            int index = 0;
            try
            {
                
                string line = program[index];
                bool flag = true;
                while (!line.Contains("begin"))
                {

                    if (line.StartsWith("var"))
                    {
                        if (flag)
                        {
                            tree.AddLeftChild("var");
                            flag = false;
                        }
                        
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
                    }
                    index++;
                    line = program[index];
                }
                tree.SetHead();

                int opIondex = 0;
                
                for (int i=index; i<n; i++)
                {
                    line = program[i];
                    if (line.Equals("begin"))
                    {
                        tree.AddRightChild(DefaultValue + opIondex);
                        tree.AddLeftChild("begin");
                        tree.SetParent(tree.Root);
                        opIondex++;
                    }
                    if (line.StartsWith("end"))
                    {
                        tree.AddRightChild(DefaultValue + opIondex);
                        tree.AddLeftChild(line);
                        tree.SetParent(tree.Root);
                        opIondex++;
                    }
                    if (line.StartsWith("if"))
                    {
                        string condition = GetSubsString(line, line.IndexOf('('), line.LastIndexOf(')'));
                        BinaryTree condTree = ParseExpression(condition);
                        tree.AddRightChild(DefaultValue + opIondex);
                        tree.AddLeftChild("if");
                        tree.SetLeftChild(condTree.Root);
                        tree.SetParent(tree.Root);
                        opIondex++;
                    }
                    if (line.Equals("then"))
                    {
                        tree.AddRightChild(DefaultValue + opIondex);
                        tree.AddLeftChild(line);
                        tree.SetParent(tree.Root);
                        opIondex++;
                    }
                    if (line.Equals("else"))
                    {
                        tree.AddRightChild(DefaultValue + opIondex);
                        tree.AddLeftChild(line);
                        tree.SetParent(tree.Root);
                        opIondex++;
                       
                    }
                    if (line.Equals("repeat"))
                    {
                        tree.AddRightChild(DefaultValue + opIondex);
                        tree.AddLeftChild(line);
                        tree.SetParent(tree.Root);
                        opIondex++;
                    }
                    if (line.StartsWith("until"))
                    {
                        string condition = GetSubsString(line, line.IndexOf('('), line.LastIndexOf(')'));
                        BinaryTree condTree = ParseExpression(condition);
                        tree.AddRightChild(DefaultValue + opIondex);
                        tree.AddLeftChild("until");
                        tree.SetLeftChild(condTree.Root);
                        tree.SetParent(tree.Root);
                        opIondex++;
                    }
                    if (line.Contains(":="))
                    {
                        string arg0 = line.Split(":=")[0];
                        bool f1 = false;
                        string a00 = ""; string a01 = "";
                        if (arg0.Contains('[') && arg0.Contains(']'))
                        {
                            a00 = arg0.Substring(0, arg0.IndexOf('['));
                            a01 = arg0.Substring(arg0.IndexOf('[')+1);
                            a01 = a01.Remove(a01.Length - 1, 1);
                            a00 += "[index]";
                            f1 = true;
                        }

                        string arg1 = line.Split(":=")[1];
                        if (arg1.Last().Equals(';'))
                        {
                            arg1 = arg1.Remove(arg1.Length - 1, 1);
                            arg1 = AddBrackets(arg1);
                        }

                        BinaryTree exprTree = new BinaryTree();
                        if (!IsCorrectValue(arg1))
                        {
                            exprTree = ParseExpression(arg1);
                        }
                        
                        tree.AddRightChild(DefaultValue + opIondex);
                        tree.AddLeftChild(":=");
                        if (f1)
                        {
                            tree.AddLeftChild(a00);
                            if (!IsCorrectValue(a01))
                            {
                                BinaryTree eTree = ParseExpression(a01);
                                tree.SetLeftChild(eTree.Root);
                                
                            }
                            else
                            {
                                tree.AddLeftChild(a01);
                                tree.SetParent(tree.Root);
                            }
                            
                            tree.SetParent(tree.Root);
                        }
                        else
                        {
                            tree.AddLeftChild(arg0);
                            tree.SetParent(tree.Root);
                        }
                        if (!IsCorrectValue(arg1))
                        {
                            tree.SetRightChild(exprTree.Root);
                        }
                        else
                        {
                            tree.AddRightChild(arg1);
                            tree.SetParent(tree.Root);
                        }
                        
                        tree.SetParent(tree.Root);
                        opIondex++;
                    }
                    if (line.StartsWith("write"))
                    {
                        string arg0 = "write";
                        if (line.StartsWith("writeln"))
                        {
                            arg0 = "writeln";
                        }
                        string arg1 = GetSubsString(line, line.IndexOf('('), line.LastIndexOf(')'));
                        tree.AddRightChild(DefaultValue + opIondex);
                        tree.AddLeftChild(arg0);
                        tree.AddLeftChild(arg1);
                        tree.SetParent(tree.Root);
                        tree.SetParent(tree.Root);
                        opIondex++;
                    }
                    if (line.StartsWith("read"))
                    {
                        string arg0 = "read";
                        if (line.StartsWith("readln"))
                        {
                            arg0 = "readln";
                        }
                        string[] inputArgs = GetSubsString(line, line.IndexOf('(') + 1, line.LastIndexOf(')') - 1).Split(',');
                        tree.AddRightChild(DefaultValue + opIondex);
                        tree.AddLeftChild(arg0);
                        for (int j=0; j<inputArgs.Length; j++)
                        {
                            tree.AddLeftChild(inputArgs[j]);
                        }
                        for (int j = 0; j < inputArgs.Length; j++)
                        {
                            tree.SetParent(tree.Root);
                        }
                        tree.SetParent(tree.Root);
                        opIondex++;
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


        protected BinaryTree ParseExpression(string line)
        {
            BinaryTree tree = new BinaryTree();
            Console.ForegroundColor = ConsoleColor.Red;
            try
            {
                if (!string.IsNullOrEmpty(line))
                {
                    line = AddBrackets(line);
                    //if (line.First().Equals('(') && line.Last().Equals(')'))
                    //{
                    //    line = line.Substring(1);
                    //    line = line.Remove(line.Length - 1, 1);
                    //}
                    int ind = 0;

                    string temp = line;

                    List<Operator> operators = new List<Operator>();

                    List<int> indexes = GetAllContains(line, "^");

                    foreach (int index in indexes)
                    {

                        operators.Add(new Operator("^", index));
                    }

                    indexes = GetAllContains(line, "*");

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
                        if (index - 1 >= 0)
                        {
                            if (!line[index - 1].Equals('<') && !line[index - 1].Equals('>'))
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
                                if (index - 1 >= 0)
                                {
                                    if (!line[index - 1].Equals('<'))
                                    {
                                        operators.Add(new Operator(">", index));
                                    }
                                }

                            }
                        }

                    }

                    indexes = GetAllContains(line, "div");

                    foreach (int index in indexes)
                    {
                        operators.Add(new Operator("div", index));
                    }

                    indexes = GetAllContains(line, "mod");

                    foreach (int index in indexes)
                    {
                        operators.Add(new Operator("mod", index));
                    }

                    indexes = GetAllContains(line, "xor");

                    foreach (int index in indexes)
                    {
                        operators.Add(new Operator("xor", index));
                    }

                    indexes = GetAllContains(line, "or");

                    foreach (int index in indexes)
                    {
                        if (index - 1 >= 0)
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
                        for (int j = 0; j < operators.Count; j++)
                        {
                            if (operators[j].Position == ind)
                            {
                                index = j;
                                operators[j].Flag = false;
                                break;
                            }
                        }

                        if (index >= 0)
                        {
                            string op = operators[index].Value;
                            ind += op.Length - 1;
                            tree.SetRootValue(op);
                            tree.AddRightChild("");

                        }
                        if (IsValue(line[ind]) && (index == -1))
                        {
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


        protected bool IsValue(char c)
        {
            return Regex.IsMatch(c.ToString(), "^[0-9]+$") || Regex.IsMatch(c.ToString(), "^[a-zA-Z]+$") || c.Equals('.') || c.Equals('_');
        }

        string[] operators = new string[] { "not", "and", "^", "/", "*", "div", "mod", "and", "+", "-", "or", "xor", "=", "<>", "<", ">", "<=", ">=", "in" };

        string[] array = new string[] { "not#", "^", "*", "/", "#div#", "#mod#", "#and#", "+", "-", "#or#", "#xor#", "=", "<>", "<", ">", "<=", ">=", "#in#" };

        char[] patterns = new char[] { '^', '*', '/', '+', '-', '(', ')', '<', '>', '=', '#' };

        protected string AddBrackets(string line)
        {

            line = ChangeSymbols(line);

            List<string> lines = new List<string>();

            if (line.First().Equals('(') && line.Last().Equals(')'))
            {
                line = line.Substring(1);
                line = line.Remove(line.Length - 1, 1);
            }


            bool IsHaveBrackets = line.Contains('(') && line.Contains(')');

        

            List<Item> indexes = FormIndexes();

            int n = indexes.Count;

            foreach (string op in array)
            {
                if (!op.Equals("#xor#") && !op.Equals("not#") && !op.Equals("#and#") && !op.Equals("#or#") && !op.Equals("#div#") && !op.Equals("#mod#") && !op.Equals("#in#")) 
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

                else
                {
                    if (line.Contains("#xor#") || line.Contains("not#") || line.Contains("#and#") || line.Contains("#or#") || line.Contains("#div#") || line.Contains("#mod#") || line.Contains("#in#"))
                    {
                        if (IsHaveBrackets)
                        {
                            
                            line = line.Substring(1);
                            line = line.Remove(line.Length - 1, 1);
                            IsHaveBrackets = false;
                        }
                       
                    }
                    switch (op)
                    {
                        case "#xor#":
                        case "#and#":
                        case "#or#":
                        case "#div#":
                        case "#mod#":
                        case "#in#":
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
                                    else
                                    {
                                        f1 = false;
                                    }
                                    //ind = indexes[i].Position;
                                    tmp = ind + op.Length;
                                    br = 0;
                                    while (tmp < line.Length)
                                    {
                                        if (line[tmp].Equals('('))
                                            br++;
                                        if (line[tmp].Equals(')'))
                                            br--;
                                        if (br == 0)
                                            if (patterns.Contains(line[tmp]))
                                                break;
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
                                    else
                                    {
                                        f2 = false;
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
                            break;
                        case "not#":
                            for (int i = 0; i < n; i++)
                            {
                                if (indexes[i].Value.Equals(op))
                                {
                                    indexes = FormIndexes();
                                    int ind = indexes[i].Position;
                                    sbyte br = 0;
                                    bool f1 = true; bool f2 = true;
                                    int lp = -1; int rp = -1;
                                    int tmp = ind;
                                    lp = tmp;
                                    if (tmp - 1 >= 0)
                                    {
                                        f1 = line[tmp - 1].Equals('(');
                                    }
                                    else
                                    {
                                        f1 = false;
                                    }
                                    tmp = ind + op.Length;
                                    while (tmp < line.Length)
                                    {
                                        if (line[tmp].Equals('('))
                                            br++;
                                        if (line[tmp].Equals(')'))
                                            br--;
                                        if (br == 0)
                                            if (patterns.Contains(line[tmp]))
                                                break;
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
                                    else
                                    {
                                        f2 = false;
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
                            break;
                    }                  
                }
                
            }

            line = ChangeSymbols(line, false);
            

            bool IsSpecificSymbol(int index)
            {
                bool f = line[index].Equals('+') || line[index].Equals('-') || line[index].Equals('*')
                    || line[index].Equals('/') || line[index].Equals('<') || line[index].Equals('^')
                    || line[index].Equals('>') || line[index].Equals('=') || line[index].Equals('#');
                return f;
            }

            List<Item> FormIndexes()
            {
                List<Item> indexes = new List<Item>();
                List<int> data = new List<int>();
                foreach (string delimiter in array)
                {
                    if (delimiter.Equals("<") || delimiter.Equals(">") || delimiter.Equals("=")) 
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

        public void Optimizeprogram()
        {
            int n = GetProgramSize();
            int body = 0;
            for (int i=0; i<program.Count; i++)
            {
                string line = program[i];
                if (line.Contains('[') && line.Contains(']') && line.Contains(".."))
                {
                    int si = line.IndexOf('[');
                    int count = 0;
                    for (int j=si+1; j<line.Length; j++)
                    {
                        if (line[j] == ' ')
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    line = line.Remove(si + 1, count);
                    si = line.IndexOf(']');
                    count = 0;
                    for (int j=si-1; j>=0; j--)
                    {
                        if (line[j].Equals(' '))
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    line = line.Remove(si+1, count);
                    program[i] = line;
                }
                if (line.Equals("begin"))
                {
                    body = i + 1;
                    break;
                }
            }
           
            for (int i=body; i<n; i++)
            {
                string line = program[i];
                if (line.StartsWith("if"))
                {
                    string arg1 = line.Remove(0, 2);
                    arg1 = RemoveLeftSpaces(arg1);
                    arg1 = Parse(arg1);
                    program[i] = "if " + arg1;
                }
                if (line.StartsWith("until"))
                {
                    string arg1 = line.Remove(0, 5);
                    arg1 = RemoveLeftSpaces(arg1);
                    arg1 = Parse(arg1);
                    program[i] = "until " + arg1;
                }
                if (line.Contains(":="))
                {
                    string arg0 = line.Split(":=")[0];
                    if (arg0.Contains('[') && arg0.Contains(']'))
                    {
                        string a00 = arg0.Substring(0, arg0.IndexOf('[') );
                        string a01 = arg0.Substring(arg0.IndexOf('[')+1);
                        a01 = a01.Remove(a01.Length - 1, 1);
                        a01 = Parse(a01);
                        a01 = AddBrackets(a01);
                        arg0 = a00 + '[' + a01 + ']';
                       
                    }
                    string arg1 = line.Split(":=")[1];
                    arg1 = Parse(arg1);
                    program[i] = arg0 + ":=" + arg1;
                }
                
            }
            

            string Parse(string arg1)
            {
                int j = 0;
                while (j < arg1.Length)
                {
                    if (patterns.Contains(arg1[j]))
                    {
                        int si = 0;
                        int count = 0;
                        if (arg1[j].Equals('='))
                        {
                            if (j-1>=0)
                            {
                                if (arg1[j - 1].Equals('<') || arg1[j - 1].Equals('>'))
                                {
                                    if (arg1[j - 2].Equals(' '))
                                    {
                                        si = j - 2;
                                        count = 0;
                                        while (si >= 0)
                                        {
                                            if (arg1[si].Equals(' '))
                                            {
                                                count++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                            si--;
                                        }

                                        arg1 = arg1.Remove(si, count);
                                    }

                                }
                                else
                                {
                                    if (arg1[j - 1].Equals(' '))
                                    {
                                        si = j - 1;
                                        count = 0;
                                        while (si >= 0)
                                        {
                                            if (arg1[si].Equals(' '))
                                            {
                                                count++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                            si--;
                                        }
                                        arg1 = arg1.Remove(si, count);
                                    }
                                }
                            }
                            if (j+1<arg1.Length)
                            {
                                if (arg1[j + 1].Equals(' '))
                                {
                                    si = j + 1;
                                    count = 0;
                                    while (si < arg1.Length)
                                    {
                                        if (arg1[si].Equals(' '))
                                        {
                                            count++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        si++;
                                    }
                                    arg1 = arg1.Remove(j + 1, count);
                                }
                            }
                            
                        }
                        if (arg1[j].Equals('<'))
                        {
                            if (j+1<arg1.Length)
                            {
                                if (arg1[j + 1].Equals('>') || arg1[j + 1].Equals('='))
                                {
                                    if (arg1[j + 2].Equals(' '))
                                    {
                                        si = j + 2;
                                        count = 0;
                                        while (si < arg1.Length)
                                        {
                                            if (arg1[si].Equals(' '))
                                            {
                                                count++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                            si++;
                                        }
                                        arg1 = arg1.Remove(j + 2, count);
                                    }
                                }
                                else
                                {
                                    if (arg1[j + 1].Equals(' '))
                                    {
                                        si = j + 1;
                                        count = 0;
                                        while (si < arg1.Length)
                                        {
                                            if (arg1[si].Equals(' '))
                                            {
                                                count++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                            si++;
                                        }
                                    }
                                }
                            }
                            
                        }
                        if (arg1[j].Equals('>'))
                        {
                            if (j-1>=0)
                            {
                                if (arg1[j - 1].Equals('=') || arg1[j + 1].Equals('<'))
                                {
                                    if (arg1[j - 2].Equals(' '))
                                    {
                                        si = j - 2;
                                        count = 0;
                                        while (si >= 0)
                                        {
                                            if (arg1[si].Equals(' '))
                                            {
                                                count++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                            si--;
                                        }

                                        arg1 = arg1.Remove(si, count);
                                    }
                                }
                                else
                                {
                                    if (arg1[j - 1].Equals(' '))
                                    {
                                        si = j - 1;
                                        count = 0;
                                        while (si >= 0)
                                        {
                                            if (arg1[si].Equals(' '))
                                            {
                                                count++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                            si--;
                                        }
                                        arg1 = arg1.Remove(si, count);
                                    }
                                }
                            }
                           if (j+1<arg1.Length)
                           {
                                if (arg1[j + 1].Equals(' '))
                                {
                                    si = j + 1;
                                    count = 0;
                                    while (si < arg1.Length)
                                    {
                                        if (arg1[si].Equals(' '))
                                        {
                                            count++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        si++;
                                    }
                                    arg1 = arg1.Remove(j + 1, count);
                                }
                            }
                            
                        }
                        if (!arg1[j].Equals('=') && !arg1[j].Equals('<') && !arg1[j].Equals('>'))
                        {
                            if (j+1<arg1.Length)
                            {
                                if (arg1[j + 1].Equals(' '))
                                {
                                    si = j + 1;
                                    count = 0;
                                    while (si < arg1.Length)
                                    {
                                        if (arg1[si].Equals(' '))
                                        {
                                            count++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        si++;
                                    }
                                    arg1 = arg1.Remove(j - 1, count);
                                }
                            }
                            if (j-1>=0)
                            {
                                if (arg1[j - 1].Equals(' '))
                                {
                                    si = j - 1;
                                    count = 0;
                                    while (si >= 0)
                                    {
                                        if (arg1[si].Equals(' '))
                                        {
                                            count++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        si--;
                                    }
                                    arg1 = arg1.Remove(si, count);
                                }
                            }
                            
                        }
                    }
                    j++;
                }
                return arg1;
            }
     
        }

        private string ChangeSymbols(string line, bool flag = true)
        {

            if (flag)
            {
                string[] array = new string[] { "xor", "not", "and", "or", "div", "mod", "in" };
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
                            line = line.Replace(temp, "#xor#");
                            break;
                        case "not":
                            line = line.Replace(temp, "not#");
                            break;
                        case "and":
                            line = line.Replace(temp, "#and#");
                            break;
                        case "or":
                            line = line.Replace(temp, "#or#");
                            break;
                        case "div":
                            line = line.Replace(temp, "#div#");
                            break;
                        case "mod":
                            line = line.Replace(temp, "#mod#");
                            break;
                        case "in":
                            line = line.Replace(temp, "#in#");
                            break;
                    }
                }

            }
            else
            {
                string[] array = new string[] { "#xor#", "not#", "#and#", "#or#", "#div#", "#mod#", "#in#" };
                foreach (string value in array)
                {

                    switch (value)
                    {
                        case "#xor#":
                            line = line.Replace(value, " xor ");
                            break;
                        case "not#":
                            line = line.Replace(value, "not ");
                            break;
                        case "#and#":
                            line = line.Replace(value, " and ");
                            break;
                        case "#or#":
                            line = line.Replace(value, " or ");
                            break;
                        case "#div#":
                            line = line.Replace(value, " div ");
                            break;
                        case "#mod#":
                            line = line.Replace(value, " mod ");
                            break;
                        case "#in#":
                            line = line.Replace(value, " in ");
                            break;
                    }
                }
            }
            return line;
        }

       
        public void PrintProgram()
        {
            if (program.Count>0)
            {
               foreach (string line in program)
                {
                    Console.WriteLine(line);
                }
            }
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
                    Console.WriteLine("find uncorrect indentificator {0}, line is {1}", error.Message, error.Col);
                }
            }
            errors = CheckCorrectAssign();
            if (errors.Count > 0)
            {
                flag = false;
                foreach (Error error in errors)
                {
                    Console.WriteLine("find uncorrect assign construction, line is {0}", error.Col);
                }
            }
            errors = CheckInicializeVariables();
            if (errors.Count>0)
            {
                flag = false;
                foreach (Error error in errors)
                {
                    Console.WriteLine(error.Message);
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
                            Console.WriteLine("please check count of pair symbols \"begin\" and \"end\"");
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
            errors = CheckArrayDeclaration();
            if (errors.Count>0)
            {
                flag = false;
                foreach (Error error in errors)
                {
                    Console.WriteLine(error.Message);
                }
            }
            errors = CheckCorrectDeclaredOfOperators();
            if (errors.Count > 0)
            {
                flag = false;
                foreach (Error error in errors)
                {
                    Console.WriteLine(error.Message);
                }
            }
            errors = CheckLanguageConstructions();
            if (errors.Count > 0)
            {
                flag = false;
                foreach (Error error in errors)
                {
                    Console.WriteLine(error.Message);
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            return flag;
        }

        protected List<Error> CheckDelimiters()
        {
            List<Error> errors = new List<Error>();
            int n = program.Count;
            for (int i=0; i<n; i++)
            {
                string line = program[i];
                if (!(line.Contains("begin") || line.StartsWith("if") || line.Equals("then") || line.Equals("else") || line.StartsWith("repeat")))
                {
                    if (line.Equals("end"))
                    {
                        if (i + 1 < n)
                        {
                            if (!program[i + 1].Equals("else"))
                            {
                                AddError(i);
                            }
                        }
                        else
                        {
                            AddError(i);
                        }
                    }
                    else
                    {
                        if (!line.Last().Equals(';') && !line.Last().Equals('.'))
                        {
                            AddError(i);
                        }
                    }
                    

                }
            }
            void AddError(int ind)
            {
                bool f = true;
               
                if (errors.Count>0)
                {
                    for (int i=0; i<errors.Count; i++)
                    {                        
                        int t0 = errors[i].Col;
                        int t1 = errors[i].Row;
                        for (int j=i+1; j<errors.Count; j++)
                        {
                            if ((errors[j].Col == t0) && (errors[j].Row == t1))
                            {
                                f = false;
                                break;
                            }
                        }
                        if (!f)
                        {
                            break;
                        }
                    }
                        
                }
                if (f)
                {
                    Error error = new Error();
                    error.Col = ind;
                    error.Message = "please add symbol \";\" in the end line " + ind;
                    errors.Add(error);
                }
                
            }
            return errors;
        }

        protected List<Error> CheckCorrectAssign()
        {
            List<Error> errors = new List<Error>();
            for (int i=0; i<program.Count; i++)
            {
                string line = program[i];
                bool flag = true;   
                if (line.Contains(":="))
                {
                    string arg0 = line.Split(":=")[0];                   
                    if (!string.IsNullOrEmpty(arg0))
                    {
                        if (arg0.Contains('[') && arg0.Contains(']'))
                        {
                            if ((GetAllContains(arg0, "[").Count==1) && (GetAllContains(arg0, "]").Count ==1) )
                            {
                                if (arg0.IndexOf('[')<arg0.LastIndexOf(']'))
                                {
                                    string temp = arg0.Substring(0, arg0.IndexOf('['));
                                    string expr = GetSubsString(arg0, arg0.IndexOf('[') + 1, arg0.LastIndexOf(']') - 1);
                                    flag = IsCorrectIdentificator(temp) && IsCorrectExpression(temp);
                                }
                                else
                                {
                                    flag = false;
                                }
                            }
                            else
                            {
                                flag = false;
                            }
                            
                        }
                        else
                        {
                            if (!IsCorrectIdentificator(arg0))
                            {
                                flag = false;
                                
                            }
                        }
                       
                    }
                    else
                    {
                        flag = false;
                    }
                    string arg1 = line.Split(":=")[1];
                    if (arg1.Last().Equals(';'))
                    {
                        arg1 = arg1.Remove(arg1.Length - 1, 1);
                    }
                    if (!string.IsNullOrEmpty(arg1))
                    {
                        flag = IsCorrectExpression(arg1);
                    }
                    else
                    {
                        flag = false;
                    }
                    if (!flag)
                    {
                        Error error = new();
                        error.Col = i;
                        error.Message = "find ucorrect asign construction, line is: " + i;
                        errors.Add(error);
                    }
                }
            }
            return errors;
        }
        

        protected List<Error> CheckIdentificators()
        {
            List<Error> errors = new List<Error>();
            if (program.Count > 0)
            {
                List<string> values = new List<string>();
                bool IsExists = false;
                for (int i = 0; i < program.Count; i++)
                {
                    string line = program[i];
                    if (line.Equals("begin"))
                        break;
                    else
                    {
                        if (line.StartsWith("var"))
                        {
                            string[] args = line.Remove(0, 4).Split(':')[0].Split(',');
                            for (int j = 0; j < args.Length; j++)
                            {
                                args[j] = args[j].Trim();
                                bool f = true;
                                if (IsCorrectIdentificator(args[j]))
                                {
                                    if (!values.Contains(args[j]))
                                    {
                                        values.Add(args[j]);
                                    }
                                    else
                                    {
                                        f = false;
                                    }
                                }
                                if (!f)
                                {
                                    IsExists = true;
                                    Error error = new Error();
                                    error.Message = args[j];
                                    error.Col = i;
                                    errors.Add(error);
                                    break;
                                }
                               
                              
                            }
                            if (IsExists)
                            {
                                break;
                            }
                        }

                    }
                }

            }

          

            Console.ForegroundColor = ConsoleColor.White;
            return errors;
        }
       


        protected List<Error> CheckInicializeVariables()
        {
            List<Error> errors = new List<Error>();
            int n = program.Count;
            int body = 0;
            List<IdentTypes> variables = new List<IdentTypes>();
            for (int i = 0; i < n; i++)
            {
                string line = program[i];
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
                string line = program[i];
                line = line.Trim();
                if (line.Contains(":="))
                {
                    string arg = line.Split(":=")[0];
                    if (arg.Contains('[') && arg.Contains(']'))
                    {
                        arg = arg.Remove(arg.IndexOf('['));
                    }
                    for (int j = 0; j < variables.Count; j++)
                    {
                        if (arg.Equals(variables[j].Identificator))
                        {
                            flags[j] = true;
                        }
                    }
                }
                if (line.StartsWith("read"))
                {
                    string[] args = GetSubsString(line, line.IndexOf("(") + 1, line.IndexOf(")") - 1).Split(',');
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

        protected List<Error> CheckCorrectDeclaredOfOperators()
        {
            List<Error> errors = new List<Error>();
            int n = program.Count;
            int body = 0;
            for (int i=0; i<n; i++)
            {
                string line = program[i];
                if (line.Equals("begin"))
                {
                    body = i + 1;
                    break;
                }
            }
            for (int i=body; i<n; i++)
            {
                string line = program[i];
                if (line.Last().Equals(';'))
                {
                    line = line.Remove(line.Length - 1, 1);
                }
                foreach (string op in operators)
                {
                    List<int> indexes = GetAllContains(line, op);
                    bool flag = true;
                    if (op.Equals("not") || op.Equals("or") || op.Equals("<") || op.Equals(">") || op.Equals("=") || op.Equals("in"))
                    {
                        switch (op)
                        {
                            case "not":
                                foreach (int index in indexes)
                                {
                                    flag = true;
                                    if (!IsLocatedInSpaces(line, index))
                                    {
                                        if (index - 1 >= 0)
                                        {
                                            flag = line[index - 1].Equals(' ') || line[index - 1].Equals('=')
                                                || line[index - 1].Equals('(') || line.StartsWith("until") || line.StartsWith("if") || line.Contains(":=");
                                            if (flag)
                                            {
                                                if (index + 4 < line.Length)
                                                {
                                                    flag = line[index + 4].Equals(' ');
                                                }
                                                else
                                                {
                                                    flag = false;
                                                }
                                            }
                                        }
                                        
                                    }
                                    if (!flag)
                                    {
                                        AddError(i, op);
                                    }
                                }
                                break;
                            case "or":
                                foreach (int index in indexes)
                                {
                                    flag = true;
                                    if (!IsLocatedInSpaces(line, index))
                                    {
                                        if (index-1>=0)
                                        {
                                            flag = line[index - 1].Equals(' ') || line[index - 1].Equals('x');
                                            if (flag)
                                            {
                                                if (index+3<line.Length)
                                                {
                                                    flag = line[index + 3].Equals(' ');
                                                }
                                                else
                                                {
                                                    flag = false;
                                                }
                                            }
                                        }
                                    }
                                    if (!flag)
                                    {
                                        AddError(i,op);
                                    }
                                }
                                break;
                            case "<":
                                foreach (int index in indexes)
                                {
                                    flag = true;
                                    if (!IsLocatedInSpaces(line, index))
                                    {
                                        if (index - 1 >= 0)
                                        {
                                            flag = line[index - 1].Equals(' ') || CheckLastSymbolInIdentificator(line[index - 1]);
                                            if (flag)
                                            {
                                                if (index + 1 < line.Length)
                                                {
                                                    flag = line[index + 1].Equals(' ') || CheckLastSymbolInIdentificator(line[index + 1]) || line[index + 1].Equals('>');
                                                }
                                                else
                                                {
                                                    flag = false;
                                                }
                                            }
                                        }
                                    }
                                    if (!flag)
                                    {
                                        AddError(i, op);
                                    }
                                }
                                break;
                            case ">":
                                foreach (int index in indexes)
                                {
                                    flag = true;
                                    if (!IsLocatedInSpaces(line, index))
                                    {
                                        if (index - 1 >= 0)
                                        {
                                            flag = line[index - 1].Equals(' ') || line[index - 1].Equals('<') || CheckLastSymbolInIdentificator(line[index - 1]);
                                            if (flag)
                                            {
                                                if (index + 1 < line.Length)
                                                {
                                                    flag = line[index + 1].Equals('=') || CheckLastSymbolInIdentificator(line[index + 1]);
                                                }
                                                else
                                                {
                                                    flag = false;
                                                }
                                            }
                                        }
                                    }
                                    if (!flag)
                                    {
                                        AddError(i, op);
                                    }
                                }
                                break;
                            case "=":
                                foreach (int index in indexes)
                                {
                                    flag = true;
                                    if (!IsLocatedInSpaces(line, index))
                                    {
                                        if (index - 1 >= 0)
                                        {
                                            flag = line[index - 1].Equals('<') || line[index - 1].Equals('>') || line[index-1].Equals(':') || CheckLastSymbolInIdentificator(line[index - 1]);
                                            if (flag)
                                            {
                                                if (index + 1 < line.Length)
                                                {
                                                    flag = CheckLastSymbolInIdentificator(line[index + 1]);
                                                }
                                                else
                                                {
                                                    flag = false;
                                                }
                                            }
                                        }
                                    }
                                    if (!flag)
                                    {
                                        AddError(i,op);
                                    }
                                  
                                }
                                break;
                            case "in":                            
                                foreach (int index in indexes)
                                {
                                    flag = true;
                                    if (!line.Equals("begin"))
                                    {
                                        if (!IsLocatedInSpaces(line, index))
                                        {
                                            if (index - 1 >= 0)
                                            {
                                                flag = line[index - 1].Equals(' ');
                                                if (flag)
                                                {
                                                    if (index+3<line.Length)
                                                    {
                                                        flag = line[index + 3].Equals(' ');
                                                    }
                                                    else
                                                    {
                                                        flag = false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (!flag)
                                    {
                                        AddError(i,op);
                                    }
                                   
                                }
                                break;

                        }
                    }
                    else
                    {
                        foreach (int index in indexes)
                        {
                            flag = true;
                            if (!IsLocatedInSpaces(line, index))
                            {
                                if (index - 1 >= 0)
                                {
                                    flag = line[index - 1].Equals(' ') || CheckLastSymbolInIdentificator(line[index - 1]);
                                    if (flag)
                                    {
                                        if (index + op.Length < line.Length)
                                        {
                                            flag = line[index + op.Length].Equals(' ') || CheckLastSymbolInIdentificator(line[index + op.Length]);
                                        }
                                        else
                                        {
                                            flag = false;
                                        }
                                    }
                                    if (!flag)
                                    {
                                        AddError(i,op);
                                    }
                                }

                            }
                        }
                    }
                    
                  

                }
            }
            void AddError(int col, string opr)
            {
                bool fl = true;
                if (errors.Count>1)
                {
                    int minInd = errors[0].Col;
                    int maxInd = errors[0].Col;
                    for (int i=0; i<errors.Count; i++)
                    {
                        int t0 = errors[i].Col;
                        int t1 = errors[i].Row;
                        fl = true;
                        for (int j=i+1; j<errors.Count; j++)
                        {
                            if ((errors[j].Col == t0) && (errors[j].Row == t1))
                            {
                                fl = false;
                                break;
                            }
                        }
                        if (!fl)
                        {
                            break;
                        }
                    }
                }
                if (fl)
                {
                    Error error = new Error();
                    error.Message = "find uncorrect operator \""+opr+"\", line: " + col;
                    error.Col = col;
                    errors.Add(error);
                }
            
            }
            return errors;
        }

        private bool IsLocatedInSpaces(string line, int pos)
        {
            short ind = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (i == pos)
                {
                    break;
                }
                if (line[i] == '"')
                {
                    ind++;
                }
            }
            return ind % 2 == 1;
        }

        protected List<Error> CheckArrayDeclaration()
        {
            List<Error> errors = new List<Error>();
            for (int i=0; i<program.Count; i++)
            {
                bool f = true;
                string line = program[i];
                if (line.Equals("begin"))
                    break;
                if (line.Contains(':'))
                {
                    string arg1 = line.Split(':')[1];
                    arg1 = RemoveLeftSpaces(arg1);
                    if (arg1.StartsWith("array"))
                    {
                        if (arg1.Contains('[') && arg1.Contains(']'))
                        {
                            if (arg1.IndexOf('[') < arg1.IndexOf(']'))
                            {
                                string area = GetSubsString(arg1, arg1.IndexOf('[') + 1, arg1.IndexOf(']') - 1);
                                if (area.Contains(".."))
                                {
                                    string si = area.Split("..")[0];
                                    string ei = area.Split("..")[1];
                                    if (IsNumber(si) && IsNumber(ei))
                                    {
                                        string btype = line.Substring(line.IndexOf(']') + 1);
                                        btype = RemoveLeftSpaces(btype);
                                        if (btype.StartsWith("of"))
                                        {
                                            btype = btype.Remove(0, 2);
                                            btype = RemoveLeftSpaces(btype);
                                            if (btype.Last().Equals(';'))
                                            {
                                                btype = btype.Remove(btype.Length - 1, 1);
                                                f = IsContainsBaseType(btype);
                                            }
                                            else
                                            {
                                                f = false;
                                            }
                                        }
                                        else
                                        {
                                            f = false;
                                        }
                                    }
                                    else
                                    {
                                        f = false;
                                    }
                                }
                                else
                                {
                                    f = false;
                                }
                            }
                            else
                            {
                                f = false;
                            }
                        }
                        else
                        {
                            f = false;
                        }
                    }
                    if (!f)
                    {
                        Error error = new Error();
                        error.Message = "find uncorrect array construction, line " + i;
                        error.Col = i;
                        errors.Add(error);
                    }
                }
            }
            return errors;
        }

        protected bool IsCorrectNumber(string val)
        {
            bool f = true;
            try
            {
                double dval = double.Parse(val, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                f = false;
            }
            return f;
        }


        protected List<Error> CheckPairSymbols()
        {
            List<Error> errors = new List<Error>();
            sbyte br0, br1, br2;
            br2 = 0;
            for (int i=0; i<program.Count; i++)
            {
                string line = program[i];
                if (line.Last().Equals(';') || line.Last().Equals('.'))
                {
                    line = line.Remove(line.Length - 1, 1);
                }
                br0 = 0;
                br1 = 0;
                
                foreach (char c in line)
                {
                    if (br0>=0)
                    {
                        if (c.Equals('['))
                            br0++;
                        if (c.Equals(']'))
                        {
                            br0--;
                        }
                        
                    }                   
                    
                    if (br1>=0)
                    {
                        if (c.Equals('('))
                            br1++;
                        if (c.Equals(')'))
                        {
                            br1--;
                        }
                       
                    }

                }
                if (line.Equals("begin"))
                {
                    br2++;
                }
                if (line.Equals("end"))
                {
                    br2--;
                }
                if (br0!=0)
                {
                    Error error = new Error();
                    error.Col = i;
                    error.Message = "RECTANGLE_BRACKET";
                    errors.Add(error);
                }
                if (br1!=0)
                {
                    Error error = new Error();
                    error.Col = i;
                    error.Message = "CIRCLE_BRACKET";
                    errors.Add(error);
                }
                
            }
            if (br2 != 0)
            {
                Error error = new Error();
                
                error.Message = "BEGIN_END";
                errors.Add(error);
            }
            return errors;
        }


        public void FormatProgram()
        {
            //add enter symbols after ;, if (), then, else, begin, end, repeat, until
        }

        protected List<Error> CheckLanguageConstructions()
        {
            List<Error> errors = new List<Error>();
            int n = program.Count;
            sbyte i0 = 0;
            sbyte i1 = 0;
            
            bool f0 = true;
            bool f1 = true;
            bool f2 = true;
            byte ind0 = 0;
            byte ind1 = 0;
            //byte ind0 = 0;
            //byte ind1 = 0;
            //byte ind2 = 0;
            for (int i=0; i < n; i++)
            {
                string line = program[i];
                if (f0)
                {
                    if (line.Equals("begin"))
                    {
                        i0++;
                    }
                    if (line.Equals("end"))
                    {
                        i0--;
                    }
                    if (i0<0)
                    {
                        Error error = new Error();
                        error.Message = "find uncorect operator \"begin/end\", line: " + i;
                        error.Col = i;
                        errors.Add(error);
                        f0 = false;
                    }
                }
                if (f1)
                {
                    if (line.Equals("repeat"))
                    {
                        i1++;
                    }
                    if (line.StartsWith("until") )
                    {
                        if (line.Contains("(") && line.Contains(")"))
                        {
                            i1--;
                        }
                        else
                        {
                            f1 = false;
                        }
                       
                    }
                    if (!f1 || (i1<0))
                    {
                        Error error = new Error();
                        error.Message = "find uncorect operator \"repeat/until\", line: " + i;
                        error.Col = i;
                        errors.Add(error);
                        f0 = false;
                    }
                } 
                if (f2)
                {
                    if (line.StartsWith("if") )
                    {
                        ind0 = 0;
                        ind1 = 0;
                        if (line.Contains('(') && line.Contains(')'))
                        {
                            for (int j=i + 1; j<program.Count; j++)
                            {
                                if (program[j].StartsWith("if"))
                                {
                                    break;
                                }
                                if (program[j].Equals("then"))
                                {
                                    ind0++;
                                }
                                if (program[j].Equals("esle"))
                                {
                                    ind1++;
                                }                                
                            }
                            if (!(ind0 == 1 && (ind1 == 0 || ind1 == 1)))
                            {
                                f2 = false;
                            }

                        }
                        else
                        {
                            f2 = false;
                        }
                        if (!f2)
                        {
                            Error error = new Error();
                            error.Message = "find uncorrect construction of condition operator, line: " + i;
                            error.Col = i;
                            errors.Add(error);
                        }
                    }
                   
                }
               
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

        protected bool CheckFirstSymbolInIdentificator(char c)
        {
            bool f = !Regex.IsMatch(c.ToString(), @"^[0-9]*$");
            if (f)
            {
                f = c.Equals('_') || Regex.IsMatch(c.ToString(), @"^[a-zA-Z]+$");
            }
            if (f)
            {
                f = !c.Equals("&") && !c.Equals("$") && !c.Equals("%");
            }
            return f;
        }
        protected bool CheckLastSymbolInIdentificator(char c)
        {
            bool f = Regex.IsMatch(c.ToString(), @"^[a-zA-Z0-9]*$") || c.Equals('_');
            return f;
        }

        protected bool IsCorrectExpression(string value)
        {
            bool flag = true;
            string[] delimiters = new string[] { "not", "and", "^", "/", "*", "div", "mod", "and", "+", "-", "or", "xor", "=", "<>", "<", ">", "<=", ">=", "in", "(", ")" };
            List<Element> indexes = new List<Element>();

            sbyte bi = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i].Equals('('))
                {
                    bi++;
                }
                if (value[i].Equals(')'))
                {
                    bi--;
                }
                if (bi < 0)
                {
                    break;
                }
            }
            flag = bi == 0;
            if (flag)
            {
                foreach (string del in delimiters)
                {
                    List<int> temp = new List<int>();
                    if (del.Equals("<") || del.Equals(">") || del.Equals("=") || del.Equals("or") || del.Equals("in") || del.Equals("not") || del.Equals("(") || del.Equals(")"))
                    {
                        temp = GetAllContains(value, del);
                        foreach (int index in temp)
                        {
                            bool f1 = true;
                            int si = 0; int ei = 0;
                            switch (del)
                            {
                                case "<":
                                    if (index - 1 >= 0)
                                    {
                                        si = index - 1;
                                        if (value[si].Equals(' '))
                                        {
                                            while (value[si].Equals(' '))
                                            {
                                                si--;
                                            }
                                        }
                                        f1 = IsValue(value[si]) || value[si].Equals(')');
                                    }
                                    else
                                    {
                                        f1 = false;
                                    }
                                    if (f1)
                                    {
                                        if (index + 1 < value.Length)
                                        {
                                            ei = index + 1;
                                            if (value[ei].Equals(' '))
                                            {
                                                while (value[ei].Equals(' '))
                                                {
                                                    ei++;
                                                }
                                            }
                                            f1 = IsValue(value[ei]) || value[ei].Equals('>') || value[ei].Equals('=') || value[ei].Equals('(');
                                        }
                                        else
                                        {
                                            f1 = false;
                                        }
                                    }
                                    if (f1)
                                    {
                                        AddIndex(new Element(index, del));
                                    }
                                    break;
                                case ">":
                                    if (index - 1 >= 0)
                                    {
                                        si = index - 1;
                                        if (value[si].Equals(' '))
                                        {
                                            while (value[si].Equals(' '))
                                            {
                                                si--;
                                            }
                                        }
                                        f1 = IsValue(value[si]) || value[si].Equals(')');

                                    }
                                    else
                                    {
                                        f1 = false;
                                    }
                                    if (f1)
                                    {
                                        if (index + 1 < value.Length)
                                        {
                                            ei = index + 1;
                                            if (value[ei].Equals(' '))
                                            {
                                                while (value[ei].Equals(' '))
                                                {
                                                    ei++;
                                                }
                                            }
                                            f1 = IsValue(value[ei]) || value[ei].Equals('=') || value[ei].Equals('(');
                                        }
                                        else
                                        {
                                            f1 = false;
                                        }
                                    }
                                    if (f1)
                                    {
                                        AddIndex(new Element(index, del));
                                    }
                                    break;
                                case "=":
                                    if (index - 1 >= 0)
                                    {
                                        si = index - 1;
                                        if (value[si].Equals(' '))
                                        {
                                            while (value[si].Equals(' '))
                                            {
                                                si--;
                                            }
                                        }
                                        f1 = IsValue(value[si]) || value[si].Equals('<') || value[si].Equals('>') || value[si].Equals(')');
                                    }
                                    else
                                    {
                                        f1 = false;
                                    }
                                    if (f1)
                                    {
                                        if (index + 1 < value.Length)
                                        {
                                            ei = index + 1;
                                            if (value[ei].Equals(' '))
                                            {
                                                while (value[ei].Equals(' '))
                                                {
                                                    ei++;
                                                }
                                            }
                                            f1 = IsValue(value[ei]) || value[ei].Equals('(');
                                        }
                                        else
                                        {
                                            f1 = false;
                                        }
                                    }
                                    if (f1)
                                    {
                                        AddIndex(new Element(index, del));
                                    }
                                    break;
                                case "or":
                                    if (index - 1 >= 0)
                                    {
                                        si = index - 1;
                                        if (value[si].Equals(' '))
                                        {
                                            while (value[si].Equals(' '))
                                            {
                                                si--;
                                            }
                                        }
                                        f1 = IsValue(value[si]) || value[si].Equals(')');
                                    }
                                    else
                                    {
                                        f1 = false;
                                    }
                                    if (f1)
                                    {
                                        if (index + 2 < value.Length)
                                        {
                                            ei = index + 2;
                                            if (value[ei].Equals(' '))
                                            {
                                                while (value[ei].Equals(' '))
                                                {
                                                    ei++;
                                                }
                                                f1 = IsValue(value[ei]) || value[ei].Equals('(');
                                            }
                                        }
                                        else
                                        {
                                            f1 = false;
                                        }
                                    }
                                    if (f1)
                                    {
                                        AddIndex(new Element(index, del));
                                    }
                                    break;
                                case "in":
                                    if (index - 1 >= 0)
                                    {

                                        List<int> data = GetAllContains(value, "begin");
                                        bool f2 = true;
                                        foreach (int ind in data)
                                        {
                                            f2 = index < ind && ind > ind + 6;
                                            if (!f2)
                                                break;
                                        }
                                        if (f2)
                                        {
                                            si = index - 1;
                                            if (value[si].Equals(' '))
                                            {
                                                while (value[si].Equals(' '))
                                                {
                                                    si--;
                                                }
                                            }
                                            f1 = IsValue(value[si]) || value[si].Equals(')');
                                            if (f1)
                                            {
                                                if (index + 3 < value.Length)
                                                {
                                                    ei = index + 3;
                                                    if (value[ei].Equals(' '))
                                                    {
                                                        while (value[ei].Equals(' '))
                                                        {
                                                            ei++;
                                                        }
                                                    }
                                                    f1 = IsValue(value[ei]) || value[ei].Equals('(');
                                                }
                                                else
                                                {
                                                    f1 = false;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            f1 = false;
                                        }
                                    }
                                    else
                                    {
                                        f1 = false;
                                    }
                                    if (f1)
                                    {
                                        AddIndex(new Element(index, del));
                                    }
                                    break;
                                case "not":
                                    if (index != 0)
                                    {
                                        if (index - 1 >= 0)
                                        {
                                            si = index - 1;
                                            if (value[si].Equals(' '))
                                            {
                                                while (value[si].Equals(' '))
                                                {
                                                    si--;
                                                }
                                            }
                                            f1 = IsAllowSymbolBeforeNot(value[si]);
                                        }
                                    }
                                    if (f1)
                                    {
                                        if (index + 4 < value.Length)
                                        {
                                            ei = index + 4;
                                            if (value[ei].Equals(' '))
                                            {
                                                while (value[ei].Equals(' '))
                                                {
                                                    ei++;
                                                }
                                            }
                                            f1 = IsValue(value[ei]);
                                        }
                                        else
                                        {
                                            f1 = false;
                                        }
                                    }
                                    if (f1)
                                    {
                                        AddIndex(new Element(index, del));
                                    }
                                    break;
                                case "(":
                                    if (index != 0)
                                    {
                                        if (index - 1 >= 0)
                                        {
                                            si = index - 1;
                                            if (value[si].Equals(' '))
                                            {
                                                while (value[si].Equals(' '))
                                                {
                                                    si--;
                                                }
                                            }
                                            f1 = IsOperator(value[si]);

                                        }
                                    }
                                    if (f1)
                                    {
                                        if (index + 1 < value.Length)
                                        {
                                            ei = index + 1;
                                            if (value[ei].Equals(' '))
                                            {
                                                while (value[ei].Equals(' '))
                                                {
                                                    ei++;
                                                }
                                            }
                                            f1 = IsValue(value[ei]);
                                        }

                                        else
                                        {
                                            f1 = false;
                                        }
                                    }
                                    if (f1)
                                    {
                                        AddIndex(new Element(index, del));
                                    }
                                    break;
                                case ")":
                                    if (index != value.Length - 1)
                                    {
                                        if (index - 1 >= 0)
                                        {
                                            si = index - 1;
                                            if (value[si].Equals(' '))
                                            {
                                                while (value[si].Equals(' '))
                                                {
                                                    si--;
                                                }
                                            }
                                            f1 = IsValue(value[si]);
                                        }
                                        else
                                        {
                                            f1 = false;
                                        }
                                    }
                                    if (f1)
                                    {
                                        if (index + 1 < value.Length)
                                        {
                                            ei = index + 1;
                                            if (value[ei].Equals(' '))
                                            {
                                                while (value[ei].Equals(' '))
                                                {
                                                    ei++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            f1 = false;
                                        }
                                    }
                                    if (f1)
                                    {
                                        AddIndex(new Element(index, del));
                                    }
                                    break;
                            }
                        }
                    }
                    if (!del.Equals("<") && !del.Equals(">") && !del.Equals("=") && !del.Equals("or") && !del.Equals("in") && !del.Equals("not") && !del.Equals("(") && !del.Equals(")"))
                    {
                        temp = GetAllContains(value, del);

                        foreach (int index in temp)
                        {
                            bool f1 = true;
                            int si = 0; int ei = 0;
                            if (index - 1 >= 0)
                            {
                                si = index - 1;
                                if (value[si].Equals(' '))
                                {
                                    while (value[si].Equals(' '))
                                    {
                                        si--;
                                    }
                                }
                                f1 = IsValue(value[si]) || value[si].Equals(')');
                            }
                            if (f1)
                            {
                                if (index + del.Length < value.Length)
                                {
                                    ei = index + del.Length;
                                    if (value[ei].Equals(' '))
                                    {
                                        while (value[ei].Equals(' '))
                                        {
                                            ei++;
                                        }
                                    }
                                    f1 = IsValue(value[ei]) || value[ei].Equals('(');
                                }
                                else
                                {
                                    f1 = false;
                                }
                            }
                            if (f1)
                            {

                                AddIndex(new Element(index, del));
                            }
                        }
                    }
                }


                string variable = "";
                if (indexes.Count == 0)
                {
                    flag = IsCorrectIdentificator(value) || IsNumber(value);
                }
                else
                {
                    int ind = 0;
                    while (ind < value.Length)
                    {
                        if (value[ind].Equals(' '))
                        {
                            ind++;
                        }
                        else
                        {
                            Element el = IsDelimiter(ind);
                            if (el != null)
                            {
                                if (!string.IsNullOrEmpty(variable))
                                {
                                    flag = IsCorrectIdentificator(variable) || IsNumber(variable);
                                    variable = "";
                                    if (!flag)
                                        break;
                                }
                                ind += el.Name.Length;
                            }
                            else
                            {
                                variable += value[ind];
                                ind++;
                            }
                        }


                    }

                }

            }

            Element IsDelimiter(int p)
            {
                Element el = null;
                foreach (Element e in indexes)
                {
                    if (e.StartIndex == p)
                    {
                        el = e;
                        break;
                    }

                }
                return el;
            }

            void AddIndex(Element el)
            {
                if (el != null)
                {
                    if (indexes.Count > 0)
                    {
                        bool f = true;
                        foreach (Element element in indexes)
                        {
                            if ((element.Name.Equals(el.Name)) && (el.StartIndex == element.StartIndex))
                            {
                                f = false;
                                break;
                            }
                        }
                        if (f)
                        {
                            indexes.Add(el);
                        }
                    }
                    else
                    {
                        indexes.Add(el);
                    }
                }

            }
            return flag;
        }


        private bool IsOperator(char c)
        {
            return c.Equals('=') || c.Equals('^') || c.Equals('*') || c.Equals('/') || c.Equals('+') || c.Equals('-') || c.Equals('<') || c.Equals('>') || c.Equals(';');
        }

        protected bool IsAllowSymbolBeforeNot(char c)
        {
            return c.Equals('=') || c.Equals('^') || c.Equals('*') || c.Equals('/') || c.Equals('+') || c.Equals('-') || c.Equals('<') || c.Equals('>') || c.Equals('(') || c.Equals(' ');
        }

        private bool IsNumber(string value)
        {
            bool f = true;
            try
            {
                double.Parse(value);
            }
            catch (Exception)
            {
                f = false;
            }

            return f;
        }

        protected bool IsCorrectValue(string val)
        {
            bool f = true;
            f =   Regex.IsMatch(val, "^[0-9]+$") || Regex.IsMatch(val, "^[a-zA-Z]+$") || val.Contains('.') || val.Contains('_');
           
            return f;
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

        public void RepairProgram()
        {
            int n = GetProgramSize();
            for (int i=0; i<n;i++)
            {
                string line = RepairProgramLine(i);
                if (!string.IsNullOrEmpty(line))
                {
                    program.Add(line);
                }               
            }
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