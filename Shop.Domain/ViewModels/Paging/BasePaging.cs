using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Paging
{
    public class BasePaging
    {
        public BasePaging()
        {
            PageId = 1;
            TakeEntity = 15;
            CountForShowAfterAndBefor = 5;
        }
        public int PageId { get; set; }
        public int PageCount { get; set; }
        public int AllEntityCount { get; set; }
        public int CountForShowAfterAndBefor { get; set; }
        public int SkipEntitiy { get; set; }
        public int TakeEntity { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }

        public BasePaging GetCurrentPaging()
        {
            return this;
        }
    }
}
