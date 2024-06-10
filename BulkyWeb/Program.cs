
//string defaultAction = "Index";
//string PrivacyAction = "Privacy";
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using BulkyWeb.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Services
// Add services to the container, all services added can be used with dependency injection.
builder.Services.AddControllersWithViews();

//adds the EntityFramwoks service that's encapsulated within the ApplicationDbContext class, on the Package Manager Console use the update-database commnad
//To create the database with the specified ConnectionString.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
var app = builder.Build();
#endregion

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


//The URL pattern for routing is considered after the domain name
//https://localHOST:55555/Category/Index/3
//https://localHOST:55555/{controller}/{action}/{id}
app.MapControllerRoute(
    name: "default", //domain name
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"); //category, action, id

app.Run();
