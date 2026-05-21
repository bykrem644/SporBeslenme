using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SporBeslenmeWeb.Data;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// ... (diðer builder.Services kodlarý) ...

// --- SÝBER GÜVENLÝK: RATE LIMITING ---
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("LoginKorumasi", config =>
    {
        config.PermitLimit = 5; // 1 Dakikada en fazla 5 deneme yapabilir
        config.Window = TimeSpan.FromMinutes(1); // Süre penceresi: 1 Dakika
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0; // Sýnýrý aþanlarý sýraya alma, direkt reddet!
    });

    // Sýnýrý aþan saldýrgana verilecek yanýt (HTTP 429 Too Many Requests)
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
// Kendi özel Login/Register ekranlarýmýzý yapacaðýmýz tam profesyonel Identity altyapýsý
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
// Microsoft'un varsayýlan Ýngilizce yönlendirmelerini bizim Türkçe sayfalara çeviriyoruz
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Giris"; // Giriþ yapmamýþsa buraya at
    options.AccessDeniedPath = "/Account/Giris"; // Yetkisi yoksa yine buraya at
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRateLimiter();
app.UseRouting();

app.UseAuthorization();
app.MapHub<SporBeslenmeWeb.Hubs.NotificationHub>("/notificationHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
