using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using Data.Entities;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Share.Consts;
using Web.Areas.Admin.ViewModels.ChapterVM;
using Web.Areas.Admin.ViewModels.ExamVM;
using Web.Areas.Admin.ViewModels.QuestionVM;
using Web.Areas.Admin.ViewModels.SubjectVM;
using Web.Common;
using Web.WebConfig;
using X.PagedList;

namespace Web.Areas.Admin.Controllers
{
	public class ExamController : AdminBaseController
	{
		protected readonly DataContext _db;
		public ExamController(DataContext db, GenericRepository repo, IMapper mapper) : base(repo, mapper)
		{
			_db = db;
		}

		// action view list exam
		[AppAuthorize(AuthConst.AppExam.VIEW_DETAIL)]
		public IActionResult Index(int page = 1, int size = 20)
		{
			var data = _repo.GetAll<Exam>(x => x.CreatedBy == this.CurrentUserId)
						.ProjectTo<ListExamVM>(AutoMapperProfile.ExamIndexConf)
				.ToPagedList(page, size);
			return View(data);
		}

		// action view detail exam
		[AppAuthorize(AuthConst.AppExam.VIEW_DETAIL)]
		public IActionResult Detail(int id)
		{
			var data = _repo.FindAsync<Exam>(id).Result;
			return View(data);
		}

		// api list subject
		[HttpGet]
		public IActionResult GetSubject()
		{
			var model = new List<Subject>();

			var data = new List<ListSubjectVM>();

			if (this.RoleId == ROLE_ADMIN_ID)
			{
				data = _repo.GetAll<Subject>()
						.ProjectTo<ListSubjectVM>(AutoMapperProfile.SubjectIndexConf).ToList();
			}
			else
			{
				// lấy ra danh sách môn học của giáo viên có trong groupdetail
				data = _repo.GetAll<GroupDetails>(x => x.UserId == CurrentUserId)
						.Join(_db.Groups,
							gd => gd.GroupId,
							g => g.Id,
							(gd, g) => new { gd, g })
						.Join(_db.Subject,
							grp => grp.g.SubjectId,
							s => s.Id,
							(grp, s) => new { grp.gd, grp.g, s })
						.Select(x => new ListSubjectVM
						{
							Id = x.s.Id,
							SubjectCode = x.s.SubjectCode,
							SubjectName = x.s.SubjectName
						})
						.ToList();
			}

			var mapData = data.Select(m => new
			{
				SubjectName = m.SubjectCode + " - " + m.SubjectName,
				SubjectCode = m.SubjectCode,
				Id = m.Id
			}).ToList();

			return Ok(mapData);
		}
		// api list chapter
		public IActionResult GetCountQuestion(int subjectId, int ChapterId, int level)
		{
			var data = _repo.GetAll<Question>()
				.Where(c => c.SubjectId == subjectId && c.Level == level)
				.Count();
			return Ok(data);
		}

		[HttpGet]
		public IActionResult GetChapter(int subjectId)
		{
			var data = _repo.GetAll<Chapter>()
				.Where(c => c.SubjectId == subjectId)
				.ProjectTo<ListChapterVM>(AutoMapperProfile.ChapterIndexConf)
				.ToList();
			return Ok(data);
		}
		// api list group
		[HttpGet]
		public IActionResult GetListGroup(int subjectId)
		{
			var data = _repo.GetAll<Group>()
				.Where(c => c.SubjectId == subjectId)
				.ToList();

			return Ok(data);
		}

		// action view create exam
		[HttpGet]
		[AppAuthorize(AuthConst.AppExam.CREATE)]
		public IActionResult CreateExam()
		{
			return View();

		}

		// action view edit exam
		[HttpGet]
		[AppAuthorize(AuthConst.AppExam.UPDATE)]
		public IActionResult EditExam(int id)
		{
			return View();
		}

		// get data update exam ro edit
		[HttpGet]
		public async Task<IActionResult> GetExam(int id)
		{
			var data = await _repo.GetOneAsync<Exam>(e => e.Id == id);

			var model = _mapper.Map<ExamAddOrEditVM>(data);

			var listGroup = _repo.GetAll<Group>()
				.Where(c => c.SubjectId == model.SubjectId)
				.ToList();
			return Ok(new { model, listGroup });
		}

		// action view add manual exam
		[HttpGet]
		[AppAuthorize(AuthConst.AppExam.CREATE)]
		public IActionResult AddManualExam(int id)
		{
			if (id == 0)
			{
				return NotFound();
			}
			var data = _repo.GetOneAsync<Exam>(e => e.Id == id).Result;
			if (data.IsAutomatic)
			{
				// trả về error http 403
				return Forbid();
			}
			return View();
		}

