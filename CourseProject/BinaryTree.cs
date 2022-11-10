using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject
{
    public class BinaryTree
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

        public Item Root { get; set; }

        private Item Result { get; set; }
        public bool IsPermitEmptyItems { get; set; } = true;

        public BinaryTree()
        {
            this.Root = new Item();
            
        }

        bool IsEmptyElement(Item item)
        {
            bool flag = false;
            if (item==null)
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
            if (Root!=null)
            {
                while (!IsEmptyElement(Root.Parent))
                {
                    Root = Root.Parent;
                }
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
            if (item.Parent!=null)
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
            if (item!=null)
            {

            }

       }

        public void ScanTree(Item item, int index=0)
        {
            if (item!=null)
            {
                string tmp = "";
                for (int i = 0; i < index; i++)
                {
                        tmp += " ";
                }
                tmp += "value " + item.Value;
                Console.WriteLine(tmp);
                
                if (item.LeftChild != null)
                    ScanTree(item.LeftChild, index + 1);
                if (item.RightChild != null)
                    ScanTree(item.RightChild, index + 1);
            }
         
        }

    }
}
