using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpeakUp.Services;
using SpeakUpCSharp.Data;
using SpeakUpCSharp.Models;
using SpeakUpCSharp.Services;
using SpeakUpCSharp.Utilities;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var SvelteOrigins = "_svelteOrigins";
var jwtKey = builder.Configuration.GetValue<string>("JwtKey");
var jwtIssuer = builder.Configuration.GetValue<string>("JwtIssuer");

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")??throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddCors(options => {
	options.AddPolicy(name: SvelteOrigins,
		policy => {
			policy.WithOrigins("https://www.speak-up.top").AllowAnyHeader().AllowCredentials().AllowAnyMethod();
			policy.WithOrigins("http://localhost:5555").AllowAnyHeader().AllowCredentials().AllowAnyMethod();
			policy.WithOrigins("http://192.168.1.106:5555").AllowAnyHeader().AllowCredentials().AllowAnyMethod();
		});
});

builder.Services.AddIdentity<ApplicationUser,IdentityRole<int>>(options => {
	options.ClaimsIdentity.UserNameClaimType=ClaimTypes.Name;
	options.ClaimsIdentity.UserIdClaimType=ClaimTypes.NameIdentifier;
	options.ClaimsIdentity.EmailClaimType=ClaimTypes.Email;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options => {
	options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme=JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
	options.RequireHttpsMetadata=false; // For development, set to true in production
	options.SaveToken=true;
	options.TokenValidationParameters=new TokenValidationParameters {
		ValidateIssuer=true,
		ValidateAudience=true,
		ValidateLifetime=true,
		ValidateIssuerSigningKey=true,
		ValidIssuer=jwtIssuer,
		ValidAudience=jwtIssuer,
		IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
		ClockSkew=TimeSpan.FromDays(1) // Reduce the default clock skew to immediate token expiry
	};
});

builder.Services.AddScoped<IDbInitializer,DbInitializer>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped<IImageService,ImageService>();
builder.Services.AddScoped<IReviewDateService, ReviewDateService>();
builder.Services.AddScoped<IDailyPerformanceService, DailyPerformanceService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
	app.UseMigrationsEndPoint();
} else {
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();

//DataSeeding();

app.UseCors(SvelteOrigins);

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

//void DataSeeding() {
//	using (var scope = app.Services.CreateScope()) {
//		var DbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
//		DbInitializer.Initialize();
//	}
//}