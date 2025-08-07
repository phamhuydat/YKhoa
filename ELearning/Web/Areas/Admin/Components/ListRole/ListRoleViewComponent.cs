using Data.Entities;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Components.ListRole
{
    public class ListRoleViewComponent : ViewComponent
    {
        readonly GenericRepository repository;
        public ListRoleViewComponent(GenericRepository _db)
        {
            repository = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? seletetedId = null)
        {
            var data = await repository
                    .GetAll<Role>()
                    .ToListAsync();

            ViewBag.SelectedId = seletetedId;
            return View(data);
        }
    }
}
