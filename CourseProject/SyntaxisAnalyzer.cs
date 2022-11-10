using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CourseProject
{
    public class SyntaxisAnalyzer
    {


        const string IdentificatorsPath = "D:\\COMPILER\\data\\identificators.txt";

        const string TokensPath = "D:\\COMPILER\\data\\tokens.txt";

        const string PatternsPath = "D:\\COMPILER\\data\\patterns.txt";



        struct Item
        {
            public string Value { get; set; }

            public int Position { get; set; }
        }

        public void FormSyntaxisTree()
        {

        }




        public void ParseProgramLine(string line)
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
                int t = 0;
                tree.SetHead();
                tree.ScanTree(tree.Root);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

        }
        List<string> ReadProgram()
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

        List<string> ParseExpression(string e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            List<string> result = new List<string>();
            List<int> startPositions = new List<int>();
            List<int> endPositions = new List<int>();
            for (int i = 0; i < e.Length; i++)
            {
                if (e[i].Equals('('))
                {
                    startPositions.Add(i);
                }
            }

            for (int i = e.Length - 1; i > 0; i--)
            {
                if (e[i].Equals(')'))
                {
                    endPositions.Add(i);
                }
            }
            if (startPositions.Count == endPositions.Count)
            {
                foreach (int sp in startPositions)
                {
                    string line = "";
                    int ind = 0;
                    for (int j = sp + 1; j < e.Length; j++)
                    {

                        if (e[j].Equals('('))
                            ind++;

                        if ((!e[j].Equals('(')) && (!e[j].Equals(')')) && (ind == 0))
                        {
                            line += e[j];
                            if (e[j + 1].Equals(')'))
                            {
                                break;
                            }
                        }

                        if (e[j].Equals(')'))

                            ind--;
                    }
                    result.Add(line);
                }
            }
            else
            {
                Console.WriteLine("count of open and close parentheses mus be equal");
            }
            for (int i = 0; i < startPositions.Count; i++)
            {
                Console.WriteLine("startpoint {0}", startPositions[i]);
            }
            for (int i = 0; i < endPositions.Count; i++)
            {
                Console.WriteLine("endpoint {0}", endPositions[i]);
            }
            foreach (string tmp in result)
            {
                Console.WriteLine(tmp);
            }
            Console.ForegroundColor = ConsoleColor.White;
            return result;
        }

        public void PrintRepairProgram()
        {
            int ind = 0;
            try
            {
                using (StreamReader reader = new StreamReader(TokensPath))
                {
                    string? line = File.ReadLines(TokensPath).Last();
                    ind = int.Parse(line.Split(" ")[1]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            for (int i = 0; i < ind; i++)
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
                        int col = int.Parse(line.Split(' ')[1]);
                        string name = line.Split(' ')[0];
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
                            int col = int.Parse(line.Split(' ')[1]);
                            string val = line.Split(' ')[0];
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
                        string arg0 = tmp.Split(" ")[0];
                        int arg1 = int.Parse(tmp.Split(" ")[1]);
                        int arg2 = int.Parse(tmp.Split(" ")[2]);
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
                        string arg0 = tmp.Split(" ")[0];
                        int arg1 = int.Parse(tmp.Split(" ")[1]);
                        int arg2 = int.Parse(tmp.Split(" ")[2]);
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
