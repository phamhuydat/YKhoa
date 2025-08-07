using Web.Areas.Admin.Controllers;
using AutoMapper;
using Data.Entities;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Share.Consts;
using Share.Extensions;
using Web.Areas.Admin.ViewModels.Role;
using Web.Common;
using Web.WebConfig;
using X.PagedList;
using static Share.Consts.DB;

namespace Web.Areas.Admin.Controllers
{
    public class RoleController : AdminBaseController
    {
        public RoleController(GenericRepository repo, IMapper mapper) : base(repo, mapper) { }

        [AppAuthorize(AuthConst.AppRole.VIEW_LIST)]
        public async Task<IActionResult> Index(int page = 1, int size = DEFAULT_PAGE_SIZE)
        {
            var data = (await _repo.GetAll<Role, RoleListItemVM>(AutoMapperProfile.RoleIndexConf)
                .ToPagedListAsync(page, size));
            return View(data);
        }

        [AppAuthorize(AuthConst.AppRole.CREATE)]
        public IActionResult Create() => View();

        [HttpPost]
        [AppAuthorize(AuthConst.AppRole.CREATE)]
        public async Task<IActionResult> Create(RoleAddVM model)
        {
            if (model.PermissionIds == null)
            {
                SetErrorMesg(MODEL_STATE_INVALID_MESG);
                return View(model);
            }
            var arrIdPermission = model.PermissionIds.Split(',');

            var role = new Role
            {
                Name = model.Name,
                Desc = model.Desc
            };
            try
            {
                await _repo.AddAsync(role);
                foreach (var item in arrIdPermission)
                {
                    var idPer = Convert.ToInt32(item);
                    role.RolePermissions.Add(new RolePermission
                    {
                        MstPermissionId = idPer
                    });
                }
                await _repo.AddAsync(role.RolePermissions);
                SetSuccessMesg($"Thêm vai trò [{role.Name}] thành công");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                LogException(ex);
                return View();
            }
        }

        [AppAuthorize(AuthConst.AppRole.UPDATE)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                SetErrorMesg(PAGE_NOT_FOUND_MESG);
                return RedirectToAction(nameof(Index));
            }
            var data = await _repo.GetOneAsync<Role, RoleEditVM>(id.Value, r => new RoleEditVM
            {
                Id = r.Id,
                Name = r.Name,
                Desc = r.Desc,
                PermissionIds = string.Join(',', r.RolePermissions.Select(rp => rp.MstPermissionId)),
            });
            if (data == null)
            {
                SetErrorMesg(PAGE_NOT_FOUND_MESG);
                return RedirectToAction(nameof(Index));
            }
            return View(data);
        }

