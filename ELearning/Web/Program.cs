using DinkToPdf;
using DinkToPdf.Contracts;
using Web.WebConfig;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();



builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/error/500");
	app.UseHsts();
}

// Điều hướng khi gặp lỗi
app.UseStatusCodePagesWithReExecute("/error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
		name: "clientLogin",
		pattern: "login",  // Không cần dấu `/` ở đầu
		defaults: new
		{
			controller = "Account",
			action = "Login"
		});

app.MapAreaControllerRoute(
  areaName: "Admin",
  name: "adminLogin",
  pattern: "admin/login",  // Sử dụng đường dẫn khác cho admin
  defaults: new
  {
	  controller = "Account",
	  action = "Login",
	  area = "Admin"
  });

app.MapControllerRoute(
		name: "error",
		pattern: "error/{statusCode}",
		defaults: new
		{
			controller = "Home",
			action = "Error"
		});

app.MapAreaControllerRoute(
  areaName: "Admin",
  name: "Admin",
  pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

// Định tuyến cho các trang ngoài area (Client)
app.MapControllerRoute(
		name: "areas",
		pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
