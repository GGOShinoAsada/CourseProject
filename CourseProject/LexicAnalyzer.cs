using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CourseProject
{
    public class LexicAnalyzer
    {

        private struct Item
        {
            public string Name { get; set; }
            public int Col { get; set; }
            public int Row { get; set; }
        }


        private List<string> program = new List<string>();

        private List<string> Identificatorslist = new List<string>();

        private List<string> TokensList = new List<string>();

        const string IdentificatorsPath = "D:\\COMPILER\\data\\identificators.txt";

        const string TokensPath = "D:\\COMPILER\\data\\tokens.txt";

        const string PatternsPath = "D:\\COMPILER\\data\\patterns.txt";

        public void Test()
        {
            string[] data = { "abc$", "aBc1434", "DEf45_", "_fdg45", "14gfg", "12574" };
            foreach (string r in data)
            {
                if (Regex.IsMatch(r, @"^[0-9]*$"))
                {
                    Console.WriteLine("item {0} is correct", r);
                }
            }
        }

        public void ExecuteNumber(String line)
        {
            Regex r = new Regex(@"^[0-9]*$");
            string numbStr = "";
            if (line != null)
            {
                if (r.IsMatch(line))
                {
                    int si = line.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' }, 0, line.Length);
                    for (int i = si; i < line.Length; i++)
                    {
                        if (line[i] == ';')
                            break;
                        numbStr += line[i];
                    }
                }
            }
        }

        public void FormIdentificatorsTest()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            List<Item> items = new List<Item>();
            try
            {
                for (int i = 0; i < program.Count; i++)
                {

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void FormIndentificators()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            List<Item> items = new List<Item>();
            bool IsNamesCorrect = true;
            int programBody = 0;
            try
            {
                for (int i = 0; i < program.Count; i++)
                {
                    if (program[i].Equals("begin"))
                    {
                        programBody = i + 1;
                        break;
                    }
                    if (program[i].Contains("var"))
                    {
                        string tmp = program[i];
                        int col = i;
                        List<string> IdentificatorsList = tmp.Remove(tmp.IndexOf("var", StringComparison.OrdinalIgnoreCase), 4).Split(':')[0].Trim().Split(',').ToList();
                        for (int j = 0; j < IdentificatorsList.Count; j++)
                        {
                            IdentificatorsList[j] = IdentificatorsList[j].Trim();
                        }
                        IsNamesCorrect = true;

                        //исключение повторений 
                        string arg = IdentificatorsList[0];
                        for (int j = 1; j < IdentificatorsList.Count; j++)
                        {
                            if (arg.Contains(IdentificatorsList[j]))
                            {

                                IsNamesCorrect = false;
                            }
                        }
                        //проверка присвоения переменной разных типов (не реализовать)

                        //проверка допустимыx символов A-za-z0-9_
                        foreach (string identificator in Identificatorslist)
                        {
                            if (!Regex.IsMatch(identificator[0].ToString(), @"^[0-9]*$"))
                            {
                                IsNamesCorrect = Regex.IsMatch(identificator, @"^[a-zA-Z0-9_]*$");
                            }

                        }

                        if (IsNamesCorrect)
                        {
                            List<int> rows = new List<int>();
                            int t1 = tmp.IndexOf(':');
                            List<int> delimiters = new List<int>(0);
                            for (int j = tmp.IndexOf(' ') + 1; j < tmp.Length; j++)
                            {
                                if (tmp[j].Equals(':') || tmp[j].Equals(','))
                                {
                                    delimiters.Add(j);
                                }
                            }
                            foreach (int item in delimiters)
                            {
                                for (int k = item - 1; k > 0; k--)
                                {
                                    if (tmp[k].Equals(' ') || tmp[k].Equals(','))
                                    {
                                        rows.Add(k + 1);
                                        break;
                                    }
                                }
                            }


                            for (int j = 0; j < IdentificatorsList.Count; j++)
                            {
                                Item item1 = new Item();
                                item1.Col = i;
                                item1.Row = rows[j];
                                item1.Name = IdentificatorsList[j].Trim();
                                items.Add(item1);
                            }
                        }
                        else
                        {
                            Console.WriteLine("find uncorrect identificator, col {0}", col);
                        }
                    }
                    else
                    {
                        string tmp = program[i];
                        int col = i;
                        List<string> identificatorsList = tmp.Split(':')[0].Split(',').ToList();

                        IsNamesCorrect = true;
                        for (int j = 0; j < identificatorsList.Count; j++)
                        {
                            string identificator = identificatorsList[j].Trim();
                            if (!Regex.IsMatch(identificator[0].ToString(), @"^[0-9]*$"))
                            {
                                IsNamesCorrect = Regex.IsMatch(identificator, @"^[a-zA-Z0-9_]*$");
                            }
                        }
                        if (IsNamesCorrect)
                        {
                            List<int> rows = new List<int>();
                            List<int> delimiters = new List<int>();
                            for (int j = 0; j < tmp.Length; j++)
                            {
                                if (tmp[j].Equals(',') || tmp[j].Equals(':'))
                                {
                                    delimiters.Add(j);
                                }
                            }
                            foreach (int dp in delimiters)
                            {
                                int r = 0;
                                for (int j = dp; j > 0; j--)
                                {
                                    if (tmp[j].Equals(' ') || tmp.Equals(','))
                                    {
                                        r = j;
                                        break;
                                    }
                                }
                                rows.Add(r);
                            }
                            for (int j = 0; j < rows.Count; j++)
                            {
                                Item item = new Item();
                                item.Name = identificatorsList[j].Trim();
                                item.Col = col;
                                item.Row = rows[j];
                                items.Add(item);
                            }
                        }
                        else
                        {
                            Console.WriteLine("find uncorrect identificator, col {0}", col);
                        }
                    }
                }
                List<Item> items1 = new List<Item>();
                for (int i = programBody; i < program.Count; i++)
                {
                    foreach (Item item in items)
                    {
                        if (program[i].Contains(item.Name))
                        {
                            int row = program[i].IndexOf(item.Name);
                            Item tmp = new Item();
                            tmp.Name = item.Name;
                            tmp.Row = row;
                            tmp.Col = i;
                            items1.Add(tmp);
                        }
                    }
                }
                items1.ForEach(item => items.Add(item));


                //numbers




                for (int i = 0; i < program.Count; i++)
                {
                    string line = "";
                    if (Regex.IsMatch(line, @"^[0-9]*$"))
                    {
                        int si = program[i].IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                        string[] ss = new string[] { "]", ")", "..", "+", "-", "/", "*", ".", " " };
                        if (si > 0)
                        {
                            for (int j = si; i < program[i].Length; j++)
                            {
                                line += program[i][j];
                                if (Regex.IsMatch(program[i][j].ToString(), @"^[],),..,+,'-','/',*,., ,;]*$"))
                                {
                                    if (j + 1 < program[i].Length)
                                    {
                                        break;
                                    }
                                }
                            }
                            Item item = new Item();
                            item.Name = line;
                            item.Row = si;
                            item.Col = i;
                            items.Add(item);
                        }
                    }
                    if (IsHexDigitValue(program[i]))
                    {
                        int si = program[i].IndexOf('$');
                        if (si > 0)
                        {
                            line += '$';
                            for (int j = 0; i < program[i].Length; j++)
                            {
                                if (Regex.IsMatch(program[i][j].ToString(), @"^[0-9]*$"))
                                {
                                    line += program[i][j];
                                }
                                else
                                {
                                    break;
                                }
                            }
                            Item item = new Item();
                            item.Name = line;
                            item.Col = i;
                            item.Row = si;
                            items.Add(item);
                        }

                    }
                }

                using (StreamWriter writer = new StreamWriter(IdentificatorsPath))
                {
                    foreach (Item item in items)
                    {
                        writer.WriteLine(item.Name + " " + item.Col + " " + item.Row);
                    }
                }
                //foreach (Item item in items)
                //{
                //    Console.WriteLine("name {0}, col {1}, row {2}", item.Name, item.Col, item.Row);                        
                //}



            }
            catch (DirectoryNotFoundException ex0)
            {
                Console.WriteLine(ex0.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }


        private bool IsHexDigitValue(string line)
        {
            bool flag = true;

            if (line != null)
            {
                flag = line[0].Equals('$');
                line = line.Remove(0, 1);
                flag = Regex.IsMatch(line, @"^[a-fA-F0-9]*$");
            }

            return flag;
        }


        string[] baseTypes = { "integer", "shortint", "smallint", "longint", "int64", "byte", "word", "longword", "cardinal", "uint64", "BigInteger", "real", "double", "single", "decimal", "boolean", "string" };

        string[] compareOperators = { ">", "<", ">=", "<=", "=", "<>" };

        string[] arifmeticOperators = { "+", "-", "/", "*" };

        string[] conditionStructure = { "if", "then", "else" };

        string[] arayStructure = { "array", "of" };

        string[] repeatStructure = { "repeat", "until" };

        string[] ioStructures = { "read", "readln", "write", "writeln" };

        string[] serviceSymbols = { "[", "]", "{", "}", "(", ")", "..", ".", ",", ";", ":=", ":" };




        public void FormTokens()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            List<Item> tokens = new List<Item>();
            try
            {

                for (int i = 0; i < program.Count; i++)
                {
                    string line = program[i];
                    if (line.Contains("var"))
                    {
                        Item item = new Item();
                        item.Name = "var";
                        item.Col = i;
                        item.Row = line.IndexOf("var");
                        tokens.Add(item);
                    }
                    if (IsContainsSymbols(line, baseTypes) > 0)
                    {
                        List<string> names = GetContainsSymbolName(line, baseTypes);
                        foreach (string n in names)
                        {
                            Item item = new Item();
                            item.Name = n;
                            item.Col = i;
                            item.Row = line.IndexOf(n);
                            tokens.Add(item);
                        }

                    }
                    if (IsContainsSymbols(line, arifmeticOperators) > 0)
                    {
                        List<string> names = GetContainsSymbolName(line, arifmeticOperators);
                        foreach (string n in names)
                        {
                            Item item = new Item();
                            item.Name = n;
                            item.Col = i;
                            item.Row = line.IndexOf(n);
                            tokens.Add(item);
                        }

                    }
                    if (IsContainsSymbols(line, compareOperators) > 0)
                    {
                        List<string> names = GetContainsSymbolName(line, compareOperators);
                        foreach (string n in names)
                        {
                            Item item = new Item();
                            item.Name = n;
                            item.Col = i;
                            item.Row = line.IndexOf(n);
                            tokens.Add(item);
                        }

                    }
                    if (IsContainsSymbols(line, conditionStructure) > 0)
                    {
                        List<string> names = GetContainsSymbolName(line, conditionStructure);
                        foreach (string n in names)
                        {
                            Item item = new Item();
                            item.Name = n;
                            item.Col = i;
                            item.Row = line.IndexOf(n);
                            tokens.Add(item);
                        }

                    }
                    if (IsContainsSymbols(line, repeatStructure) > 0)
                    {
                        List<string> names = GetContainsSymbolName(line, repeatStructure);
                        foreach (string n in names)
                        {
                            Item item = new Item();
                            item.Name = n;
                            item.Col = i;
                            item.Row = line.IndexOf(n);
                            tokens.Add(item);
                        }

                    }
                    if (line.Contains("array") && line.Contains("of"))
                    {

                        Item item = new Item();
                        item.Name = "array";
                        item.Col = i;
                        item.Row = line.IndexOf("array");
                        tokens.Add(item);
                        item = new Item();
                        item.Name = "of";
                        item.Col = i;
                        item.Row = line.IndexOf("of");
                        tokens.Add(item);
                    }
                    if (IsContainsSymbols(line, ioStructures) > 0)
                    {
                        string pattern = "";
                        if (program[i].Contains("write"))
                        {
                            string arg = program[i].Substring(0, program[i].IndexOf("("));
                            arg = arg.Trim();
                            if (arg.Equals("write"))
                            {
                                pattern = "write";
                            }
                            if (arg.Equals("writeln", StringComparison.InvariantCultureIgnoreCase))
                            {
                                pattern = "writeln";
                            }
                        }
                        if (program[i].Contains("read"))
                        {
                            string arg = program[i].Substring(0, program[i].IndexOf("("));
                            arg = arg.Trim();
                            if (arg.Equals("read"))
                            {
                                pattern = "read";
                            }
                            if (arg.Equals("readln", StringComparison.InvariantCultureIgnoreCase))
                            {
                                pattern = "readln";
                            }
                        }

                        Item item = new Item();
                        item.Name = pattern;
                        item.Col = i;
                        item.Row = line.IndexOf(pattern);
                        tokens.Add(item);
                    }
                    if (IsContainsSymbols(line, serviceSymbols) > 0)
                    {
                        //
                        if (line.Contains("."))
                        {
                            string name = ".";
                            Item item = new Item();
                            item.Name = name;
                            item.Row = line.IndexOf(".");
                            item.Col = i;
                            tokens.Add(item);
                        }
                        else if (line.Contains(".."))
                        {
                            string name = "..";
                            Item item = new Item();
                            item.Name = name;
                            item.Row = line.IndexOf("..");
                            item.Col = i;
                            tokens.Add(item);
                        }
                        else
                        {
                            List<string> names = GetContainsSymbolName(line, serviceSymbols);
                            foreach (string n in names)
                            {
                                Item item = new Item();
                                item.Name = n;
                                item.Col = i;
                                item.Row = line.IndexOf(n);
                                tokens.Add(item);
                            }

                        }

                    }

                }
                //foreach (Item item1 in tokens)
                //{
                //    Console.WriteLine(item1.Name + " " + item1.Col + " " + item1.Row);
                //}
                using (StreamWriter writer = new StreamWriter(TokensPath))
                {
                    foreach (Item item in tokens)
                    {
                        writer.WriteLine(item.Name + " " + item.Col + " " + item.Row);
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
        }

        int IsContainsSymbols(string line, string[] symbols)
        {
            int pos = -1;
            foreach (string p in symbols)
            {
                pos = line.IndexOf(p);
                if (pos > 0)
                    break;
            }
            return pos;
        }

        List<string> GetContainsSymbolName(string line, string[] paterns)
        {
            List<string> names = new List<string>();
            foreach (string p in paterns)
            {
                if (line.Contains(p))
                {
                    names.Add(p);
                    //break;
                }
            }
            return names;
        }

        private string RemoveSpaces(string input)
        {
            string result = String.Empty;
            foreach (char c in input)
            {
                if (!c.Equals(' '))
                {
                    result += c;
                }
            }
            return result;
        }


        public List<string> ReadProgram(string path)
        {
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

    }
}
