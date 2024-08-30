using Shop.Domain.Models.Site;
using Shop.Domain.ViewModels.Site.Sliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Interfaces
{
    public interface ISiteSettingRepository
    {
        Task<FilterSlidersViewModel> FliterSliders(FilterSlidersViewModel filter);
        Task AddSlider(Slider slider);
        Task<Slider> GetSliderById(long sliderId);
        void UpdateSlider(Slider slider);

        Task SaveChanges();
    }
}
