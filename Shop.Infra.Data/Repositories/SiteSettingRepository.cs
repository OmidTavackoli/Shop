using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Site;
using Shop.Domain.ViewModels.Paging;
using Shop.Domain.ViewModels.Site.Sliders;
using Shop.Presistance.Context;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presistance.Repositories
{
    public class SiteSettingRepository(ShopDbContext context) : ISiteSettingRepository
    {
        #region constractor
        private readonly ShopDbContext _context = context;


        #endregion

        #region slider

        public async Task AddSlider(Slider slider)
        {
            await _context.Sliders.AddAsync(slider);
        }

        public async Task<FilterSlidersViewModel> FliterSliders(FilterSlidersViewModel filter)
        {
            var query = _context.Sliders.AsQueryable();

            #region filter
            if (!string.IsNullOrEmpty(filter.SliderTitle))
            {
                query = query.Where(c => EF.Functions.Like(c.SliderTitle, $"%{filter.SliderTitle}%"));
            }
            #endregion


            #region paging
            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.CountForShowAfterAndBefor);
            var allData = await query.Paging(pager).ToListAsync();
            #endregion

            return filter.SetPaging(pager).SetSilders(allData);
        }

        public async Task<Slider> GetSliderById(long sliderId)
        {
            return await _context.Sliders.AsQueryable()
                .SingleOrDefaultAsync(c => c.Id == sliderId);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public void UpdateSlider(Slider slider)
        {
            _context.Sliders.Update(slider);
        }
        #endregion
    }
}
