using Shop.Domain.Models.Wallet;
using Shop.Domain.ViewModels.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Wallet
{
    public class FilterWalletViewModel : BasePaging
    {
        #region properties
        public long? UserId { get; set; }
        public List<UserWallet> UserWallets { get; set; }
        #endregion

        #region methods
        public FilterWalletViewModel SetWallets(List<UserWallet> userWallets)
        {
            this.UserWallets = userWallets;
            return this;
        }

        public FilterWalletViewModel SetPaging(BasePaging paging)
        {
            this.PageId = paging.PageId;
            this.AllEntityCount = paging.AllEntityCount;
            this.StartPage = paging.StartPage;
            this.EndPage = paging.EndPage;
            this.TakeEntity = paging.TakeEntity;
            this.CountForShowAfterAndBefor = paging.CountForShowAfterAndBefor;
            this.SkipEntitiy = paging.SkipEntitiy;
            this.PageCount = paging.PageCount;

            return this;
        }

        #endregion
    }
}
