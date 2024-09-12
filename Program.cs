using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IDP.Data;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using IDP;

var builder = WebApplication.CreateBuilder(args);

var userConnection = builder.Configuration.GetConnectionString("DefaultConnection");
var identityConnection = builder.Configuration.GetConnectionString("IdentityConnection");
var migrationAssembly = typeof(Program).Assembly.GetName().Name;
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(userConnection,
    sql=>sql.MigrationsAssembly(migrationAssembly)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    .AddConfigurationStore(option=> 
    { 
        option.ConfigureDbContext = builder => builder.UseSqlServer(identityConnection,
    sql => sql.MigrationsAssembly(migrationAssembly)); 
    })
    .AddOperationalStore(option =>
    {
        option.ConfigureDbContext = builder => builder.UseSqlServer(identityConnection,
    sql => sql.MigrationsAssembly(migrationAssembly));
    })
    .AddDeveloperSigningCredential();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
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
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
