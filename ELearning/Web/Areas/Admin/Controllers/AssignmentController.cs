using AutoMapper;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Data.Entities;
using Web.Areas.Admin.ViewModels.AssignmentVM;
using Web.WebConfig;
using DocumentFormat.OpenXml.InkML;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Controllers
{
    public class AssignmentController : AdminBaseController
    {
        public readonly DataContext context;

        public AssignmentController(GenericRepository repo, IMapper mapper) : base(repo, mapper)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetList()
        {
            var data = _repo.GetAll<Assignment, ListAssignmentVM>
                (AutoMapperProfile.GetListAssignmentConf);
            return Ok(data);
        }

        public IActionResult GetListTeacher()
        {
            var data = _repo.GetAll<Users>(x => x.AppRoleId == ROLE_TEACHER_ID).ToList();


            return Ok(data);
        }
        public IActionResult GetListSubject()
        {
            var data = _repo.GetAll<Subject>().ToList();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] AddOrEditAssignmentVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("dữ liệu không hợp lệ");
            }
            try
            {
                foreach (var subjectId in model.SubjectId)
                {
                    Random random = new Random();
                    int randomNumber = random.Next(100);
                    var assignment = new Assignment
                    {
                        Id = randomNumber,
                        SubjectId = subjectId,
                        UserId = model.UserId,
                        CreatedBy = CurrentUserId,
                        CreatedDate = DateTime.Now
                    };
                    await _repo.AddAsync(assignment);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý dữ liệu");
            }
            return Ok(new
            {
                success = true,
                message = "Thêm mới thành công"
            });
        }



        public IActionResult Create()
        {
            return View();
        }
    }
}