        [HttpPost]
        [AppAuthorize(AuthConst.AppRole.UPDATE)]
        public async Task<IActionResult> Edit(RoleEditVM model)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMesg(MODEL_STATE_INVALID_MESG, true);
                return RedirectToAction(nameof(Index));
            }
            var role = await _repo.FindAsync<Role>(model.Id);
            var curPermisssionIds = _repo
                                .GetAll<RolePermission>(where: s => s.RoleId == role.Id)
                                .ToList();
            if (role == null)
            {
                SetErrorMesg(PAGE_NOT_FOUND_MESG);
                return RedirectToAction(nameof(Index));
            }

            // danh sách permission bị xóa khỏi role
            var deletedPermissionIds = model.DeletedPermissionIds.IsNullOrEmpty() ? null : model.DeletedPermissionIds.Split(',').Select(i => Convert.ToInt32(i));
            // danh sách permission được thêm vào role
            var addedPermissionIds = model.AddedPermissionIds.IsNullOrEmpty() ? null : model.AddedPermissionIds.Split(',').Select(i => Convert.ToInt32(i)).OrderBy(i => i);
            // danh sách permission hiện tại
            var rolePermissionIds = curPermisssionIds
                                .Where(x => deletedPermissionIds != null && deletedPermissionIds.Contains(x.MstPermissionId))
                                .Select(x => x.Id)
                                .OrderBy(x => x);
            // nếu xóa hết permission mà không thêm mới thì không cho thêm
            if ((addedPermissionIds == null || !addedPermissionIds.Any()) && deletedPermissionIds != null && rolePermissionIds.SequenceEqual(deletedPermissionIds))
            {
                SetErrorMesg(MODEL_STATE_INVALID_MESG);
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }

            if (deletedPermissionIds != null && deletedPermissionIds.Any())
            {
                await _repo.HardDeleteAsync<RolePermission>(rolePermissionIds);
            }

            if (addedPermissionIds != null && addedPermissionIds.Any())
            {
                var addedRolePermisson = new List<RolePermission>();
                foreach (var item in addedPermissionIds)
                {
                    addedRolePermisson.Add(new RolePermission
                    {
                        RoleId = role.Id,
                        MstPermissionId = item
                    });
                }
                await _repo.AddAsync(addedRolePermisson);
            }
            role.Name = model.Name;
            role.Desc = model.Desc;
            await _repo.UpdateAsync(role);
            SetSuccessMesg($"Cập nhật vai trò [{role.Name}] thành công");
            return RedirectToAction(nameof(Index));
        }

        [AppAuthorize(AuthConst.AppRole.DELETE)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                SetErrorMesg(PAGE_NOT_FOUND_MESG);
                return RedirectToAction(nameof(Index));
            }

            var data = await _repo.FindAsync<Role, RoleDeleteVM>(id.Value, AutoMapperProfile.RoleDeleteConf);

            if (data == null)
            {
                SetErrorMesg(PAGE_NOT_FOUND_MESG);
                return RedirectToAction(nameof(Index));
            }
            // Xóa không cần xác nhận nếu không có dữ liệu user liên quan
            if (data.AppUsers == null || data.AppUsers.Count == 0)
            {
                await _repo.DeleteAsync<Role>(data.Id);
                SetSuccessMesg($"Xóa vai trò [{data.Name}] thành công");
                return RedirectToAction(nameof(Index));
            }

            var userDeletedCount = data.AppUsers.Where(u => u.DeletedDate != null).Count();
            if (userDeletedCount == data.AppUsers.Count)
            {
                await _repo.DeleteAsync<Role>(data.Id);
                var users = await _repo.GetAll<Users>(where: u => u.AppRoleId == data.Id).ToListAsync();
                // Cập nhật vai trò mới
                users.ForEach(u => u.AppRoleId = null);
                await _repo.UpdateAsync(users);
                SetSuccessMesg($"Xóa vai trò [{data.Name}] thành công");
                return RedirectToAction(nameof(Index));
            }
            // Chỉ hiển thị user chưa bị xóa
            data.AppUsers = data.AppUsers.Where(u => u.DeletedDate == null).ToList();
            return View(data);
        }

        [HttpPost]
        [AppAuthorize(AuthConst.AppRole.DELETE)]
        public async Task<IActionResult> Delete(RoleDeleteVM data)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMesg(MODEL_STATE_INVALID_MESG, true);
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var users = await _repo.GetAll<Users>(where: u => u.AppRoleId == data.Id).ToListAsync();
                // Cập nhật vai trò mới
                users.ForEach(u => u.AppRoleId = data.NewId);

                await _repo.BeginTransactionAsync();

                // Cập nhật role mới cho users
                await _repo.UpdateAsync(users);
                // Xóa role cũ
                await _repo.DeleteAsync<Users>(data.Id);
                await _repo.CommitTransactionAsync();

                SetSuccessMesg($"Xóa vai trò [{data.Name}] thành công");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Rollback
                await _repo.RollbackTransactionAsync();

                SetErrorMesg(EXCEPTION_ERR_MESG);
                LogException(ex);
                return RedirectToAction(nameof(Delete), new { id = data.Id });
            }
        }
    }
}
