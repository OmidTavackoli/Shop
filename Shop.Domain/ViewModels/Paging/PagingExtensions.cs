using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Paging
{
    public static class PagingExtentions
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> query, BasePaging basePaging)
        {
            return query.Skip(basePaging.SkipEntitiy).Take(basePaging.TakeEntity);
        }
    }
}
