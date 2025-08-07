using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Entities;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Share.Consts;
using Web.Areas.Admin.ViewModels.GroupDetailVM;
using Web.Areas.Admin.ViewModels.GroupVM;
using Web.Areas.Admin.ViewModels.user;
using Web.Common;
using Web.WebConfig;
using X.PagedList;

namespace Web.Areas.Admin.Controllers
{

    public class GroupController : AdminBaseController
    {
        private const int LENGTH_INVITED_CODE = 7;
        public GroupController(GenericRepository repo, IMapper mapper) : base(repo, mapper)
        {
        }

        [AppAuthorize(AuthConst.AppGroup.VIEW_DETAIL)]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ListGroup()
        {
            if (this.RoleId == ROLE_ADMIN_ID)
            {
                var listGroup = await _repo.GetAll<Group>(x => x.DeletedDate == null)
                    .Include(x => x.subject)
                    .Include(x => x.GroupDetails)
                    .ProjectTo<ListGroupVM>(AutoMapperProfile.GroupIndexConf)
                    .ToListAsync();
                var groupedListGroup = listGroup
                                    .GroupBy(item => item.Title) // Nhóm theo Title
                                    .Select(group => new
                                    {
                                        Title = group.Key,
                                        ListItemGroup = group.SelectMany(item => item.ListItemGroup).ToList(),
                                        Id = group.First().Id,
                                        RowIndex = group.First().RowIndex
                                    })
                                    .ToList();

                return Ok(groupedListGroup);
            }
            else
            {
                var listGroupByTeacher = _repo.GetAll<Group>(
                        x => x.DeletedDate == null && x.GroupDetails.Any(gd => gd.UserId == this.CurrentUserId))
                    .Include(x => x.subject) // Ensure that the Subject navigation property is included
                    .Include(x => x.GroupDetails) // Include GroupDetails to access it in the mapping
                    .ProjectTo<ListGroupVM>(AutoMapperProfile.GroupIndexConf) // Pass any parameters if needed
                    .ToList();
                var groupedListGroupTeacher = listGroupByTeacher
                        .GroupBy(item => item.Title) // Nhóm theo Title
                        .Select(group => new
                        {
                            Title = group.Key,
                            ListItemGroup = group.SelectMany(item => item.ListItemGroup).ToList(),
                            Id = group.First().Id,
                            RowIndex = group.First().RowIndex
                        })
                        .ToList();
                return Ok(groupedListGroupTeacher);
            }

        }

        [HttpPost]
        [AppAuthorize(AuthConst.AppGroup.CREATE)]
        public async Task<IActionResult> CreateGroup([FromBody] GroupAddOrEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "dữ liệu không hợp lệ"
                });
            }

            if (this.RoleId == ROLE_TEACHER_ID)
            {
                var group1 = _mapper.Map<Group>(model);
                group1.Status = true;
                group1.Teacher = this.CurrentUsername;
                group1.InvitationCode = GenerateRandomCode();
                group1.CreatedBy = this.CurrentUserId;
                group1.CreatedDate = DateTime.Now;

                await _repo.AddAsync(group1);
                var groupDetail = new GroupDetails
                {
                    GroupId = group1.Id,
                    UserId = this.CurrentUserId
                };
                await _repo.AddAsync(groupDetail);
                return Ok(new
                {
                    success = true,
                    message = "Thêm nhóm thành công"
                });
            }

            var group = _mapper.Map<Group>(model);
            group.Status = true;
            group.InvitationCode = GenerateRandomCode();
            group.CreatedBy = this.CurrentUserId;
            group.CreatedDate = DateTime.Now;

            await _repo.AddAsync(group);
            return Ok(new
            {
                success = true,
                message = "Thêm nhóm thành công"
            });

        }

        [HttpPost]
        [AppAuthorize(AuthConst.AppGroup.UPDATE)]
        public async Task<IActionResult> EditGroup(int id, [FromBody] GroupAddOrEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ"
                });
            }

            var group = await _repo.GetOneAsync<Group>(x => x.Id == id);
            if (group == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không tìm thấy nhóm"
                });
            }

            _mapper.Map(model, group);
            group.UpdatedBy = this.CurrentUserId;
            group.UpdatedDate = DateTime.Now;

            await _repo.UpdateAsync(group);
            return Ok(new
            {
                success = true,
                message = "Cập nhật nhóm thành công"
            });
        }

        [HttpPost]
        [AppAuthorize(AuthConst.AppGroup.DELETE)]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            await _repo.DeleteAsync<Group>(id);
            return Ok(new
            {
                success = true,
                message = "Xóa nhóm thành công"
            });
        }

        [HttpGet]
        [AppAuthorize(AuthConst.AppGroup.UPDATE)]
        public async Task<IActionResult> GetGroup(int id)
        {
            var data = await _repo.GetOneAsync<Group>(x => x.Id == id);
            return Ok(data);
        }

        [HttpGet]
        public IActionResult GetViewUser(int id)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListUser(int id)
        {

            var listUser = await _repo.GetAll<GroupDetails>(x => x.GroupId == id)
                .Include(x => x.User)
                .Include(x => x.Group)
                .ProjectTo<ListUserGroupVM>(AutoMapperProfile.GroupDetailIndexConf)
                .ToListAsync();
            return Ok(listUser);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToGroup(string mssv, [FromBody] GroupDetailAddOrEditVM model)
        {
            var data = await _repo.GetOneAsync<Users>(x => x.MSSV == mssv);

            if (data == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Mã sinh viên không tồn tại",
                    data = model
                });
            }
            model.UserId = data.Id;

            var groupDetail = new GroupDetails
            {
                GroupId = model.GroupId,
                UserId = model.UserId,
            };
            await _repo.AddAsync(groupDetail);
            return Ok(new
            {
                success = true,
                message = "Thêm sinh viên thành công",
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetInvitationCode(int id)
        {
            var data = await _repo.GetOneAsync<Group>(x => x.Id == id);
            return Ok(new
            {
                success = true,
                data = data.InvitationCode
            });
        }

        public async Task<IActionResult> UpdateInvitedCode(int id)
        {
            var group = await _repo.GetOneAsync<Group>(x => x.Id == id);
            if (group == null)
            {
                return BadRequest(new
                {
                    success = true,
                    message = "lỗi nhóm",
                });
            }

            group.InvitationCode = GenerateRandomCode();
            group.UpdatedBy = this.CurrentUserId;
            group.UpdatedDate = DateTime.Now;

            await _repo.UpdateAsync(group);
            return Ok(new
            {
                success = true,
                message = "Cập nhật mã mời thành công",
                data = group.InvitationCode
            });
        }

        [HttpPost]
        public async Task<IActionResult> BlockUser(BlockUserVM data)
        {
            try
            {
                var user = await _repo.FindAsync<Users>(data.Id);
                user.BlockedBy = CurrentUserId;
                if (data.Permanentblock)
                {
                    var date = DateTime.Now;
                    var blockTime = date.AddYears(100);
                    user.BlockedTo = blockTime;
                }
                else
                {
                    user.BlockedTo = data.BlockedTo;
                }
                //SetSuccessMesg($"Khóa tài khoản [{user.Username}] thành công!");
                await _repo.UpdateAsync<Users>(user);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                SetErrorMesg($"Có lỗi trong quá trình xử lý!");
                return RedirectToAction(nameof(Index));
            }

        }

        private string GenerateRandomCode(int length = LENGTH_INVITED_CODE)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
