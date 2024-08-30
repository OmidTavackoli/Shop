using Shop.Domain.ViewModels.Site.Sliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Interfaces
{
    public interface ISiteSettingService
    {
        #region slider
        Task<FilterSlidersViewModel> FilterSliders(FilterSlidersViewModel filterSlidersViewModel);
        Task<CreateSliderResult> CreateSlider(CreateSliderViewModel createSlider);
        Task<EditSliderViewModel> GetEditSlider(long sliderId);
        Task<EditSliderResult> EditSlider(EditSliderViewModel editSlider);
        #endregion 
    }
}
