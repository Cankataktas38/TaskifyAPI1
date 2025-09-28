using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskifyAPI.Data;
using TaskifyAPI.Services;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
//jwt settings ( demo secret - deðiþtir!)
builder.Configuration["Jwt:Key"] = "ThisIsASecretKeyForDemoPurposeOnly!ChangeMe";
builder.Configuration["Jwt:Issuer"] = "TaskifyAPI";
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//DbContext - demo için InMemory , productionda UseSqlServer kullanýlýr
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseInMemoryDatabase("TaskifyDb"));

//Services
builder.Services.AddScoped<ITaskService,TaskService>();
builder.Services.AddScoped<IAuthService,AuthService>();

//JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();
//Seed demo user
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var auth = scope.ServiceProvider.GetRequiredService<IAuthService>();

    //Add a demo user
    if (!ctx.Users.Any())
    {
        var u = auth.Register(new TaskifyAPI.DTOs.RegisterDto { UserName = "demo",Password = "demo123"}).Result;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();
