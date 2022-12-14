using NUnit.Framework.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using static CourseProject.BinaryTree;

namespace CourseProject
{
    public class SyntaxisAnalyzer : Service
    {

        private List<string> program = new();

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
                    }
                    index++;
                    line = RepairProgramLine(index);
                }
                tree.SetHead();

                

                for (int i=index; i<n; i++)
                {
                    line = RepairProgramLine(i);
                    if (line.StartsWith("begin"))
                    {
                        tree.AddRightChild("begin");
                    }
                    if (line.StartsWith("end"))
                    {
                        tree.AddRightChild(line);
                    }
                    if (line.StartsWith("if"))
                    {
                        string condition = GetSubsString(line, line.IndexOf('('), line.LastIndexOf(')'));
                        BinaryTree condTree = ParseExpression(condition);
                        tree.AddRightChild("if");
                        tree.SetLeftChild(condTree.Root);
                    }
                    if (line.StartsWith("then"))
                    {
                        tree.AddRightChild("then");
                    }
                    if (line.StartsWith("else"))
                    {
                        tree.AddRightChild("else");
                    }
                    if (line.StartsWith("repeat"))
                    {
                        tree.AddRightChild("repeat");
                    }
                    if (line.StartsWith("until"))
                    {
                        string condition = GetSubsString(line, line.IndexOf('('), line.LastIndexOf(')'));
                        BinaryTree condTree = ParseExpression(condition);
                        tree.AddRightChild("until");
                        tree.SetLeftChild(condTree.Root);
                    }
                    if (line.Contains(":="))
                    {
                        string arg0 = line.Split(":=")[0];
                        string arg1 = line.Split(":=")[1];
                        BinaryTree exprTree = ParseExpression(arg1);
                        tree.AddRightChild(":=");
                        tree.AddLeftChild(arg0);
                        tree.SetParent(tree.Root);
                        tree.SetRightChild(exprTree.Root);
                    }
                    if (line.StartsWith("write"))
                    {
                        string arg0 = "write";
                        if (line.StartsWith("writeln"))
                        {
                            arg0 = "writeln";
                        }
                        string arg1 = GetSubsString(line, line.IndexOf('('), line.LastIndexOf(')'));
                        BinaryTree outputTree = ParseExpression(arg1);
                        tree.AddRightChild(arg0);
                        tree.SetLeftChild(outputTree.Root);
                    }
                    if (line.StartsWith("read"))
                    {
                        string arg0 = "read";
                        if (line.StartsWith("readln"))
                        {
                            arg0 = "readln";
                        }
                        string[] inputArgs = GetSubsString(line, line.IndexOf('(') + 1, line.LastIndexOf(')') - 1).Split(',');
                        tree.AddRightChild(arg0);
                        for (int j=0; j<inputArgs.Length; j++)
                        {
                            tree.AddLeftChild(inputArgs[j]);
                        }
                        for (int j = 0; j < inputArgs.Length; j++)
                        {
                            tree.SetParent(tree.Root);
                        }
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
                    //add brackets
                    line = AddBrackets(line);
                    int ind = 0;

                    //number - [0-9,.]
                    //value - a-zA-Z0-9_
                    //and, or, xor, not, <, >, =, <>, <=, >=, +, -, *, /

                    
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

        string[] array = new string[] { "not#", "^", "*", "/", "#div#", "#mod#", "#and#", "+", "-", "#or#", "#xor#", "=", "<>", "<", ">", "<=", ">=", "#in#" };

        char[] patterns = new char[] { '^', '*', '/', '+', '-', '(', ')', '<', '>', '=', '#' };

        private string AddBrackets(string line)
        {
          

            //replase 

            line = ChangeSymbols(line);

            List<string> lines = new List<string>();

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
                            //else
                            //{
                            //    f1 = false;
                            //}

                            //indexes = FormIndexes();
                            //ind = indexes[i].Position;
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
                            //else
                            //{
                            //    f2 = false;
                            //}
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
                if (program[i].Equals("begin"))
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
                if (line.StartsWith(":="))
                {
                    string arg0 = line.Split(":=")[0];
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
                        if (arg1[j].Equals('<'))
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
                        if (arg1[j].Equals('>'))
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
                        if (!arg1[j].Equals('=') && !arg1[j].Equals('<') && !arg1[j].Equals('>'))
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
                    Console.WriteLine("find unknown identificator, location: col {0}, row {1}", error.Col, error.Row);
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
                            Console.WriteLine("please check count of pair symbols \"begin\" and \"end\", line is {0}", error.Col);
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

        private List<Error> CheckDelimiters()
        {
            List<Error> errors = new List<Error>();
            int n = GetProgramSize();
            for (int i=0; i<n; i++)
            {
                string line = RepairProgramLine(i);
                //check 
                if (!(line.Contains("begin") || line.Equals("if") || line.Equals("else") || line.Equals("until") ))
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

        private List<Error> CheckCorrectAssign()
        {
            List<Error> errors = new List<Error>();
            for (int i=0; i<program.Count; i++)
            {
                string line = program[i];
                if (line.Contains(":="))
                {
                    string arg0 = line.Split(":=")[0];                   
                    if (string.IsNullOrEmpty(arg0))
                    {
                        Error error = new();
                        error.Col = i;
                    }
                }
            }
            //:=5; must be exists operator before :=
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

        private List<Error> CheckCorrectDeclaredOfOperators()
        {
            string[] operators = new string[] { "not", "and", "^", "/", "div", "mod", "and", "+", "-", "or", "xor", "=", "<>", "<", ">", "<=", ">=", "in" };
            //check correct location for all supported operators
         
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
                foreach (string op in operators)
                {
                    List<int> indexes = GetAllContains(line, op);
                    bool flag = true;
                    if (op.Equals("not"))
                    {
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
                                if (!flag)
                                {
                                    Error error = new Error();
                                    error.Message = "find uncorrect operator \"not\", line: " + i;
                                    error.Col = i;
                                    errors.Add(error);
                                }
                            }
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
                                            flag = line[index + op.Length].Equals(' ') || CheckFirstSymbolInIdentificator(line[index + op.Length]);
                                        }
                                        else
                                        {
                                            flag = false;
                                        }
                                    }
                                    if (!flag)
                                    {
                                        Error error = new Error();
                                        error.Message = "find uncorect operator \"" + op + "\", line: " + i;
                                        error.Col = i;
                                        errors.Add(error);
                                    }
                                }

                            }
                        }
                    }

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

        private List<Error> CheckPairSymbols()
        {

            //[ and ], ( and ), begin/end;
            List<Error> errors = new List<Error>();
            sbyte br0, br1, br2;
            for (int i=0; i<program.Count; i++)
            {
                string line = program[i];
                br0 = 0;
                br1 = 0;
                br2 = 0;
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
                        if (br0 < 0)
                        {
                            Error error = new Error();
                            error.Col = i;
                            error.Message = "RECTANGLE_BRACKET";
                            errors.Add(error);
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
                        if (br1 < 0)
                        {
                            Error error = new Error();
                            error.Col = i;
                            error.Message = "CIRCLE_BRACKET";
                            errors.Add(error);
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
                if (br2<0)
                {
                    Error error = new Error();
                    error.Col = i;
                    error.Message = "BEGIN_END";
                    errors.Add(error);
                }
            }
            return errors;
        }


        public void FormatProgram()
        {
            //add enter symbols after ;, if (), then, else, begin, end, repeat, until
        }

        private List<Error> CheckLanguageConstructions()
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
            byte ind2 = 0;
            //if-then-else; repeat-until (cond)
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
                    if (line.StartsWith("until") && line.Contains("(") && line.Contains(")"))
                    {
                        i1--;
                    }
                    if (i1<0)
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
                    if (line.StartsWith("if") && line.Contains('(') && line.Contains(')'))
                    {
                        ind0++;
                    }
                    if (line.Equals("then"))
                    {
                        if (ind1 == ind0 - 1)
                        {
                            ind1++;
                        }
                        else
                        {
                            f2 = false;
                        }
                    }
                    if (line.Equals("else"))
                    {
                        if ((ind2 == ind0-1) && (ind2 == ind1-1))
                        {
                            ind2++;
                        }
                        else
                        {
                            f2 = false;
                        }
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
          
            //var 2 (if):
            //идем сначала; если встретили if (condition)
            //  если следующая строка eq then
            //    если следующая строка eq begin
            //      ind+1;
            //    идем вниз
            //    если строка eq end
            //      ind-1;
            //     если ind<0
            //       error count of begin end; break;
            //     если ind ==0
            //     если след строка = else
            //       если след строка = begin
            //        ind+1;
            //       если след строка = end
            //        ind -1;
            //       если ind<0
            //        throw error
            //         если ind=0
            //         конструкция корректна
            //--------------------
            //для if: скнанируем с конца:
            //если line eq 'else'
            //идем на 1 вниз - проверяем наличия оператора или begin
            //идем на 1 вверх - проверяем наличие оператора или end, if end =>f67 = true
            //идем вверх, пока не встретим then
            //если встретили then; if (f67) =>next line must be eq begin else next line must be exists
            //; то на 1 вверх - проверяем, соответствует ли строка формату if (condition)
            //для repeat
            //идем сначала 
            //if line is eq repeat fl = fl+1;
            //if line is line until (condition) -> fl = fl-1;
            //if fl<0 -> throw error
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

        private bool CheckFirstSymbolInIdentificator(char c)
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
        private bool CheckLastSymbolInIdentificator(char c)
        {
            bool f = Regex.IsMatch(c.ToString(), @"^[a-zA-Z0-9]*$") || c.Equals('_');
            return f;
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

        public void RepairProgram()
        {
            int n = GetProgramSize();
            for (int i=0; i<n;i++)
            {
                string line = RepairProgramLine(i);
                program.Add(line);
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

        private string RemoveLeftSpaces(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                int si = 0;
                for (int k = 0; k < line.Length; k++)
                {
                    if (line[k].Equals(' '))
                    {
                        si++;
                    }
                    else
                    {
                        break;
                    }
                }
                line = line.Substring(si);
            }
            return line;
        }

        private string RemoveRigthSpaces(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                int index = line.Length - 1;
                while (index >= 0)
                {
                    if (line[index].Equals(' '))
                    {

                    }
                    else
                    {
                        break;
                    }
                    index--;
                }


                line = line.Remove(index);
            }
            return line;
        }

        //public List<string> ReadProgramFromFile(string path)
        //{
        //    List<string> program = new List<string>();
        //    try{
        //        using (StreamReader reader = new StreamReader(path))
        //        {
        //            string? line;
        //            while (!string.IsNullOrEmpty(line=reader.ReadLine()))
        //            {
        //                program.Add(line);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.StackTrace);
        //    }
        //    return program;
        //}
    }
}