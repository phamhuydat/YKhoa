using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Entities;
using Data.Repositories;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.ViewModels.GroupVM;
using Web.ViewModels.ClientExamVM;
using Web.ViewModels.ClientGroupVM;
using Web.WebConfig;

namespace Web.Controllers
{
    public class GroupUserController : BaseController
    {

        public GroupUserController(GenericRepository _repo, IMapper mapper) : base(_repo, mapper)
        {

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoadListGroup()
        {
            // get all group of user in groupdetails

            var listGroup = _repo.GetAll<Group>(
                       x => x.DeletedDate == null && x.GroupDetails.Any(gd => gd.UserId == this.CurrentUserId))
                   .Include(x => x.subject)
                   .Include(x => x.GroupDetails)
                   .ProjectTo<ListGroupClientVM>(AutoMapperProfile.GroupIndexClientConf) // Pass any parameters if needed
                   .ToList();

            return Ok(listGroup);
        }

        public async Task<IActionResult> JoinGroup([FromBody] string code)
        {
            var group = await _repo.GetOneAsync<Group>(x => x.InvitationCode == code);
            var check = await _repo.GetOneAsync<GroupDetails>(x => x.GroupId == group.Id && x.UserId == this.CurrentUserId);
            if (group == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Mã lớp không tồn tại"
                });
            }
            if (check != null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Bạn đã tham gia lớp này"
                });
            }

            var user = await _repo.FindAsync<Users>(CurrentUserId);
            if (user == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "User đăng nhập thất bại"
                });
            }

            var groupUser = new GroupDetails
            {
                GroupId = group.Id,
                UserId = user.Id,
                IsBlock = false
            };

            await _repo.AddAsync(groupUser);
            return Ok(new
            {
                success = true,
                message = "Tham gia lớp học thành công"
            });
        }

        public async Task<IActionResult> LeaveGroup(int id)
        {
            var groupUser = _repo.GetOneAsync<GroupDetails>(x => x.GroupId == id && x.UserId == this.CurrentUserId);

            if (groupUser == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Bạn không tham gia lớp này"
                });
            }

            await _repo.DeleteAsync<GroupDetails>(groupUser.Id);
            return Ok(new
            {
                success = true,
                message = "Rời khỏi lớp học thành công"
            });
        }

        public IActionResult LoadListExam(int id)
        {
            try
            {
                //var listExam = _repo.GetAll<Exam>(x => x.Id == id)
                //    .Include(x => x.Subject)
                //    .ProjectTo<ListExamUserVM>(AutoMapperProfile.ExamIndexClientConf)
                //    .ToList();

                var listExam = _repo.GetAll<Exam>(x => x.DeletedDate == null
                        && x.handOutExams.Any(h => h.GroupId == id))
                        .ProjectTo<ListExamUserVM>(AutoMapperProfile.ExamIndexClientConf)
                .ToList();

                return Ok(listExam);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    success = false,
                    message = e.Message
                });
            }
        }
        public IActionResult LoadNoty(int id)
        {
            var listNoty = _repo.GetAll<Notification>(x =>
                x.NotificationDetailsDetails.Any(n => n.GroupId == id))
                .ToList();

            return Ok(listNoty);
        }

        public IActionResult LoadListUser(int id)
        {
            var listUser = _repo.GetAll<GroupDetails>(x => x.GroupId == id && x.UserId != this.CurrentUserId)
                .Include(x => x.User)
                .ProjectTo<ListUserInGroup>(AutoMapperProfile.GroupDetailIndexClientConf)
                .ToList();

            return Ok(listUser);
        }

    }
}
