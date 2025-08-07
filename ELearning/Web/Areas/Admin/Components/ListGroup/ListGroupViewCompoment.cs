using Data.Entities;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Components.ListGroup
{
    public class ListGroupViewCompoment : ViewComponent
    {
        readonly GenericRepository repository;
        public async Task<IViewComponentResult> InvokeAsync(int? seletetedId = null)
        {

            var data = await repository
                    .GetAll<Group>()
                    .ToListAsync();

            ViewBag.SelectedId = seletetedId;
            return View(data);
        }

    }
}
