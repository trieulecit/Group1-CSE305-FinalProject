using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DeliveryAgentManagementApp.Data;
using Microsoft.AspNetCore.Identity;
using DeliveryAgentManagementApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DeliveryAgentManagementAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DeliveryAgentManagementAppContext") ?? throw new InvalidOperationException("Connection string 'DeliveryAgentManagementAppContext' not found.")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<DeliveryAgentManagementAppContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Accounts/AccessDenied";
    options.Cookie.Name = "DeliveryAgentManagementAppCookie";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Accounts/Login";
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
});

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1000);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole<int>>>();
    if (!roleManager.Roles.Any())
    {
        await roleManager.CreateAsync(new IdentityRole<int>() { Name = "manager" });
        await roleManager.CreateAsync(new IdentityRole<int>() { Name = "courier" });
    }

    var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
    if (await userManager.FindByNameAsync("manager") == null)
    {
        var result = await userManager.CreateAsync(new ApplicationUser { UserName = "manager" }, "aA12#$%");
    }

    var manager = await userManager.FindByNameAsync("manager");
    await userManager.AddToRoleAsync(manager, "manager");



}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.UseSession();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Orders}/{action=Index}/{id?}");

app.Run();
