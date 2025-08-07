using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Web.Components.MainNavBarClient
{
	public class MainNavBarClientViewComponent : ViewComponent
	{
		readonly GenericRepository _repo;
		public MainNavBarClientViewComponent(GenericRepository repo)
		{
			_repo = repo;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var navBar = new NavBarViewClientModel();
			navBar.Items.AddRange(new MenuItem[]
			{
				new MenuItem
				{
					Action = "Index",
					Controller = "Home",
					DisplayText = "Trang chủ",
					Icon = "home"
				},
				new MenuItem
				{
					Action = "Index",
					Controller = "GroupUser",
					DisplayText = "Nhóm học",
					Icon = "layer-group"
				},
				//new MenuItem
				//{
				//	Action = "Index",
				//	Controller = "Test",
				//	DisplayText = "Bài thi",
				//	Icon = "file-alt"
				//},
				new MenuItem
				{
					Action = "Index",
					Controller = "YKhoa",
					DisplayText = "Khám lâm sàng",
					Icon = "file-alt"
				}
			});
			return View(navBar);
		}
	}
}
