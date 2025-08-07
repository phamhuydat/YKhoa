using Data.Entities;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Components.Permission
{
	public class PermissionViewComponent : ViewComponent
	{
		readonly GenericRepository _repo;
		public PermissionViewComponent(GenericRepository repo)
		{
			_repo = repo;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var data = _repo.GetAllMst<MstPermission>()
							.AsEnumerable()
							.GroupBy(x => x.GroupName).ToList();

			return View(data);
		}
	}
}
