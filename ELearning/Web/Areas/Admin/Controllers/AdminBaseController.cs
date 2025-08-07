using AutoMapper;
using Data.Repositories;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Share.Consts;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Web.Common;
using Web.WebConfig;
using X.PagedList;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = AppConst.COOKIES_AUTH)]
    public class AdminBaseController : Controller
    {
        protected const string AREA_NAME = "Admin";
        protected const int DEFAULT_PAGE_SIZE = 15;
        protected const string EXCEPTION_ERR_MESG = "Đã xảy ra lỗi trong quá trình xử lý dữ liệu (500)";
        protected const string MODEL_STATE_INVALID_MESG = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại";
        protected const string PAGE_NOT_FOUND_MESG = "Không tìm thấy trang";
        protected readonly object ROUTE_FOR_AREA = new
        {
            area = AREA_NAME
        };
        protected const int ROLE_ADMIN_ID = 2;
        protected const int ROLE_TEACHER_ID = 3;

        protected RedirectToActionResult AdminHomePage() => RedirectToAction("Index", "Home", new { area = "Admin" });

        protected int CurrentUserId { get => Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)); }
        protected string CurrentUsername { get => HttpContext.User.Identity.Name; }
        protected int RoleId { get => Convert.ToInt32(HttpContext.User.FindFirstValue(AppClaimTypes.RoleId)); }
        protected string Referer { get => Request.Headers["Referer"].ToString(); }

        private readonly ILog _logger;

        protected readonly IMapper _mapper;
        protected readonly GenericRepository _repo;

        public AdminBaseController(GenericRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
            _logger = LogManager.GetLogger(typeof(AdminBaseController));
        }

        protected RedirectToActionResult HomePage() => RedirectToAction("Index", "Home", new { area = "Admin" });

        /// <summary>
        /// Gán thông báo lỗi để hiển thị lên view
        /// </summary>
        /// <param name="mesg">Thông báo lỗi</param>
        /// <param name="modelStateIsInvalid">Đặt là true khi lỗi validate dữ liệu</param>
        protected void SetErrorMesg(string mesg, bool modelStateIsInvalid = false)
        {
            TempData["Err"] = mesg;
            if (modelStateIsInvalid)
            {
                // hiển thị tin nhắn lỗi ở file log
                var invalidMesg = string.Join("\n", ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage));
                _logger.Error($"Model state is invalid: {invalidMesg}");
            }
        }

        protected void SetSuccessMesg(string mesg) => TempData["Success"] = mesg;

        protected void LogException(Exception ex)
        {
            _logger.Error(ex);
            SetErrorMesg(EXCEPTION_ERR_MESG);
        }
    }
}