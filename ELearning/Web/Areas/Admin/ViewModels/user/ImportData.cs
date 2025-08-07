using Shared.Attributes;

namespace Web.Areas.Admin.ViewModels.user
{
	public class ImportData
	{
		[AppRequired]
		public IFormFile FileExcel { get; set; }

		[AppRequired]
		public int CompanyId { get; set; }
		public bool IsLimitMaxRows { get; set; } = true;
		public bool IsAutoCreateDepartment { get; set; }

		[AppRequired]
		[AppRange(1, short.MaxValue)]
		public int BaseRow { get; set; } = 2;
	}
}