		// server create exam
		[HttpPost]
		[AppAuthorize(AuthConst.AppExam.CREATE)]
		public async Task<IActionResult> CreateExam([FromBody] ExamAddOrEditVM model)
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

			var exam = _mapper.Map<Exam>(model);
			exam.CreatedBy = CurrentUserId;
			exam.CreatedDate = DateTime.Now;
			exam.Status = true;

			await _repo.AddAsync(exam);
			// lấy ra dữ liệu mới được thêm vào của exam không có Id
			exam = await _repo.GetOneAsync<Exam>(e => e.Id == exam.Id);

			return Ok(new
			{
				success = true,
				message = "Tạo bài kiểm tra thành công",
				Data = exam
			});
		}

		// server get data exam
		[HttpGet]
		[AppAuthorize(AuthConst.AppExam.VIEW_DETAIL)]
		public async Task<IActionResult> GetDetailExam(int id)
		{
			try
			{
				// load list question to exam
				var exam = await _repo.GetOneAsync<Exam>(e => e.Id == id);

				var listQuestion = _repo.GetAll<Question>(x => x.SubjectId.Equals(exam.SubjectId))
					.ProjectTo<ListQuestionVM>(AutoMapperProfile.QuestionIndexConf)
					.ToList();
				var detailExam = _db.ExamDetails.Where(x => x.ExamId == id).OrderBy(x => x.DisplayOrder)
								.Join(_db.Question, ed => ed.QuestionId, q => q.Id, (ed, q) => new { q, ed })
								.Select(x => new ListQuestionVM
								{
									// Ánh xạ các thông tin từ câu hỏi
									Id = x.q.Id,
									Content = x.q.Content,
									SubjectId = x.q.SubjectId,
									ChapterId = x.q.ChapterId,
									Level = x.q.Level,
									// Ánh xạ danh sách câu trả lời (Options) cho từng câu hỏi
									Answers = _db.Answers
										.Where(a => a.QuestionId == x.q.Id && a.DeletedDate == null)
										.ToList() // Lọc các câu trả lời theo QuestionId
								}).ToList();

				return Ok(new { exam, listQuestion, detailExam });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
			}
		}

		// server add ExamDetail tạo đề thủ công
		[HttpPost]
		[AppAuthorize(AuthConst.AppExam.CREATE)]
		public async Task<IActionResult> SaveExamDetail(int id, [FromBody] List<ExamDetailsVM> model)
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

			var exam = await _repo.FindAsync<Exam>(id);

			if (exam is null)
			{
				return NotFound();
			}

			// Retrieve existing ExamDetails from the database
			var existingExamDetails = await _db.ExamDetails.Where(ed => ed.ExamId == id).ToListAsync();

			// Identify ExamDetails to delete
			var examDetailsToDelete = existingExamDetails
				.Where(ed => !model.Any(m => m.QuestionId == ed.QuestionId))
				.ToList();

			// Identify ExamDetails to add
			var examDetailsToAdd = model
				.Where(m => !existingExamDetails.Any(ed => ed.QuestionId == m.QuestionId))
				.Select(m => new ExamDetails
				{
					ExamId = id,
					QuestionId = m.QuestionId,
					DisplayOrder = m.DisplayOrder
				})
				.ToList();

			// Identify ExamDetails to update
			var examDetailsToUpdate = existingExamDetails
				.Where(ed => model.Any(m => m.QuestionId == ed.QuestionId))
				.ToList();

			// Update existing ExamDetails
			foreach (var examDetail in examDetailsToUpdate)
			{
				var updatedDetail = model.First(m => m.QuestionId == examDetail.QuestionId);
				examDetail.DisplayOrder = updatedDetail.DisplayOrder;
			}

			// Apply changes to the database
			_db.ExamDetails.RemoveRange(examDetailsToDelete);
			await _db.ExamDetails.AddRangeAsync(examDetailsToAdd);
			_db.ExamDetails.UpdateRange(examDetailsToUpdate);

			await _db.SaveChangesAsync();

