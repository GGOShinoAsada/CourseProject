using System;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace CourseProject
{
    public class LexicAnalyzer : Service
    {
        private class Item
        {
            public string? Name { get; set; }
            public int Col { get; set; }
            public int Row { get; set; }
        }

        private readonly char[] delimiters = new char[] { '^', '/', '*', '+', '-', '=', '<', '>', ' ', '(', ')', '[', ']', ';', ',' };

        private readonly string[] BaseTypes = { "integer", "shortint", "smallint", "longint", "int64", "byte", "word", "longword", "cardinal", "uint64", "BigInteger", "real", "double", "single", "decimal", "boolean", "string", "char" };

        private readonly string[] CompareOperators = { ">", "<", ">=", "<=",/* "=",*/ "<>" };

        private readonly string[] ArifmeticOperators = { "+", "-", "/", "*", "^", "div", "mod", "in" };

        private readonly string[] ConditionStructure = { "if", "then", "else" };

        private readonly string[] LogicalOperators = { "and", "or", "not", "xor" };

        private readonly string[] LogicalValues = { "true", "false" };

        private readonly string[] BorderOfStatements = { "begin", "end" };

        private readonly string[] ArrayStructure = { "array", "of" };

        private readonly string[] RepeatStructure = { "repeat", "until" };

        private readonly string[] IOStructure = { "read", "readln", "write", "writeln" };

        private readonly string[] ServiceSymbols = { "[", "]", "{", "}", "(", ")", "..", ".", ",", ";"/*":=", ":"*/ };

        private List<string> program = new();

       // private List<string> Identificatorslist = new List<string>();

        //private List<string> TokensList = new List<string>();

        //public void Test()
        //{
        //    string[] data = { "abc$", "aBc1434", "DEf45_", "_fdg45", "14gfg", "12574" };
        //    foreach (string r in data)
        //    {
        //        if (Regex.IsMatch(r, @"^[a-zA-Z0-9]*$"))
        //        {
        //            Console.WriteLine("item {0} is correct", r);
        //        }
        //    }
        //}


        public void PrintProgram()
        {
            foreach (string line in program)
            {
                Console.WriteLine(line);
            }
        }

        //public List<string> CheckProgram()
        //{
        //    bool flag = true;
        //    List<string> errors = new();

        //    for (int i = 0; i < program.Count; i++)
        //    {
        //        if (program[i] != "")
        //        {
        //            if (program[i].Equals("begin"))
        //            {            
        //                break;
        //            }
        //            if (program[i].Contains("var"))
        //            {
        //                string tmp = program[i];
        //                int col = i;
        //                List<string> IdentificatorsList = tmp.Remove(tmp.IndexOf("var"), 4).Split(':')[0].Trim().Split(',').ToList();
        //                for (int j = 0; j < IdentificatorsList.Count; j++)
        //                {
        //                    IdentificatorsList[j] = IdentificatorsList[j].Trim();
        //                }
        //                //проверока корректности объявленного типа                        
        //                flag = false;
        //                string basetype = program[i].Split(":")[1];
        //                if (basetype.IndexOf(";") >= 0)
        //                    basetype = basetype.Remove(basetype.IndexOf(";"), 1);
        //                if (basetype.Contains("of"))
        //                {
        //                    basetype = basetype.Split("of")[1];
        //                }
        //                basetype = basetype.Trim();
        //                foreach (string type in BaseTypes)
        //                {
        //                    if (type.Equals(basetype))
        //                    {
        //                        flag = true;
        //                        break;
        //                    }
        //                }
        //                if (!flag)
        //                {
        //                    errors.Add("find uncorrect base type \"" + basetype + "\", col is " + i);
        //                }
        //                //Проверка идентификаторов (наличие повторений и разрешенные символы)
        //                for (int j = 0; j < IdentificatorsList.Count; j++)
        //                {
        //                    if (IsAlreadyDeclared(IdentificatorsList, IdentificatorsList[j]))
        //                    {
        //                        errors.Add("find repeat declare of identificator \"" + IdentificatorsList[j] + "\", line is" + i);
        //                    }
        //                    if (!IsAllowSymbolsInIdentificator(IdentificatorsList[j]))
        //                    {
        //                        errors.Add("identificator \"" + IdentificatorsList[j] + "\" have uncorrect syntax, line is " + i);
        //                    }
        //                }

        //            }
        //        }
        //    }
        //    bool IsAlreadyDeclared(List<string> list, string identificator)
        //    {
        //        bool flag = true;
        //        for (int j = 0; j < list.Count; j++)
        //        {
        //            if (identificator.Contains(list[j]))
        //            {
        //                flag = false;
        //                break;
        //            }
        //        }
        //        return flag;
        //    }

        //    bool IsAllowSymbolsInIdentificator(string identificator)
        //    {
        //        bool flag = true;
        //        if (!string.IsNullOrEmpty(identificator))
        //        {
        //            if (!Regex.IsMatch(identificator[0].ToString(), @"^[0-9]*$"))
        //            {
        //                flag = Regex.IsMatch(identificator, @"^[a-zA-Z0-9_]*$");
        //            }
        //        }
        //        return flag;
        //    }

        //    return errors;
        //}

        public void FormIndentificators()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            List<Item> items = new List<Item>();
            int programBody = 0;
            try
            {
                for (int i = 0; i < program.Count; i++)
                {
                    string temp = program[i];
                    if (program[i] != "")
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
                            List<string> IdentificatorsList = tmp.Remove(tmp.IndexOf("var"), 4).Split(':')[0].Trim().Split(',').ToList();
                            for (int j = 0; j < IdentificatorsList.Count; j++)
                            {
                                IdentificatorsList[j] = IdentificatorsList[j].Trim();
                            }

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
                            string tmp = program[i];
                            int col = i;
                            List<string> identificatorsList = tmp.Split(':')[0].Split(',').ToList();

                            for (int j = 0; j < identificatorsList.Count; j++)
                            {
                                string identificator = identificatorsList[j].Trim();
                            }

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
                    }
                }
                //List<Item> items1 = new List<Item>();

                for (int i = programBody; i < program.Count; i++)
                {
                    string line = program[i];
                    int dx = 0;
                    int si = 0;
                    if (line.StartsWith("read"))
                    {
                        dx = 0;
                        if (line.StartsWith("readln"))
                        {
                            line = line.Remove(0, 6);
                            dx = 6;
                        }
                        else
                        {
                            line = line.Remove(0, 4);
                            dx = 4;
                        }
                        if (line.StartsWith(' '))
                        {
                            while (line[si].Equals(' '))
                            {
                                si++;
                            }
                        }
                        line = line.Remove(0, si);
                        dx += si;
                        List<Item> temp = GetIdentificatorsInExpression(line, i);
                        for (int j = 0; j < temp.Count; j++)
                        {
                            temp[j].Row += dx;
                        }
                        items.AddRange(temp);

                    }
                    //if (line.StartsWith("write"))
                   // {
                        //List<int> indexes = new List<int>();
                        //dx = 0;
                        //if (line.StartsWith("writeln"))
                        //{
                        //    line = line.Remove(0, 7);
                        //    dx = 7;
                        //}
                        //else
                        //{
                        //    line = line.Remove(0, 5);
                        //    dx = 5;
                        //}
                        //si = 0;
                        //if (line.StartsWith(' '))
                        //{
                        //    while (line[si].Equals(' '))
                        //    {
                        //        si++;
                        //    }
                        //}
                        //line = line.Remove(0, si);
                        //dx += si;
                        //List<Item> data = GetIdentificatorsInExpression(line, i);
                        //for (int j = 0; j < data.Count; j++)
                        //{
                        //    data[j].Row += dx - 1;
                        //}
                        //items.AddRange(data);


                  //  }
                    if (line.StartsWith("if"))
                    {
                        dx = 0;
                        si = 0;
                        line = line.Remove(0, 2);
                        dx = 2;
                        if (line.StartsWith(' '))
                        {
                            while (line[si].Equals(' '))
                            {
                                si++;
                            }
                        }
                        dx += si;
                        List<Item> data = GetIdentificatorsInExpression(line, i);
                        for (int j = 0; j < data.Count; j++)
                        {
                            data[j].Row += dx-1;
                        }
                        items.AddRange(data);
                    }
                    if (line.Contains(":="))
                    {
                        string arg0 = line.Split(":=")[0];
                        dx = 0;
                        si = 0;
                        if (arg0.StartsWith(' '))
                        {
                            while (arg0[si].Equals(' '))
                            {
                                si++;
                            }
                        }
                        dx = si;
                        List<Item> data = GetIdentificatorsInExpression(arg0, i);
                        for (int j = 0; j < data.Count; j++)
                        {
                            data[j].Row = data[j].Row + dx;
                        }
                        items.AddRange(data);
                        //a+b+c:=45;

                        dx = arg0.Length + 2;
                        string arg1 = line.Split(":=")[1];

                        data = GetIdentificatorsInExpression(arg1, i);
                        for (int j = 0; j < data.Count; j++)
                        {
                            data[j].Row = data[j].Row + dx;
                        }
                        items.AddRange(data);

                    }
                    if (line.StartsWith("until"))
                    {
                        dx = 0;
                        line = line.Remove(0, 5);
                        dx = 5;
                        si = 0;
                        if (line.StartsWith(' '))
                        {
                            while (line[si].Equals(' '))
                            {
                                si++;
                            }
                        }
                        dx += si;
                        List<Item> data = GetIdentificatorsInExpression(line, i);
                        for (int j=0; j<data.Count; j++)
                        {
                            data[j].Row += dx - 1;
                        }
                        items.AddRange(data);


                    }
                    //if (!line.Equals(""))
                    //{

                    //    if (!line.Contains("write"))
                    //    {

                    //        foreach (Item item in items)
                    //        {
                    //            List<int> indexes = GetStartPositions(line, item.Name);
                    //            foreach (int ind in indexes)
                    //            {
                    //                Item tmp = new Item();
                    //                tmp.Name = item.Name;
                    //                tmp.Col = i;
                    //                tmp.Row = ind;
                    //                items1.Add(tmp);
                    //            }
                    //        }
                    //    }
                    //}
                }


                //numbers

                for (int i = 0; i < program.Count; i++)
                {
                    string item = program[i];
                    if (item != "")
                    {
                        int si = item.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });

                        int start_index = si;
                        bool sf = true;
                        if (!IsLocatedInSpaces(item, si))
                        {
                            if (si >= 0)
                            {
                                string number = "";
                                bool f1 = true; bool f2 = true;
                                if (si > 0)
                                {
                                    if (item[si - 1].Equals("_") || Regex.IsMatch(item[si - 1].ToString(), @"^[a-zA-Z]*$"))
                                    {
                                        f1 = false;
                                    }
                                    //<-
                                }
                                if (item[si + 1].Equals(" ") || Regex.IsMatch(item[si + 1].ToString(), @"^[a-zA-Z]*$") || item[si + 1].Equals("_"))
                                {
                                    f2 = false;
                                }
                                //->

                                if (f1 && f2)
                                {
                                    if (!item[si - 1].Equals("&") && !item[si - 1].Equals("$") && !item[si - 1].Equals("%"))
                                    {
                                        for (int j = si; j < item.Length; j++)
                                        {
                                            if (Regex.IsMatch(item[j].ToString(), @"^[0-9]*$") || (item[j].Equals('.')))
                                            {
                                                if (sf)
                                                {
                                                    start_index = j;
                                                    sf = false;
                                                }
                                                number += item[j];
                                                // cSi = j;
                                            }
                                            else
                                            {
                                                if (!number.Equals(""))
                                                {
                                                    Item item1 = new Item();
                                                    if (number.Contains(".."))
                                                    {
                                                        string[] tmp = number.Split("..");
                                                        item1.Name = tmp[0];
                                                        item1.Col = i;
                                                        item1.Row = si;
                                                        items.Add(item1);
                                                        item1 = new Item();
                                                        item1.Name = tmp[1];
                                                        item1.Col = i;
                                                        item1.Row = start_index + number.IndexOf("..") + 2;
                                                        items.Add(item1);
                                                        number = "";
                                                    }
                                                    else
                                                    {
                                                        item1.Name = number;
                                                        item1.Col = i;
                                                        item1.Row = start_index;
                                                        items.Add(item1);
                                                        number = "";
                                                    }
                                                    sf = true;
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            if (IsHexDigitValue(item))
                            {
                                List<int> indexes = new List<int>();
                                for (int j = item.IndexOf('$'); j < item.Length; j++)
                                {
                                    if (item[j].Equals('$'))
                                    {
                                        indexes.Add(j);
                                    }
                                }
                                foreach (int index in indexes)
                                {
                                    string number = "";
                                    for (int j = index + 1; j < item.Length; j++)
                                    {
                                        if (Regex.IsMatch(item[j].ToString(), @"^[a-fA-F0-9]*$"))
                                        {
                                            number += item[j];
                                        }
                                        else
                                        {
                                            if (number != "")
                                            {
                                                Item item1 = new Item();
                                                item1.Name = "$" + number;
                                                item1.Col = i;
                                                item1.Row = index;
                                                items.Add(item1);
                                                break;
                                            }

                                        }
                                    }
                                }
                            }
                            bool fff = IsOctDigitValue(item);
                            if (IsOctDigitValue(item))
                            {
                                List<int> indexes = new List<int>();
                                for (int j = item.IndexOf('&'); j < item.Length; j++)
                                {
                                    if (item[j].Equals('&'))
                                    {
                                        indexes.Add(j);
                                    }
                                }
                                foreach (int index in indexes)
                                {
                                    string number = "";
                                    for (int j = index + 1; j < item.Length; j++)
                                    {
                                        if (Regex.IsMatch(item[j].ToString(), @"^[0-7]*$"))
                                        {
                                            number += item[j];
                                        }
                                        else
                                        {
                                            Item item1 = new Item();
                                            item1.Name = "&" + number;
                                            item1.Col = i;
                                            item1.Row = index;
                                            items.Add(item1);
                                            break;
                                        }
                                    }
                                }
                            }
                            if (IsBinDigitValue(item))
                            {
                                List<int> indexes = new List<int>();
                                for (int j = item.IndexOf('%'); j < item.Length; j++)
                                {
                                    if (item[j].Equals('%'))
                                    {
                                        indexes.Add(j);
                                    }
                                }
                                foreach (int index in indexes)
                                {
                                    string number = "";
                                    for (int j = index + 1; j < item.Length; j++)
                                    {
                                        if (item[j].Equals('0') || item[j].Equals('1'))
                                        {
                                            number += item[j];
                                        }
                                        else
                                        {
                                            Item item1 = new Item();
                                            item1.Name = "%" + number;
                                            item1.Col = i;
                                            item1.Row = index;
                                            items.Add(item1);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //foreach (Item item in items)
                //{
                //    Console.WriteLine("name {0}, col {1}, row {2}", item.Name, item.Col, item.Row);
                //}

                using (StreamWriter writer = new StreamWriter(IdentificatorsPath))
                {
                    foreach (Item it in items)
                    {
                        writer.WriteLine(it.Name + "##" + it.Col + "##" + it.Row);
                    }
                }
            }
            
            catch (DirectoryNotFoundException ex0)
            {
                Console.WriteLine(ex0.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            //before it use RemoveLeftSpaces
            

        //}

        Console.ForegroundColor = ConsoleColor.White;
        }


        private List<Item> GetIdentificatorsInExpression(string value, int col)
        {
            //если нет разделителей, то проверить, является 
            //ли идентификатором, иначе проверить все числа на предмет вхождения (<- and ->)
            List<Item> items = new List<Item>();
            string temp = "";
            int si = -1;
            List<int> data = new List<int>();
            for (int i = 0; i < value.Length; i++)
            {
                if (!IsLocatedInSpaces(value, i))
                {
                    if (delimiters.Contains(value[i]))
                    {
                        if (temp != "")
                        {
                            if (IsCorrectIdentificator(temp))
                            {
                                AddItem(temp);
                            }

                        }
                        temp = "";
                        si = -1;
                    }
                    else
                    {
                        if (si < 0)
                        {
                            si = i;
                        }
                        temp += value[i];
                    }
                }
               
                
            }
            if ((items.Count==0) && (!string.IsNullOrEmpty(temp)))
            {
                if (IsCorrectIdentificator(temp))
                {
                    AddItem(temp);
                }
            }

            void AddItem(string val)
            {
                Item item = new Item();
                item.Name = val;
                item.Col = col;
                item.Row = si;
                items.Add(item);
            }
            return items;
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

        private List<int> GetStartPositions(string line, string value)
        {
            List<int> positions = new List<int>();
            int index = line.IndexOf(value);
            while (index > -1)
            {
                if (index + value.Length < line.Length)
                {
                    char c = line[index + value.Length];
                    string varn = "";
                    for (int j = index; j > 0; j--)
                    {
                        if (IsBorderSymbol(line[j]))
                            break;
                        varn += line[j];
                    }
                    char[] arr = varn.ToCharArray();
                    Array.Reverse(arr);
                    varn = new string(arr);

                    for (int j = index + 1; j < line.Length; j++)
                    {
                        if (IsBorderSymbol(line[j]))
                            break;
                        varn += line[j];
                    }

                    if (IsAllowVariable(varn))
                        if (c.Equals(':') || c.Equals(';') || c.Equals('+') || c.Equals('-') || c.Equals('/')
                            || c.Equals('*') || c.Equals('=') || c.Equals('>') || c.Equals('<')
                            || c.Equals('[') || c.Equals(']') || c.Equals('(') || c.Equals(')') || c.Equals(' '))
                        {
                            positions.Add(index);
                        }
                }
                index = line.IndexOf(value, index + value.Length);
            }
            
            return positions;
        }

        private bool IsHexDigitValue(string line)
        {
            bool flag = true;
            if (line.IndexOf('$') >= 0)
            {
                List<int> indexes = new List<int>();
                if (line.IndexOf(';') >= 0)
                    line = line.Remove(line.IndexOf(';'), 1);
                for (int j = 0; j < line.Length; j++)
                {
                    if (line[j].Equals('$'))
                    {
                        indexes.Add(j);
                    }
                }
                if (indexes.Count == 1)
                    indexes.Add(line.Length);
                if (indexes.Count > 0)
                {
                    for (int i = 0; i < indexes.Count - 1; i++)
                    {
                        for (int j = indexes[i] + 1; j < indexes[i + 1]; j++)
                        {
                            flag = Regex.IsMatch(line[j].ToString(), @"^[a-fA-F0-9]*$") || IsContainsValues(line[j], new char[] { '+', '-', '/', '*' });
                            if (!flag)
                                break;
                        }
                        if (!flag)
                            break;
                    }
                }
            }
            else
            {
                flag = false;
            }
            return flag;
       
        }

        private bool IsOctDigitValue(string line)
        {
            bool flag = true;
            if (line.IndexOf('&') >= 0)
            {
                List<int> indexes = new List<int>();
                if (line.IndexOf(';') >= 0)
                    line = line.Remove(line.IndexOf(';'), 1);
                for (int j = 0; j < line.Length; j++)
                {
                    if (line[j].Equals('&'))
                    {
                        indexes.Add(j);
                    }
                }
                if (indexes.Count == 1)
                    indexes.Add(line.Length);
                if (indexes.Count > 0)
                {
                    for (int i = 0; i < indexes.Count - 1; i++)
                    {
                        for (int j = indexes[i] + 1; j < indexes[i + 1]; j++)
                        {
                            flag = Regex.IsMatch(line[j].ToString(), @"^[0-7]*$") || IsContainsValues(line[j], new char[] { '+', '-', '/', '*' });
                            if (!flag)
                                break;
                        }
                        if (!flag)
                            break;
                    }
                }
            }
            else
            {
                flag = false;
            }


            return flag;
        }

        private bool IsBinDigitValue(string line)
        {
            bool flag = true;
            if (line.IndexOf('%') >= 0)
            {
                List<int> indexes = new List<int>();
                if (line.IndexOf(';') >= 0)
                    line = line.Remove(line.IndexOf(';'), 1);
                for (int j = 0; j < line.Length; j++)
                {
                    if (line[j].Equals('%'))
                    {
                        indexes.Add(j);
                    }
                }
                if (indexes.Count == 1)
                    indexes.Add(line.Length);
                if (indexes.Count > 0)
                {
                    for (int i = 0; i < indexes.Count - 1; i++)
                    {
                        for (int j = indexes[i] + 1; j < indexes[i + 1]; j++)
                        {
                            flag = IsContainsValues(line[j], new char[] { '0', '1' }) || IsContainsValues(line[j], new char[] { '+', '-', '/', '*' });
                            if (!flag)
                                break;
                        }
                        if (!flag)
                            break;
                    }
                }
            }
            else
            {
                flag = false;
            }


            return flag;
        }

        private bool IsContainsValues(char line, char[] values)
        {
            bool f = false;
            foreach (char v in values)
            {
                f = line.Equals(v);
                if (f)
                    break;
            }
            return f;
        }

        private bool IsBorderSymbol(char c)
        {
            bool f = true;
            f = c.Equals('(') || c.Equals(')') || c.Equals('[') || c.Equals(']') || c.Equals(' ') || c.Equals('>')
                || c.Equals('<') || c.Equals('=') || c.Equals(';') || c.Equals('+') || c.Equals('-') || c.Equals('/') || c.Equals('*');
            return f;
        }

        private bool IsAllowVariable(string variable)
        {
            bool f = true;
            for (int i = 0; i < BaseTypes.Length; i++)
            {
                f = !variable.Equals(BaseTypes[i]);
                if (!f)
                    break;
            }
            if (f)
            {
                for (int i = 0; i < ConditionStructure.Length; i++)
                {
                    f = !variable.Equals(ConditionStructure[i]);
                    if (!f)
                        break;
                }
            }
            if (f)
            {
                for (int i = 0; i < LogicalOperators.Length; i++)
                {
                    f = !variable.Equals(LogicalOperators[i]);
                    if (!f)
                        break;
                }
            }
            if (f)
            {
                for (int i = 0; i < LogicalValues.Length; i++)
                {
                    f = !variable.Equals(LogicalValues[i]);
                    if (!f)
                        break;
                }
            }
            if (f)
            {
                for (int i = 0; i < BorderOfStatements.Length; i++)
                {
                    f = !variable.Equals(BorderOfStatements[i]);
                    if (!f)
                        break;
                }
            }
            if (f)
            {
                for (int i = 0; i < ArrayStructure.Length; i++)
                {
                    f = !variable.Equals(ArrayStructure[i]);
                    if (!f)
                        break;
                }
            }
            if (f)
            {
                for (int i = 0; i < RepeatStructure.Length; i++)
                {
                    f = !variable.Equals(RepeatStructure[i]);
                    if (!f)
                        break;
                }
            }
            if (f)
            {
                for (int i = 0; i < IOStructure.Length; i++)
                {
                    f = !variable.Equals(IOStructure[i]);
                    if (!f)
                        break;
                }
            }
            return f;
        }

        public void RemoveComments()
        {
            bool f = false;
            List<string> data = new List<string>();
            for (int i = 0; i < program.Count; i++)
            {
                string line = program[i];
                if (line.IndexOf("{") >= 0 && line.IndexOf("}") >= 0)
                {
                    int n = line.IndexOf("}") - line.IndexOf("{");
                    data.Add(line.Remove(line.IndexOf("{"), n));

                }
                if (line.IndexOf("{") >= 0 && line.IndexOf("}") == -1)
                {
                    string tmp = line.Substring(0, line.IndexOf("{"));
                    if (tmp != "")
                        data.Add(tmp);
                    f = true;
                }
                if (program[i].IndexOf("}") >= 0 && program[i].IndexOf("{") == -1)
                {
                    string tmp = line.Substring(program[i].IndexOf("}"), line.Length);
                    if ((tmp != "") && !(tmp.Equals("}")))
                        data.Add(tmp);
                    f = false;
                }
                if (!f)
                {
                    if (line.IndexOf("//") >= 0)
                    {
                        string tmp = line.Substring(0, line.IndexOf("//"));
                        data.Add(tmp);
                    }
                    else
                    {
                        if ((line != "") && !(line.Equals("}")))
                            data.Add(line);
                    }
                }

            }
            program = data;

        }

        public void FormTokens()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            List<Item> tokens = new List<Item>();
          //  if (CheckProgram().Count == 0)
            {
                try
                {

                    for (int i = 0; i < program.Count; i++)
                    {
                        string line = program[i];
                        if (line != "")
                        {
                            if (line.Contains("var"))
                            {
                                Item item = new Item();
                                item.Name = "var";
                                item.Col = i;
                                item.Row = line.IndexOf("var");
                                tokens.Add(item);
                            }
                            if (IsContainsSymbols(line, BorderOfStatements) >= 0)
                            {
                                List<Item> items = GetContainsSymbolName(line, BorderOfStatements);

                                foreach (Item item in items)
                                {
                                    bool f = !IsLocatedInSpaces(line, item.Row);
                                    if (!IsLocatedInSpaces(line, item.Row))
                                    {
                                        Item tmp = item;
                                        tmp.Col = i;
                                        tokens.Add(tmp);
                                    }
                                }
                            }
                            if (IsContainsSymbols(line, BaseTypes) > 0)
                            {
                                List<Item> items = GetContainsSymbolName(line, BaseTypes);
                                foreach (Item item in items)
                                {
                                    if (!IsLocatedInSpaces(line, item.Row))
                                    {
                                        Item tmp = item;
                                        tmp.Col = i;
                                        tokens.Add(tmp);
                                    }
                                }
                            }
                            if (line.Contains(' '))
                            {
                                int st = line.IndexOf(' ');
                                string pr = "";
                                for (int j = line.IndexOf(' '); j < line.Length; j++)
                                {
                                    if (line[j].Equals(' '))
                                    {
                                        if (st < 0)
                                            st = j;
                                        pr += line[j];
                                    }
                                    else
                                    {
                                        if (!pr.Equals(""))
                                        {
                                            Item item = new Item();
                                            item.Name = pr;
                                            item.Col = i;
                                            item.Row = st;
                                            tokens.Add(item);
                                            st = -1;
                                            pr = "";
                                        }
                                    }
                                }
                            }
                            if (!line.Contains("write"))
                            {
                                if (IsContainsSymbols(line, ArifmeticOperators) >= 0)
                                {
                                    List<Item> items = GetContainsSymbolName(line, ArifmeticOperators);
                                    foreach (Item item in items)
                                    {
                                        bool f = !IsLocatedInSpaces(line, item.Row);
                                        if (!IsLocatedInSpaces(line, item.Row))
                                        {
                                            Item tmp = item;
                                            tmp.Col = i;
                                            tokens.Add(tmp);
                                        }
                                    }
                                }
                            }

                            if (IsContainsSymbols(line, LogicalOperators) >= 0)
                            {
                                List<Item> items = GetContainsSymbolName(line, LogicalOperators);
                                foreach (Item item in items)
                                {
                                    if (!IsLocatedInSpaces(line, item.Row))
                                    {
                                        Item tmp = item;
                                        tmp.Col = i;
                                        tokens.Add(tmp);
                                    }
                                }
                            }

                            if (IsContainsSymbols(line, LogicalValues) >= 0)
                            {
                                List<Item> items = GetContainsSymbolName(line, LogicalValues);
                                foreach (Item item in items)
                                {
                                    if (!IsLocatedInSpaces(line, item.Row))
                                    {
                                        Item tmp = item;
                                        tmp.Col = i;
                                        tokens.Add(tmp);
                                    }
                                }
                            }

                            if (IsContainsSymbols(line, CompareOperators) >= 0)
                            {
                                List<Item> items = GetContainsSymbolName(line, CompareOperators);
                                foreach (Item item in items)
                                {
                                    if (!IsLocatedInSpaces(line, item.Row))
                                    {
                                        Item tmp = item;
                                        tmp.Col = i;
                                        tokens.Add(tmp);
                                    }
                                }
                            }
                            if (IsContainsSymbols(line, ConditionStructure) >= 0)
                            {
                                List<Item> items = GetContainsSymbolName(line, ConditionStructure);
                                foreach (Item item in items)
                                {
                                    bool f = !IsLocatedInSpaces(line, item.Row);
                                    if (!IsLocatedInSpaces(line, item.Row))
                                    {
                                        Item tmp = item;
                                        tmp.Col = i;
                                        tokens.Add(tmp);
                                    }
                                }
                            }
                            if (IsContainsSymbols(line, RepeatStructure) >= 0)
                            {
                                List<Item> items = GetContainsSymbolName(line, RepeatStructure);
                                foreach (Item item in items)
                                {
                                    bool f = !IsLocatedInSpaces(line, item.Row);
                                    if (!IsLocatedInSpaces(line, item.Row))
                                    {
                                        Item tmp = item;
                                        tmp.Col = i;
                                        tokens.Add(tmp);
                                    }
                                }
                            }
                            if (IsContainsSymbols(line, ArrayStructure) >= 0)
                            {
                                List<Item> items = GetContainsSymbolName(line, ArrayStructure);
                                foreach (Item item in items)
                                {
                                    if (!IsLocatedInSpaces(line, item.Row))
                                    {
                                        Item tmp = item;
                                        tmp.Col = i;
                                        tokens.Add(tmp);
                                    }
                                }
                            }

                            if (IsContainsSymbols(line, IOStructure) >= 0)
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
                                    if (arg.Equals("writeln"))
                                    {
                                        pattern = "writeln";
                                    }
                                    Item item = new Item();
                                    item.Name = pattern;
                                    item.Col = i;
                                    item.Row = line.IndexOf(pattern);
                                    tokens.Add(item);

                                    int sind = program[i].IndexOf("(") + 1;
                                    int count = program[i].IndexOf(")") - sind;
                                    if (count<0)
                                    {
                                        count = program[i].Length - sind;
                                    }
                                    string msg = program[i].Substring(sind, count);
                                    item = new Item();
                                    item.Name = msg;
                                    item.Col = i;
                                    item.Row = program[i].IndexOf("(") + 1;
                                    tokens.Add(item);
                                }
                                if (program[i].Contains("read"))
                                {
                                    string arg = program[i].Substring(0, program[i].IndexOf("("));
                                    arg = arg.Trim();
                                    if (arg.Equals("read"))
                                    {
                                        pattern = "read";
                                    }
                                    if (arg.Equals("readln"))
                                    {
                                        pattern = "readln";
                                    }
                                    Item item = new Item();
                                    item.Name = pattern;
                                    item.Col = i;
                                    item.Row = line.IndexOf(pattern);
                                    tokens.Add(item);
                                }

                                //Item item = new Item();
                                //item.Name = pattern;
                                //item.Col = i;
                                //item.Row = line.IndexOf(pattern);
                                //tokens.Add(item);
                            }
                            if (IsContainsSymbols(line, ServiceSymbols) >= 0)
                            {
                                //if (line.Contains(".."))
                                //{
                                //    string name = "..";
                                //    Item item = new Item();
                                //    item.Name = name;
                                //    item.Row = line.IndexOf("..");
                                //    item.Col = i;
                                //    tokens.Add(item);
                                //}

                                //if (line.Contains(".") && !line.Contains(".."))
                                //{
                                //    string name = ".";
                                //    Item item = new Item();
                                //    item.Name = name;
                                //    item.Row = line.IndexOf(".");
                                //    item.Col = i;
                                //    tokens.Add(item);
                                //}

                                if (line.Contains(":="))
                                {
                                    int si = line.IndexOf(":=");
                                    Item item = new Item();
                                    item.Name = ":=";
                                    item.Col = i;
                                    item.Row = si;
                                    tokens.Add(item);
                                }

                                if (line.Contains("=") && !line.Contains(":="))
                                {
                                    if (!line.Contains("write"))
                                    {
                                        int si = line.IndexOf("=");
                                        Item item = new Item();
                                        item.Name = "=";
                                        item.Col = i;
                                        item.Row = si;
                                        tokens.Add(item);
                                    }
                                }

                                if (line.Contains(":") && !(line.Contains(":=")))
                                {
                                    int si = line.IndexOf(":");
                                    Item item = new Item();
                                    item.Name = ":";
                                    item.Col = i;
                                    item.Row = si;
                                    tokens.Add(item);
                                }

                                List<Item> items = GetAllContainsSymbols(line, ServiceSymbols);
                                for (int j = 0; j < items.Count; j++)
                                {
                                    Item tmp = items[j];
                                    tmp.Col = i;
                                    items[j] = tmp;
                                }
                                foreach (Item item in items)
                                {
                                    tokens.Add(item);
                                }
                            }
                            if (line.Contains('\"'))
                            {
                                if (!program[i].Contains("write"))
                                {
                                    List<int> indexes = GetAllContains(line, "\"");
                                    foreach (int pos in indexes)
                                    {
                                        Item item = new Item();
                                        item.Name = "\"";
                                        item.Col = i;
                                        item.Row = pos;
                                        tokens.Add(item);
                                    }
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
                            writer.WriteLine(item.Name + "##" + item.Col + "##" + item.Row);
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
            }



            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ResetFiles()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            try
            {
                FileStream fstream = new FileStream(IdentificatorsPath, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(fstream);
                fstream = new FileStream(TokensPath, FileMode.Create, FileAccess.Write);
                writer = new StreamWriter(fstream);
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

        private int IsContainsSymbols(string line, string[] symbols)
        {
            int pos = -1;
            foreach (string p in symbols)
            {
                pos = line.IndexOf(p);
                if (pos >= 0)
                    break;
            }
            return pos;
        }

        private List<Item> GetAllContainsSymbols(string line, string[] patterns)
        {
            List<Item> result = new List<Item>();

            foreach (string p in patterns)
            {
                if (p.Equals("."))
                {
                    if (line.Contains(".."))
                    {
                        List<int> indexes = GetAllContains(line, p);
                        foreach (int index in indexes)
                        {
                            Item item = new Item();
                            item.Row = index;
                            item.Name = p;
                            result.Add(item);
                        }
                    }
                    if (line.Contains(".") && !line.Contains(".."))
                    {
                        List<int> indexes = GetAllContains(line, p);
                        foreach (int index in indexes)
                        {
                            Item item = new Item();
                            item.Row = index;
                            item.Name = p;
                            result.Add(item);
                        }
                    }
                }
                else
                {
                    if (line.Contains(p) && !p.Equals(".") && !p.Equals(".."))
                    {
                        List<int> indexes = GetAllContains(line, p);
                        foreach (int index in indexes)
                        {
                            Item item = new Item();
                            item.Row = index;
                            item.Name = p;
                            result.Add(item);
                        }
                    }
                }
            }

            return result;
        }

        private List<Item> GetContainsSymbolName(string line, string[] patterns)
        {
            List<Item> items = new List<Item>();

            foreach (string p in patterns)
            {
                bool f = p.Equals(".");
                bool f1 = p.Equals("<") || p.Equals(">");
                bool f2 = p.Equals("or");
                bool f3 = p.Equals("in");
                if (f)
                {
                    int index = line.IndexOf("..");
                    while (index > 0)
                    {
                        Item item = new Item();
                        item.Name = "..";
                        item.Row = index;
                        items.Add(item);
                        line = line.Remove(index, 2);
                        index = line.IndexOf("..");
                    }
                    index = line.IndexOf(".");
                    while (index > 0)
                    {
                        Item item = new Item();
                        item.Name = ".";
                        item.Row = index;
                        items.Add(item);
                        line = line.Remove(index, 1);
                        index = line.IndexOf(".");
                    }
                }
                if (f1)
                {
                    int index = line.IndexOf("<>");
                    while (index > 0)
                    {
                        Item item = new Item();
                        item.Name = "<>";
                        item.Row = index;
                        items.Add(item);
                        line = line.Remove(index, 2);
                        index = line.IndexOf("<>");
                    }
                    index = line.IndexOf("<");
                    while (index > 0)
                    {
                        Item item = new Item();
                        item.Name = "<";
                        item.Row = index;
                        items.Add(item);
                        line = line.Remove(index, 1);
                        index = line.IndexOf("<");
                    }
                    index = line.IndexOf(">");
                    while (index > 0)
                    {
                        Item item = new Item();
                        item.Name = ">";
                        item.Row = index;
                        items.Add(item);
                        line = line.Remove(index, 1);
                        index = line.IndexOf(">");
                    }
                }
                if (f2)
                {
                    int index = line.IndexOf("xor");
                    while (index > 0)
                    {
                        Item item = new Item();
                        item.Name = "xor";
                        item.Row = index;
                        items.Add(item);
                        line = line.Remove(index, 3);
                        index = line.IndexOf("xor");
                    }
                    index = line.IndexOf("or");
                    while (index > 0)
                    {
                        Item item = new Item();
                        item.Name = "or";
                        item.Row = index;
                        items.Add(item);
                        line = line.Remove(index, 2);
                        index = line.IndexOf("or");
                    }
                }
                if (f3)
                {
                    List<int> temp = GetAllContains(line, "in");
                    foreach (int pos in temp)
                    {
                        if (!IsLocatedInSpaces(line, pos))
                        {
                            if (pos-1>=0)
                            {
                                if (line[pos - 1].Equals(' '))
                                {
                                    if (pos+3<line.Length)
                                    {
                                        if ((line[pos + 3].Equals('(')) || (line[pos + 3].Equals(' ')))
                                        {
                                            Item item = new Item();
                                            item.Name = "in";
                                            item.Row = pos;
                                            items.Add(item);
                                        }
                                    }
                                }
                            }
                          
                        }
                    }
                }
                if (line.Contains(p) && !f && !f1 && !f2 && !f3)
                {
                    List<int> indexes = GetAllContains(line, p);
                    foreach (int index in indexes)
                    {
                        Item item = new Item();
                        item.Name = p;
                        item.Row = index;
                        items.Add(item);
                    }
                }
            }
            return items;
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

        public void RemoveSpacesAndEmptySymbols()
        {
            char[] patterns = new char[] { '^','+', '-', '*', '/', '<', '>', '=' };

            int ind = 0;
            if (program.Count>0)
            {
                while (!program[ind].Equals("begin"))
                {
                    string line = program[ind];
                    line = RemoveLeftSpaces(line);
                    if (line.StartsWith("var"))
                    {
                        line = line.Remove(0, 4);


                    }
                    string[] args = line.Split(":")[0].Split(',');
                    string arg1 = line.Split(':')[1];
                    for (int i = 0; i < args.Length - 1; i++)
                    {
                        args[i] = args[i].Trim() + ',';
                    }
                    args[args.Length - 1] = args[args.Length - 1].Trim();
                    line = "var ";
                    foreach (string arg in args)
                    {
                        line += arg;
                    }
                    line += ':' + arg1;
                    program[ind] = line;
                    ind++;
                }
                for (int i = ind + 1; i < program.Count; i++)
                {
                    program[i] = RemoveLeftSpaces(program[i]);
                    program[i] = RemoveRigthSpaces(program[i]);
                }
            }
           
            int t = 0;
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
                while (index>=0)
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

                
                line = line.Remove(index+1);
            }
            return line;
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
                        //if (line.IndexOf(";") >= 0)
                        //{
                        //    string[] array = line.Split(";");

                        //    foreach (string item in array)
                        //    {
                        //        if (item != "")
                        //        {
                        //            program.Add(item + ";");
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    program.Add(line);
                        //}
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
            bool IsAllovSymbol(string line)
            {
                return !line.Equals("begin") || !line.Equals("end") || line.Contains("if") || line.Contains("else") || line.Contains("while");
            }
            Console.ForegroundColor = ConsoleColor.White;
            return program;
        }
    }
}