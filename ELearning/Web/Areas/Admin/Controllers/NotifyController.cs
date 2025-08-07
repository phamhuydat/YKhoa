using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using Data.Entities;
using Data.Repositories;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using Web.Areas.Admin.ViewModels.NotifyVM;
using Web.WebConfig;
using X.PagedList;

namespace Web.Areas.Admin.Controllers
{
    public class NotifyController : AdminBaseController
    {

        public readonly DataContext _db;

        public NotifyController(GenericRepository repo, IMapper mapper, DataContext db)
            : base(repo, mapper)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 15)
        {
            var data = _repo.GetAll<Notification>()
                .Where(m => m.CreatedBy == CurrentUserId)
                .ProjectTo<ListNoifyVM>(AutoMapperProfile.NotificationIndexConf)
                .ToPagedList(page, size);

            return View(data);
        }

        [HttpGet]
        public IActionResult Create() => View();
        [HttpGet]
        public IActionResult Update() => View();

        public async Task<IActionResult> GetNotifyById(int id)
        {
            var notify = await _repo.FindAsync<Notification>(id);

            if (notify == null)
            {
                return BadRequest(new
                {
                    success = false,
                    mesg = "Không tìm thấy thông báo"
                });
            }

            return Ok(notify);
        }

        [HttpPost]
        public async Task<IActionResult> SaveNotify([FromBody] AddOrEditNotifyVM notification)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new
                {
                    success = false,
                    mesg = "Dữ liệu không hợp lệ"
                });
            }
            try
            {
                if (notification.Id == 0)
                {
                    var notify = _mapper.Map<Notification>(notification);

                    notify.CreateName = CurrentUsername;
                    notify.CreatedBy = CurrentUserId;
                    notify.CreatedDate = DateTime.Now;

                    await _repo.AddAsync(notify);

                    foreach (var item in notification.Details)
                    {
                        var notifyUser = new NotificationDetails
                        {
                            GroupId = item.GroupId ?? 0
                        };
                        await _db.AddAsync(notifyUser);
                    }
                    return Ok(new
                    {
                        success = true,
                        mesg = "Thêm thông báo thành công"
                    });
                }
                else
                {

                    var notify = await _repo.FindAsync<Notification>(notification.Id);
                    if (notify == null)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            mesg = "Không tìm thấy thông báo"
                        });
                    }

                    notify = _mapper.Map(notification, notify);

                    await _repo.UpdateAsync(notify);
                    return Ok(new
                    {
                        success = true,
                        mesg = "Cập nhật thông báo thành công"
                    });
                }

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    mesg = "Có lỗi xảy ra"
                });
            }
        }
    }
}
