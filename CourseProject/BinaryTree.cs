namespace CourseProject
{
    public class BinaryTree : Service
    {
        public class Item
        {
            public string Value { get; set; }

            public Item Parent { get; set; }

            public Item LeftChild { get; set; }

            public Item RightChild { get; set; }

            public Item()
            {
                this.Parent = null;
                this.LeftChild = null;
                this.RightChild = null;
            }
        }

        public class RepairItem
        {
            public string Value { get; set; }

            public int Level { get; set; }

            public RepairItem()
            {
                this.Value = null;
                this.Level = 0;
            }

            public RepairItem(string value, int level)
            {
                Value = value;
                Level = level;
            }
        }

        public Item Root { get; set; }

        private Item Result { get; set; }
        public bool IsPermitEmptyItems { get; set; } = true;

        public BinaryTree()
        {
            this.Root = new Item();
        }

        private bool IsEmptyElement(Item item)
        {
            bool flag = false;
            if (item == null)
            {
                flag = true;
            }
            else
            {
                flag = item.Parent == null && item.LeftChild == null && item.RightChild == null && item.Value == null;
            }
            return flag;
        }

        public void SetRootValue(string value)
        {
            this.Root.Value = value;
        }

        public void SetHead()
        {
            if (Root != null)
            {
                while (!IsEmptyElement(Root.Parent))
                {
                    Root = Root.Parent;
                }
            }
        }

        public void SetLeftChild(Item tree)
        {
            if (tree != null)
            {
                Root.LeftChild = tree;
            }
        }

        public void SetRightChild(Item tree)
        {
            if (tree != null)
            {
                Root.RightChild = tree;
            }
        }

        public void AddLeftChild(string value)
        {
            if (value != null)
            {
                Item child = new Item();
                child.Value = value;
                child.LeftChild = null;
                child.RightChild = null;
                if (IsPermitEmptyItems)
                {
                    Root.LeftChild = child;
                    child.Parent = Root;
                    Root = child;
                }
                else
                {
                    if (!IsEmptyElement(Root))
                    {
                        child.Parent = Root;
                        child.Parent.LeftChild = child;
                        Root = child;
                        //Root = Root.LeftChild;
                    }
                    else
                    {
                        Root = child;
                    }
                }
            }
        }

        public void AddRightChild(string value)
        {
            if (value != null)
            {
                Item item = new Item();
                item.Value = value;
                item.LeftChild = null;
                item.RightChild = null;
                if (IsPermitEmptyItems)
                {
                    Root.RightChild = item;
                    item.Parent = Root;
                    Root = item;
                    //Root = Root.RightChild;
                }
                else
                {
                    if (!IsEmptyElement(Root))
                    {
                        item.Parent = Root;
                        item.Parent.RightChild = item;
                        Root = item;
                    }
                    else
                    {
                        Root = item;
                    }
                }
            }
        }

        public void SetParent(Item item)
        {
            if (item.Parent != null)
            {
                Root = Root.Parent;
            }
        }

        public void ResetTree()
        {
            this.Root = new Item();
        }

        public void RemoveItem(string value)
        {
            throw new Exception("not realized");
        }

        public void SearchElement(Item item)
        {
            if (item != null)
            {
            }
        }

        string[] ArifmeticOperators = { "+", "-", "/", "*" };

        bool IsNumber(string value)
        {
            return false;
        }

        public void OptimizeTree(Item root)
        {
            if (root==null)
            {
                return;
            }
            if (root.LeftChild != null)
            {
                if (ArifmeticOperators.Contains(root.LeftChild.Value))
                {
                    OptimizeTree(root.LeftChild);
                }
                if (IsNumber(root.LeftChild.Value))
                {
                    double n = double.Parse(root.LeftChild.Value);
                    if (root.RightChild!=null)
                    {
                        if (ArifmeticOperators.Contains(root.RightChild.Value))
                        {
                            OptimizeTree(root.RightChild);
                        }
                        if (IsNumber(root.RightChild.Value))
                        {
                            double m = double.Parse(root.RightChild.Value);  
                            if (ArifmeticOperators.Contains(root.Value))
                            {
                                double result = 0;
                                switch (root.Value)
                                {
                                    case "+":
                                        result = m + n;
                                        break;
                                    case "-":
                                        result = m - n;
                                        break;
                                    case "*":
                                        result = m * n;
                                        break;
                                    case "/":
                                        result = m / n;
                                        break;
                                }
                                Item item = new Item();
                                item.Value = result.ToString();
                                root = item;
                            }
                           
                        }
                    }
                }
            }
        }

        public static void SaveTreeTofile(BinaryTree tree, string path = OutputBinaryTreePath)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (tree != null)
            {
                List<string> listTree = new List<string>();
                ConvertTreeToList(ref listTree, tree.Root);
                try
                {
                    using (StreamWriter writer = new StreamWriter(path))
                    {
                        foreach (string line in listTree)
                        {
                            writer.WriteLine(line);
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
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static BinaryTree RepairTreeFromFile(string path = OutputBinaryTreePath)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            List<RepairItem> repairTree = new List<RepairItem>();
            BinaryTree tree = new BinaryTree();
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string value = line.Trim();
                        int lvl = GetCountOfSpaces(line);
                        RepairItem item = new RepairItem(value, lvl);
                        repairTree.Add(item);
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
            int GetCountOfSpaces(string line)
            {
                int n = 0;
                foreach (char symbol in line)
                {
                    if (symbol.Equals(' '))
                        n++;
                }
                return n;
            }
            //sort by increase
            for (int i = 0; i < repairTree.Count - 1; i++)
            {
                for (int j = i + 1; j < repairTree.Count; j++)
                {
                    if (repairTree[i].Level > repairTree[j].Level)
                    {
                        RepairItem item = repairTree[i];
                        repairTree[i] = repairTree[j];
                        repairTree[j] = item;
                    }
                }
            }
            tree.SetRootValue(repairTree[0].Value);
            Repair(ref tree, repairTree);
            Console.ForegroundColor = ConsoleColor.White;
            return tree;
        }

        private static void Repair(ref BinaryTree tree, List<RepairItem> items)
        {
            int lvl = 1;
            if (items != null)
            {
                for (int i = 1; i < items.Count; i += 2)
                {
                    if (items[i].Level == lvl + 1)
                    {
                        tree.AddLeftChild(items[i].Value);
                        tree.SetParent(tree.Root);
                    }
                    if (items[i + 1].Level == lvl + 1)
                    {
                        tree.AddRightChild(items[i + 1].Value);
                        tree.SetParent(tree.Root);
                    }
                    lvl++;
                }
            }
        }

        public void PrintTree(Item root, int index = 0)
        {
            if (root != null)
            {
                string tmp = "";
                for (int i = 0; i < index; i++)
                {
                    tmp += " ";
                }
                tmp += "value " + root.Value;
                Console.WriteLine(tmp);
                if (root.LeftChild != null)
                    PrintTree(root.LeftChild, index + 1);
                if (root.RightChild != null)
                    PrintTree(root.RightChild, index + 1);
            }
        }

        private static void ConvertTreeToList(ref List<string> list, Item root, int index = 0)
        {
            if (root != null)
            {
                string tmp = "";
                if (root.Value != null)
                {
                    for (int i = 0; i < index; i++)
                    {
                        tmp += " ";
                    }
                    tmp += root.Value;
                    list.Add(tmp);
                }

                if (root.LeftChild != null)
                    ConvertTreeToList(ref list, root.LeftChild, index + 1);
                if (root.RightChild != null)
                    ConvertTreeToList(ref list, root.RightChild, index + 1);
            }
        }
    }
}