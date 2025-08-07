using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Data.Entities;
using Data.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Common.Helpers;
using Web.ViewModels.Account;
using Web.WebConfig;

namespace Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        public readonly INotyfService _notyf;
        public AccountController(GenericRepository repository, IMapper mapper, INotyfService noty) : base(repository, mapper)
        {
            _notyf = noty;
        }
        [HttpGet]
        public ActionResult Login() => User.Identity.IsAuthenticated ? RedirectToAction(nameof(Index)) : View();


        [HttpPost]
        public async Task<IActionResult> Login(LoginClientVM model)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Vui lòng điền đủ thông tin");
                return View(model);
            }
            var user = await _repo.GetOneAsync<Users, UserDataForApp>
                (
                    where: x => x.MSSV == model.MSSV.ToString(),
                    AutoMapperProfile.LoginConf
                );
            if (user == null)
            {
                _notyf.Error("Tên đăng nhập hoặc mật khẩu không chính xác!");
                return View(model);
            }
            if (user.BlockedTo.HasValue && user.BlockedTo.Value >= DateTime.Now)
            {
                _notyf.Error($"Tài khoản của bạn bị khóa đến {user.BlockedTo.Value:dd/MM/yyyy HH:mm}");
                return View(model);
            }
            if (user.BlockedTo.HasValue && user.BlockedTo.Value <= DateTime.Now)
            {
                var data = await _repo.FindAsync<Users>(user.Id);
                data.BlockedTo = null;
                data.BlockedBy = null;
                await _repo.UpdateAsync(data);
            }


            if (BCrypt.Net.BCrypt.Verify(model.Password, user.Password) == false)
            {
                _notyf.Error("Tên đăng nhập hoặc mật khẩu không chính xác!");
                return View(model);
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

            string roleName = user.RoleName.ToUpper();

            if (roleName == "ADMIN" || roleName == "TEACHER")
            {
                // Trả về đường dẫn cho admin
                var returnUrl = Request.Query["ReturnUrl"].ToString();
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect("/Admin/Home/Index"); // Thay thế bằng phương thức trang chủ admin của bạn
                }
                return Redirect(returnUrl);
            }
            else
            {
                // Trả về trang chủ cho client
                return RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpGet]
        public ActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UpdateProfileVM model)
        {

            if (!ModelState.IsValid)
            {
                _notyf.Error("Vui lòng điền đủ thông tin");
                return View(model);
            }
            var user = await _repo.FindAsync<Users>(CurrentUserId);
            if (user == null)
            {
                _notyf.Error("Không tìm thấy thông tin người dùng");
                return View(model);
            }


            user.FullName = model.fullname;
            user.Email = model.email;

            await _repo.UpdateAsync(user);
            _notyf.Success("Cập nhật thông tin thành công");
            SetSuccessMesg("Cập nhật thông tin thành công");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(AppConst.COOKIES_AUTH);
            _notyf.Information("bạn đã đăng xuất");
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> ChangePassword(ChangePwdVM model)
        {
            var user = await _repo.FindAsync<Users>(CurrentUserId);
            if (user == null)
            {
                _notyf.Error("Không tìm thấy thông tin người dùng");
                return View(model);
            }
            if (BCrypt.Net.BCrypt.Verify(model.Pwd, user.Password) == false)
            {
                _notyf.Error("Mật khẩu cũ không chính xác");
                return View(model);
            }
            if (model.NewPwd != model.ConfirmPassword)
            {
                _notyf.Error("Mật khẩu mới không khớp");
                return View(model);
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPwd);
            await _repo.UpdateAsync(user);
            _notyf.Success("Đổi mật khẩu thành công");
            return View(model);

        }




        private static void CreateDirIfNotExist(string username)
        {
            var userPath = $"{AppConst.SYSTEM_FILE_PATH}{Path.DirectorySeparatorChar}{username}";
            var fullPath = PathHelper.MapPath(userPath);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                // Thêm file tạm để giữ folder
                var file = PathHelper.MapPath($"{userPath}{Path.DirectorySeparatorChar}{username}.txt");
                System.IO.File.WriteAllText(file, $"Hello {username}!");
            }
        }
    }

}
