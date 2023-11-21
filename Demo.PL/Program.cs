using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Context;
using Demo.DAL.Models;
using Demo.PL.MappingProfiles;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var Builder = WebApplication.CreateBuilder(args);


            #region Configure Services That Allow Dependancy Injection

            Builder.Services.AddControllersWithViews();
            Builder.Services.AddDbContext<MvcAppG01DbContext>(Options =>
            {
                Options.UseSqlServer(Builder.Configuration.GetConnectionString("DefaultConnection"));
            }); // Allow Dependancy Injection

            Builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>(); // Allow Dependancy injection class department

            //services.AddAutoMapper(M=>M.AddProfile(new EmployeeProfile()));
            //services.AddAutoMapper(M=>M.AddProfile(new UserProfile()));
            // You Can Use Theses Two automapper or the below one
            Builder.Services.AddAutoMapper(M => M.AddProfiles(new List<Profile>() { new EmployeeProfile(), new UserProfile(), new RoleProfile() }));

            Builder.Services.AddScoped<iUnitOfWork, UnitOfWork>();

            Builder.Services.AddIdentity<ApplicationUser, IdentityRole>(Options =>
            {
                Options.Password.RequireNonAlphanumeric = true;
                Options.Password.RequireDigit = true;
                Options.Password.RequireLowercase = true;
                Options.Password.RequireUppercase = true;
            })
                     .AddEntityFrameworkStores<MvcAppG01DbContext>()
                     .AddDefaultTokenProviders();


            //Builder.Services.AddScoped<UserManager<ApplicationUser>>();
            Builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                     .AddCookie(Options =>
                     {
                         Options.LoginPath = ("Account/Login");
                         Options.AccessDeniedPath = "Home/Error";
                     });
            #endregion

            var app = Builder.Build();
            #region Configure Http Requrest PipeLines

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });
            #endregion

            app.Run();
        }


    }

   
    
}
