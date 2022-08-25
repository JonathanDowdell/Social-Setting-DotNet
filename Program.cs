using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Social_Setting.Comment.Service;
using Social_Setting.Database;
using Social_Setting.Exception;
using Social_Setting.Post.Service;
using Social_Setting.Setting.Service;
using Social_Setting.Token.Service;
using Social_Setting.User.Service;
using Social_Setting.Vote.Service;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Databases
// builder.Services.AddDbContext<ApplicationApiDbContext>(options => options.UseInMemoryDatabase("Social-Setting"));
builder.Services.AddDbContext<ApplicationApiDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SocialSettingConnectionString");
    Console.WriteLine(connectionString);
    options.UseNpgsql(connectionString);
});

// Controllers
builder.Services.AddControllers();

// Swagger & Endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (Bearer {token})",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretToken = builder.Configuration.GetSection("AppSettings:Token").Value;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretToken)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IVoteService, VoteService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();