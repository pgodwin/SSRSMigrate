using SSRSMigrate.SSRS.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Tree
{
    public class Root
    {
        public Root(IReportServerReader reader)
        {
            this.Title = reader.Url;
            this.Reader = reader;
        }

        public string Title { get; private set; }

        public object Reader { get; private set; }

    }


    public class Tree<T>
    {
        private T data;
        private LinkedList<Tree<T>> children;


    }
}
