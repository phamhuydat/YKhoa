using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Entities;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Share.Consts;
using Web.Areas.Admin.ViewModels.ChapterVM;
using Web.Areas.Admin.ViewModels.SubjectVM;
using Web.Common;
using Web.WebConfig;

namespace Web.Areas.Admin.Controllers
{
    public class SubjectController : AdminBaseController
    {
        public SubjectController(GenericRepository repo, IMapper mapper) : base(repo, mapper)
        {
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [Route("/Admin/Subject/ListItem")]
        public async Task<IActionResult> GetList()
        {
            var data = await _repo.GetAll<Subject>()
                    .ProjectTo<ListSubjectVM>(AutoMapperProfile.SubjectIndexConf).ToListAsync();
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetSubject(int id)
        {
            var data = await _repo.GetOneAsync<Subject>(X => X.Id == id);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetListChapter(int id)
        {
            var data = await _repo.GetAll<Chapter>(x => x.SubjectId == id)
                .ProjectTo<ListChapterVM>(AutoMapperProfile.ChapterIndexConf)
                .OrderBy(x => x.Id)
                .ToListAsync();
            return Ok(data);
        }

        [HttpPost]
        [AppAuthorize(AuthConst.AppSubject.CREATE)]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectAddOrUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "DỮ liệu không hợp lệ",
                    data = model
                });
            }
            if (await _repo.GetOneAsync<Subject>(X => X.SubjectCode == model.SubjectCode) != null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Mã môn học này đã tồn tại",
                    data = model
                });
            }

            var subject = _mapper.Map<Subject>(model);
            subject.CreatedBy = this.CurrentUserId;
            subject.CreatedDate = DateTime.Now;
            subject.UpdatedBy = this.CurrentUserId;
            subject.UpdatedDate = DateTime.Now;
            await _repo.AddAsync(subject);

            return Ok(new
            {
                success = true,
                message = "Thêm dữ liệu thành công",
                data = model
            });
        }

        [HttpPost]
        [AppAuthorize(AuthConst.AppSubject.UPDATE)]
        public async Task<IActionResult> EditSubject(int id, [FromBody] SubjectAddOrUpdateVM model)
        {

            var subject = await _repo.GetOneAsync<Subject>(X => X.Id == id);
            if (subject == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không tìm thấy môn học",
                    data = id
                });
            }
            subject = _mapper.Map(model, subject);
            await _repo.UpdateAsync(subject);

            return Ok(new
            {
                success = true,
                message = "Cập nhật thông tin môn học thành công",
                data = id
            });
        }

        [HttpPost]
        [AppAuthorize(AuthConst.AppSubject.DELETE)]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _repo.FindAsync<Subject>(id);
            if (subject == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không tìm thấy môn học",
                });
            }

            await _repo.DeleteAsync(subject);
            return Ok(new
            {
                success = true,
                message = "Xóa thành công",
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateChapter([FromBody] ChapterAddOrEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ",
                    data = model
                });
            }

            var data = await _repo.GetOneAsync<Chapter>(X => X.ChapterName == model.ChapterName);

            var chapter = _mapper.Map<Chapter>(model);
            chapter.CreatedBy = this.CurrentUserId;
            chapter.CreatedDate = DateTime.Now;

            await _repo.AddAsync(chapter);

            return Ok(new
            {
                success = true,
                message = "Thêm chương môn học thành công",
            });
        }


        [HttpPost]
        public async Task<IActionResult> EditChapter(int id, [FromBody] ChapterAddOrEditVM model)
        {
            var chapter = await _repo.GetOneAsync<Chapter>(X => X.Id == id);
            if (chapter == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không tìm thấy chương",
                    data = id
                });
            }
            chapter = _mapper.Map(model, chapter);

            chapter.UpdatedBy = this.CurrentUserId;
            chapter.UpdatedDate = DateTime.Now;

            await _repo.UpdateAsync(chapter);
            return Ok(new
            {
                success = true,
                message = "Cập nhật thông tin chương thành công",
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetChapter(int id)
        {
            var data = await _repo.GetOneAsync<Chapter>(x => x.Id == id);
            var result = _mapper.Map<ChapterAddOrEditVM>(data);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            var chapter = await _repo.FindAsync<Chapter>(id);
            if (chapter == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không tìm thấy chương",
                });
            }

            await _repo.DeleteAsync(chapter);
            return Ok(new
            {
                success = true,
                message = "Xóa thành công",
            });
        }
    }
}
