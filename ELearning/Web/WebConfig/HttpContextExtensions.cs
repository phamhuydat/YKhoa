namespace Web.WebConfig
{
	public static class HttpContextExtensions
	{
		public static string GetAuthenticationScheme(this HttpContext context)
		{
			// Kiểm tra AuthenticationScheme dựa trên thông tin xác thực
			if (context.User.Identity.AuthenticationType == AppConst.ADMIN_COOKIES_AUTH)
			{
				return AppConst.ADMIN_COOKIES_AUTH;
			}
			return AppConst.CLIENT_COOKIES_AUTH;
		}
	}
}
