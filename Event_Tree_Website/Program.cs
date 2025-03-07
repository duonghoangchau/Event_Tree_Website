﻿using Event_Tree_Website.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "Event_Tree_Cookie";
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Home/Index";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
})
.AddGoogle(options =>
{
    IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleAuthNSection["ClientId"];
    options.ClientSecret = googleAuthNSection["ClientSecret"];
    options.CallbackPath = "/signin-google";
})
.AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = "1589403635192838";
    facebookOptions.AppSecret = "22800fe6807d73c663ff15297aa75609";
    facebookOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
    facebookOptions.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("2"));
    options.AddPolicy("RequireManagerRole", policy => policy.RequireRole("1"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("0"));
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var connectionString = builder.Configuration.GetConnectionString("Event_TreeConnection");
builder.Services.AddDbContext<Event_TreeContext>(options => options.UseSqlServer(connectionString));

// Đăng ký EmailNotificationService
builder.Services.AddHostedService<EmailNotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "trang-chu",
        pattern: "trang-chu",
        defaults: new { controller = "Home", action = "Index" });

    endpoints.MapControllerRoute(
        name: "su-kien",
        pattern: "su-kien",
        defaults: new { controller = "Event", action = "Index" });

    endpoints.MapControllerRoute(
       name: "lien-he",
       pattern: "lien-he",
       defaults: new { controller = "Contact", action = "Index" });

    endpoints.MapControllerRoute(
        name: "dang-ky",
        pattern: "dang-ky",
        defaults: new { controller = "Account", action = "Register" });

    endpoints.MapControllerRoute(
        name: "dang-nhap",
        pattern: "dang-nhap",
        defaults: new { controller = "Account", action = "Login" });

    endpoints.MapControllerRoute(
        name: "dang-xuat",
        pattern: "dang-xuat",
        defaults: new { controller = "Account", action = "Logout" });

    endpoints.MapControllerRoute(
       name: "thong-tin",
       pattern: "thong-tin",
       defaults: new { controller = "Account", action = "Info" });

    endpoints.MapControllerRoute(
        name: "chinh-sua-thong-tin",
        pattern: "chinh-sua-thong-tin",
        defaults: new { controller = "Account", action = "EditInfo" });

    endpoints.MapControllerRoute(
        name: "su-kien-ca-nhan",
        pattern: "su-kien-ca-nhan",
        defaults: new { controller = "PersonalEvent", action = "Index" });

    endpoints.MapControllerRoute(
        name: "them_su-kien-ca-nhan",
        pattern: "them-su-kien-ca-nhan",
        defaults: new { controller = "PersonalEvent", action = "Create" }); 

     endpoints.MapControllerRoute(
        name: "chi-tiet-su-kien-ca-nhan",
        pattern: "chi-tiet-su-kien-ca-nhan/{id}",
        defaults: new { controller = "PersonalEvent", action = "Details" });

    endpoints.MapControllerRoute(
       name: "chinh-sua-su-kien-ca-nhan",
       pattern: "chinh-sua-su-kien-ca-nhan/{id}",
       defaults: new { controller = "PersonalEvent", action = "Edit" });

    endpoints.MapControllerRoute(
        name: "quan-ly_su-kien-ca-nhan",
        pattern: "quan-ly-su-kien-ca-nhan",
        defaults: new { controller = "PersonalEventManagement", action = "Index" });

    endpoints.MapControllerRoute(
        name: "ql-chi-tiet-su-kien-ca-nhan",
        pattern: "ql-chi-tiet-su-kien-ca-nhan/{id}",
        defaults: new { controller = "PersonalEventManagement", action = "Details" });

    endpoints.MapControllerRoute(
        name: "ql-chinh-sua-su-kien-ca-nhan",
        pattern: "ql-chinh-sua-su-kien-ca-nhan/{id}",
        defaults: new { controller = "PersonalEventManagement", action = "Edit" });

    endpoints.MapControllerRoute(
        name: "tim-kiem-su-kien-ca-nhan",
        pattern: "tim-kiem-su-kien-ca-nhan",
        defaults: new { controller = "PersonalEventManagement", action = "Search" });

    endpoints.MapControllerRoute(
        name: "quan-ly-su-kien",
        pattern: "quan-ly-su-kien",
        defaults: new { controller = "EventManagement", action = "Index" });

    endpoints.MapControllerRoute(
       name: "ql-them-su-kien",
       pattern: "ql-them-su-kien",
       defaults: new { controller = "EventManagement", action = "Create" });

    endpoints.MapControllerRoute(
      name: "ql-chi-tiet-su-kien",
      pattern: "ql-chi-tiet-su-kien/{id}",
      defaults: new { controller = "EventManagement", action = "Details" });

    endpoints.MapControllerRoute(
     name: "ql-chinh-sua-su-kien",
     pattern: "ql-chinh-sua-su-kien/{id}",
     defaults: new { controller = "EventManagement", action = "Edit" });

    endpoints.MapControllerRoute(
        name: "quan-ly-danh-muc",
        pattern: "quan-ly-danh-muc",
        defaults: new { controller = "Category", action = "Index" });

    endpoints.MapControllerRoute(
        name: "chi-tiet-danh-muc",
        pattern: "chi-tiet-danh-muc/{id}",
        defaults: new { controller = "Category", action = "Details" });

    endpoints.MapControllerRoute(
        name: "tao-danh-muc",
        pattern: "tao-danh-muc",
        defaults: new { controller = "Category", action = "Create" });

    endpoints.MapControllerRoute(
        name: "chinh-sua-danh-muc",
        pattern: "chinh-sua-danh-muc/{id}",
        defaults: new { controller = "Category", action = "Edit" });

    endpoints.MapControllerRoute(
       name: "tim-kiem-danh-muc",
       pattern: "tim-kiem-danh-muc",
       defaults: new { controller = "Category", action = "Search" });

    endpoints.MapControllerRoute(
       name: "quan-ly-tai-khoan",
       pattern: "quan-ly-tai-khoan",
       defaults: new { controller = "AccountManagement", action = "Index" });

    endpoints.MapControllerRoute(
      name: "quan-ly-chinh-sua-tai-khoan",
      pattern: "quan-ly-chinh-sua-tai-khoan/{id}",
      defaults: new { controller = "AccountManagement", action = "Edit" });

    endpoints.MapControllerRoute(
        name: "tim-kiem",
        pattern: "tim-kiem",
        defaults: new { controller = "Event", action = "TimKiem" });

    endpoints.MapControllerRoute(
        name: "the-loai-su-kien",
        pattern: "{slug}-{id}",
        defaults: new { controller = "Event", action = "CateEvent" });

    endpoints.MapControllerRoute(
        name: "chi-tiet-su-kien",
        pattern: "su-kien/{slug}-{id}",
        defaults: new { controller = "Event", action = "EventDetail" });

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
