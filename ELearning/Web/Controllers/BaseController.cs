using AutoMapper;
using Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.WebConfig;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = AppConst.COOKIES_AUTH)]
    public class BaseController : Controller
    {
        protected readonly IMapper _mapper;
        protected readonly GenericRepository _repo;
        //private readonly ILog _logger;
        protected int CurrentUserId { get => Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)); }
        protected string CurrentUserName { get => HttpContext.User.FindFirstValue(ClaimTypes.Name); }
        protected readonly string ADMIN = "admin";
        protected readonly string TEACHER = "teacher";
        protected readonly string STUDENT = "student";

        public BaseController(GenericRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }
        protected void SetErrorMesg(string mesg, bool modelStateIsInvalid = false)
        {
            TempData["Err"] = mesg;
            if (modelStateIsInvalid)
            {
                // hiển thị tin nhắn lỗi ở file log
                var invalidMesg = string.Join("\n", ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage));
                //_logger.Error($"Model state is invalid: {invalidMesg}");
            }
        }
        protected void SetSuccessMesg(string mesg) => TempData["Messenger"] = mesg;

    }
}
