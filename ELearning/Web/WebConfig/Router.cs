namespace Web.WebConfig
{
    public static class Router
    {
        public static void MapAppRouter(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                    name: "login",
                    pattern: "/login",
                    defaults: new
                    {
                        controller = "Account",
                        action = "Login"
                    });

            endpoints.MapAreaControllerRoute(
                areaName: "Admin",
                name: "/adminLogin",
                pattern: "/Admin/Login",
                defaults: new
                {
                    controller = "Account",
                    action = "Login",
                    area = "Admin"
                });

            // Đường dẫn cho trang lỗi
            endpoints.MapControllerRoute(
                    name: "error",
                    pattern: "/error/{statusCode}",
                    defaults: new
                    {
                        controller = "Home",
                        action = "Error"
                    });

            endpoints.MapAreaControllerRoute(
                areaName: "Admin",
                name: "Admin",
                pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
                );

            // Mặc định
            endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "/{controller=Home}/{action=Index}/{id?}");
        }

    }
}
