using AutoMapper;
using Data.Entities;
using Data.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Areas.Admin.ViewModels.Account;
using Web.ViewModels.Account;
using Web.WebConfig;

namespace Web.Areas.Admin.Controllers
{
	[AllowAnonymous]
	public class AccountController : AdminBaseController
	{
		public AccountController(GenericRepository repo, IMapper mapper) : base(repo, mapper)
		{
		}
		public IActionResult Login() => View();

		[HttpPost]
		public async Task<IActionResult> Login(LoginVM model)
		{
			var user = await _repo.GetOneAsync<Users, UserDataForApp>
							(
								where: x => x.MSSV == model.MSSV.ToString(),
								AutoMapperProfile.LoginConf
							);
			if (user == null)
			{
				TempData["Mesg"] = "Tài khoản không tồn tại!";
				return Redirect("/Admin/Account/Login");
			}

			if (user.BlockedTo.HasValue && user.BlockedTo.Value >= DateTime.Now)
			{
				TempData["Mesg"] = $"Tài khoản của bạn bị khóa đến {user.BlockedTo.Value:dd/MM/yyyy HH:mm}";
				return Redirect("/Admin/Account/Login");
			}

			if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
			{
				TempData["Mesg"] = "Tên đăng nhập hoặc mật khẩu không chính xác!";
				return Redirect("/Admin/Account/Login");
			}

			var claims = new List<Claim> {
							new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
							new Claim(ClaimTypes.Name, user.FullName),
							new Claim(ClaimTypes.Email, user.Email),
							new Claim(AppClaimTypes.FullName, user.FullName),
							new Claim(AppClaimTypes.PhoneNumber, user.Phone),
							new Claim(AppClaimTypes.RoleName, user.RoleName),
							new Claim(AppClaimTypes.RoleId, user.AppRoleId.ToString()),
							new Claim(AppClaimTypes.Permissions, user.Permission),
						};

			var claimsIdentity = new ClaimsIdentity(claims, AppConst.COOKIES_AUTH);
			var principal = new ClaimsPrincipal(claimsIdentity);

			var authenPropeties = new AuthenticationProperties()
			{
				ExpiresUtc = DateTime.UtcNow.AddHours(AppConst.LOGIN_TIMEOUT),
				IsPersistent = model.RememberMe
			};

			await HttpContext.SignInAsync(AppConst.COOKIES_AUTH, principal, authenPropeties);

			// tạo folder khi đăng nhập
			//CreateDirIfNotExist(model.Username);
			string roleName = user.RoleName.ToUpper();

			if (roleName == "ADMIN" || roleName == "TEACHER")
			{
				// Trả về đường dẫn cho admin
				var returnUrl = Request.Query["ReturnUrl"].ToString();
				if (string.IsNullOrEmpty(returnUrl))
				{
					return AdminHomePage(); // Thay thế bằng phương thức trang chủ admin của bạn
				}
				return Redirect(returnUrl);
			}
			else
			{
				// Trả về trang chủ cho client
				return RedirectToAction(nameof(Index), "Home");
			}
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(AppConst.COOKIES_AUTH);
			return Redirect("/admin/account/login");
		}
	}
}
