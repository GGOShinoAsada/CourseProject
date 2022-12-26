using System.Text.RegularExpressions;

namespace CourseProject
{
    public class Service
    {
        public const string OutputBinaryTreePath = "D:\\COMPILER\\data\\tree.txt";

        public const string IdentificatorsPath = "D:\\COMPILER\\data\\identificators.txt";

        public const string TokensPath = "D:\\COMPILER\\data\\tokens.txt";

        public const string PatternsPath = "D:\\COMPILER\\data\\patterns.txt";

        public const string DefaultValue = "OP";

        protected string RemoveLeftSpaces(string line)
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

        protected string RemoveRigthSpaces(string line)
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

                line = line.Remove(index + 1);
            }
            return line;
        }

        protected bool IsCorrectIdentificator(string line)
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

        protected string GetSubsString(string line, int sp, int ep)
        {
            string result = string.Empty;
            for (int i = sp; i <= ep; i++)
            {
                result += line[i];
            }
            return result;
        }
    }
}