			return Ok(new
			{
				success = true,
				message = "Cập nhật chi tiết bài kiểm tra thành công",
				data = model
			});
		}


		[AppAuthorize(AuthConst.AppExam.DELETE)]
		// Delete Exam    
		public async Task<IActionResult> DeleteExam(int id)
		{
			var data = _repo.GetOneAsync<Exam>(x => x.Id == id).Result;

			if (data == null)
			{
				return NotFound();
			}

			await _repo.DeleteAsync(data);
			return RedirectToAction(nameof(Index));
		}

		[AppAuthorize(AuthConst.AppExam.VIEW_DETAIL)]
		// acton view detail exam
		[HttpGet]
		public IActionResult DetailExam(int id)
		{
			var data = _repo.GetOneAsync<Exam>(x => x.Id == id).Result;

			if (data == null)
			{
				return NotFound();
			}
			return View();
		}

		// mở bài thi
		[AppAuthorize(AuthConst.AppExam.UPDATE)]
		public async Task<IActionResult> OpenExam(int id)
		{
			var exam = _repo.GetOneAsync<Exam>(x => x.Id == id).Result;
			if (exam == null)
			{
				return NotFound();
			}
			exam.TimeStart = DateTime.Now;

			await _repo.UpdateAsync(exam);

			return RedirectToAction(nameof(Index));
		}

		// thống kê bài lm của học sinh
		public async Task<IActionResult> ListStudentOfTakingExam(int id)
		{
			try
			{
				var users = await _db.GroupDetails
						.Join(_db.Groups,
							gm => gm.GroupId,
							g => g.Id,
							(gm, g) => new { gm, g })
						.Join(_db.HandOutExam,
							grp => grp.g.Id,
							ge => ge.GroupId,
							(grp, ge) => new { grp.gm, grp.g, ge })
						.Join(_db.Users,
							grpUser => grpUser.gm.UserId,
							u => u.Id,
							(grpUser, u) => new { grpUser.gm, grpUser.g, grpUser.ge, u })
						.GroupJoin(_db.Result,
							grpUserWithExam => new { grpUserWithExam.u.Id, grpUserWithExam.ge.ExamId },
							r => new { Id = r.UserId, ExamId = r.ExamId },
							(grpUserWithExam, results) => new { grpUserWithExam, results })
						.SelectMany(
							x => x.results.DefaultIfEmpty(),
							(x, result) => new ListStudentTakeExamVM
							{
								Id = x.grpUserWithExam.u.Id,
								Mssv = x.grpUserWithExam.u.MSSV,
								FullName = x.grpUserWithExam.u.FullName,
								Email = x.grpUserWithExam.u.Email,
								GroupId = x.grpUserWithExam.g.Id,
								GroupName = x.grpUserWithExam.g.GroupName,
								ExamId = x.grpUserWithExam.ge.ExamId,
								TestScores = result != null ? result.TestScores : (double?)0, // Điểm số hoặc null
								NumTSC = result != null ? result.NumTSC : 0, // Số câu đúng hoặc null
								StartTime = result != null ? result.StartTime : (DateTime?)null,
								EndTime = result != null ? result.EndTime : (DateTime?)null,
								TotalWorkingTime = result != null ? result.TotalWorkTime : 0,
							})
						.Where(x => x.ExamId == id)
						.GroupBy(x => x.Id) // Nhóm theo UserId để loại bỏ trùng lặp
						.Select(g => g.FirstOrDefault())
						.ToListAsync();

				var groups = _repo.GetAll<Group>(x => x.HandOutExams.Any(ge => ge.ExamId == id)).ToList();

				List<double> scores = ListStudentTakeExamVM.GetScores(users);
				var ranges = new List<string> { "0-1", "1-2", "2-3", "3-4", "4-5", "5-6", "6-7", "7-8", "8-9", "9-10" };

				var scoreCounts = ranges
					.Select((range, index) =>
					{
						double lowerBound = index;
						double upperBound = index + 1;

						var count = scores.Count(s => s >= lowerBound && s < upperBound);

						if (index == 9)
						{
							count += scores.Count(s => s == upperBound);
						}

						return count > 0 ? count : 0;
					})
					.ToList();

				return Ok(new { groups, users, scores = scoreCounts });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
			}
		}

		[HttpGet]
		public IActionResult ResultDetail(int userId, int examId)
		{
			var data = _repo.GetOneAsync<Result>(x => x.UserId == userId && x.ExamId == examId).Result;

			if (data == null)
			{
				return NotFound(new { message = "Result not found." });
			}

			var resultDetails = _db.ResultDetails
				.Where(x => x.ResultId == data.Id)
				.Join(_db.Question,
					rd => rd.QuestionId,
					q => q.Id,
					(rd, q) => new { rd, q })
				.Select(x => new
				{
					QuestionId = x.q.Id,
					Content = x.q.Content,
					UserIsCorrect = x.rd.AnswerId,
					Check = _db.Answers.Where(a => a.Id == x.rd.AnswerId).Single().Status,
					Answers = _db.Answers
						.Where(a => a.QuestionId == x.q.Id)
						.Select(a => new
						{
							AnswerId = a.Id,
							AnswerContent = a.AnswerContent,
							IsCorrect = ((a.Status && x.rd.AnswerId == a.Id) || a.Status) ? 1
									: (!a.Status && x.rd.AnswerId == a.Id) ? 0 : (int?)null
						})
					.ToList()
				})
			.ToList();
			return Ok(resultDetails);
		}

	}
}