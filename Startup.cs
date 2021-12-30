using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using URLShortener.Data;
using URLShortener.Services.URLShortenerService;

namespace URLShortener
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("DefaultConnection")));

			services.AddDatabaseDeveloperPageExceptionFilter();

			services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services.Add(new ServiceDescriptor(typeof(IURLShortenerService), typeof(URLShortenerService), ServiceLifetime.Transient));

			services.AddAuthentication(options => {})
				.AddCookie(options =>
				{
					options.LoginPath = "/Identity/Account/Login";
					options.LogoutPath = "/Identity/Account/Logout";
				})
				.AddGitHub(options =>
				{
					options.ClientId = Configuration["GitHub:ClientId"];
					options.ClientSecret = Configuration["GitHub:ClientSecret"];
					options.CallbackPath = "/signin-github";
					options.Scope.Add("user:email");
					options.SignInScheme = IdentityConstants.ExternalScheme;
				});

			services.AddControllersWithViews();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "redirect",
					pattern: "{shortURL}",
					defaults: new { controller = "URL", action = "ShortRedirect" }
				);
				
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}"
				);

				endpoints.MapControllerRoute(
					name: "shorten",
					pattern: "{controller=URL}/{action=Shorten}/{URL?}"
				);		

				endpoints.MapRazorPages();
			});
		}
	}
}
