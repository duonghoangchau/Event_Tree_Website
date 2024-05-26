using Event_Tree_Website.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString =
builder.Configuration.GetConnectionString("Event_TreeConnection");
builder.Services.AddDbContext<Event_TreeContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();


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
    name: "ql_su_kien",
    pattern: "ql_event",
    defaults: new { controller = "EventManagement", action = "Index" });

    endpoints.MapControllerRoute(
    name: "ql_danhmuc",
    pattern: "ql_danhmuc",
    defaults: new { controller = "Category", action = "Index" });

    endpoints.MapControllerRoute(
    name: "chi-tiet-danh-muc",
    pattern: "chi-tiet-danh-muc/{id}",
    defaults: new { controller = "Category", action = "Details" });

    endpoints.MapControllerRoute
    (name: "tao-danh-muc",
    pattern: "tao-danh-muc",
    defaults: new { controller = "Category", action = "Create" });

    endpoints.MapControllerRoute
    (name: "chinh-sua-danh-muc",
    pattern: "chinh-sua-danh-muc/{id}",
    defaults: new { controller = "Category", action = "Edit" });

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

    app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.Run();
