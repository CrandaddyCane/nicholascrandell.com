
using Microsoft.AspNetCore.Mvc;
using Percy.Website.Models;

namespace Percy.Website.ViewComponents
{
    public class CardCollectionViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            CardCollectionViewModel model)
        {
            return View("Default", model);
        }
    }
}
