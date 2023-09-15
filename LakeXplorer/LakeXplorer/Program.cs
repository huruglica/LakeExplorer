using AutoMapper;
using FluentValidation;
using Hangfire;
using LakeXplorer.Data;
using LakeXplorer.Helpers;
using LakeXplorer.Models.Dto.LakeDtos;
using LakeXplorer.Models.Dto.LakeSightingDtos;
using LakeXplorer.Models.Dto.UserDtos;
using LakeXplorer.Repository;
using LakeXplorer.Repository.IRepository;
using LakeXplorer.Services;
using LakeXplorer.Services.ISerices;
using LakeXplorer.Validators.LakeSightingValidator;
using LakeXplorer.Validators.LakeValidator;
using LakeXplorer.Validators.UserValidator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace LakeXplorer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins("http://192.168.0.18:5500")
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            builder.Services.AddHangfire(x =>
                x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DeafultConnection")));
            builder.Services.AddHangfireServer();

            var mapperConfiguration = new MapperConfiguration(
                mc => mc.AddProfile(new AutoMapperConfiguration()));

            IMapper mapper = mapperConfiguration.CreateMapper();

            builder.Services.AddSingleton(mapper);

            builder.Services.AddDbContext<LakeXplorerDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DeafultConnection")));

            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<ILakeService, LakeService>();
            builder.Services.AddTransient<ILakeSightingService, LakeSightingService>();
            builder.Services.AddTransient<ILikeService, LikeService>();

            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ILakeRepository, LakeRepository>();
            builder.Services.AddScoped<ILakeSightingRepository, LakeSightingRepository>();
            builder.Services.AddScoped<ILikeRepository, LikeRepository>();

            builder.Services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();

            builder.Services.AddScoped<IValidator<LakeCreateDto>, LakeCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<LakeUpdateDto>, LakeUpdateDtoValidator>();

            builder.Services.AddScoped<IValidator<LakeSightingCreateDto>, LakeSightingCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<LakeSightingUpdateDto>, LakeSightingUpdateDtoValidator>();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "dev-nq3upfdndrxpn4bz.us.auth0.com",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("3RiMMI3eusj2CJu15cJQIpXP8YallpXQQj8ad_13GiLu4uS7sUxL3Wezw6HpzfLL"))
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "LakeXplorer", Version = "v1" });
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization",
                    In = ParameterLocation.Header
                });
                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            var app = builder.Build();

            app.UseCors("AllowSpecificOrigin");

            app.UseHangfireDashboard();

            var scope = app.Services.CreateScope();
            var lakeSightingService = scope.ServiceProvider.GetService<ILakeSightingService>()
                ?? throw new Exception("Failed to get service");
            HangfireService hagfireService = new HangfireService(lakeSightingService);

            RecurringJob.AddOrUpdate("DailyFunFact", () => hagfireService.DailyFunFact(), Cron.Daily);


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.DisplayRequestDuration();
                    c.DefaultModelExpandDepth(0);

                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LakeXplorer");
                    c.OAuthClientId("pwhlJHybcPXXkCWZ2MGLXlDFTXD9oTK4");
                    c.OAuthClientSecret("3RiMMI3eusj2CJu15cJQIpXP8YallpXQQj8ad_13GiLu4uS7sUxL3Wezw6HpzfLL");
                    c.OAuthAppName("LakeXplorer");
                    //c.OAuthUsePkce();
                    //c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                    c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}