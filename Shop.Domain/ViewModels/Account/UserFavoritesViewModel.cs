using Shop.Domain.Models.Account;
using Shop.Domain.ViewModels.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Account
{
    public class UserFavoritesViewModel : BasePaging
    {
        public List<UserFavorite> UserFavorites { get; set; }
    }
}
