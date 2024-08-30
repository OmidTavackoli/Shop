using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Domain.ViewModels.Site.Sliders;
using System.Threading.Tasks;

namespace Shop.Web.Areas.Admin.Controllers
{
    public class SiteController : AdminBaseController
    {
        #region constractor
        private readonly ISiteSettingService _siteSettingService;
        public SiteController(ISiteSettingService siteSettingService)
        {
            _siteSettingService = siteSettingService;
        }
        #endregion

        #region sliders
        public async Task<IActionResult> FilterSlider(FilterSlidersViewModel filter)
        {
            return View(await _siteSettingService.FilterSliders(filter));
        }
        #endregion


        #region create-slider
        [HttpGet]
        public IActionResult CreateSlider()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSlider(CreateSliderViewModel createSlider)
        {
            if (ModelState.IsValid)
            {
                var result = await _siteSettingService.CreateSlider(createSlider);

                switch (result)
                {
                    case CreateSliderResult.ImageNotFound:
                        TempData[WarningMessage] = "";
                        break;
                    case CreateSliderResult.Success:
                        TempData[SuccessMessage] = "";
                        return RedirectToAction("FilterSlider");
                }
            }

            return View(createSlider);
        }
        #endregion

        #region edit-slider
        [HttpGet]
        public async Task<IActionResult> EditSlider(long sliderId)
        {
            var data = await _siteSettingService.GetEditSlider(sliderId);
            if (data == null)
                return NotFound();

            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSlider(EditSliderViewModel editSlider)
        {
            if (ModelState.IsValid)
            {
                var result = await _siteSettingService.EditSlider(editSlider);
                switch (result)
                {
                    case EditSliderResult.NotFound:
                        TempData[WarningMessage] = "";
                        break;
                    case EditSliderResult.Success:
                        TempData[SuccessMessage] = "";
                        return RedirectToAction("FilterSlider");
                }
            }

            return View(editSlider);
        }
        #endregion
    }
}
