using Clinic.Infrastructure.Persistence;
using Hospital.Application.Helper;
using Hospital.Application.Interfaces.Payment;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Application.MappingProfiles;
using Hospital.Domain.Models;
using Hospital.Infrastructure.Payment;
using Hospital.Infrastructure.PaymentRepAndService;
using Hospital.Infrastructure.Repos;
using Hospital.Infrastructure.Repository;
using Hospital.Infrastructure.Services;
using Hospital.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Hospital
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(
                "logs/log-.txt",
                rollingInterval: RollingInterval.Day
            )
            .CreateLogger();


            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();


            // =====================================
            // MVC + Swagger
            // =====================================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // =====================================
            // Database
            // =====================================
            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // =====================================
            // Config Sections
            // =====================================
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
            builder.Services.Configure<PaymobOptions>(builder.Configuration.GetSection(PaymobOptions.SectionName));
            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

            // =====================================
            // HttpClient
            // =====================================
            builder.Services.AddHttpClient<IPaymobClient, PaymobClient>();

            // =====================================
            // Dependency Injection
            // =====================================
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IBranchService, BranchService>();
            builder.Services.AddScoped<INewsService, NewsService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IBannerService, BannerService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<ISpecializationService, SpecializationService>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IScheduleService, ScheduleService>();
            builder.Services.AddScoped<ISupportTicketService, SupportTicketService>();
            builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<ITotalRevenueService, TotalRevenueService>();
            builder.Services.AddSingleton<IMemoryService, MemoryService>();
            builder.Services.AddHttpClient("PythonRagClient", client =>
            {
                client.BaseAddress = new Uri("http://localhost:8001/");
            });

            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<IBranchRepository, BranchRepository>();
            builder.Services.AddScoped<INewsRepository, NewsRepository>();
            builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
            builder.Services.AddScoped<IBannerRepository, BannerRepository>();
            builder.Services.AddScoped<ISpecializationRepository, SpecializationRepository>();
            builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
            builder.Services.AddScoped<IPatientRepository, PatientRepository>();
            builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
            builder.Services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
            builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();

            // =====================================
            // Identity
            // =====================================
            builder.Services
                .AddIdentity<User, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // =====================================
            // AutoMapper
            // =====================================
            builder.Services.AddAutoMapper(typeof(EventProfile));
            builder.Services.AddAutoMapper(typeof(BranchProfile));
            builder.Services.AddAutoMapper(typeof(NewsProfile));
            builder.Services.AddAutoMapper(typeof(SpecializationProfile));
            builder.Services.AddAutoMapper(typeof(BannerProfile));
            builder.Services.AddAutoMapper(typeof(ServiceProfile));
            builder.Services.AddAutoMapper(typeof(PatientProfile));
            builder.Services.AddAutoMapper(typeof(ScheduleProfile));
            builder.Services.AddAutoMapper(typeof(SupportTicketProfile));
            builder.Services.AddAutoMapper(typeof(DoctorProfile));
            builder.Services.AddAutoMapper(typeof(MedicalRecordProfile));
            builder.Services.AddAutoMapper(typeof(AppointmentProfile));

            // =====================================
            // CORS
            // =====================================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowWebApp",
                    policyBuilder => policyBuilder
                        .WithOrigins(
                            "http://127.0.0.1:5500",
                            "http://localhost:4200"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
            });

            // =====================================
            // JWT Authentication
            // =====================================

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            builder.Services
                .AddAuthentication(options =>
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
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,

                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])
                        ),

                        // IMPORTANT FIX:
                        // ASP.NET Core must use ClaimTypes.Role for authorization to work
                        NameClaimType = "uid",
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            // =====================================
            // Authorization Policies
            // =====================================
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("DoctorPolicy", policy => policy.RequireRole("Doctor"));
                options.AddPolicy("PatientPolicy", policy => policy.RequireRole("Patient"));
            });

            // =====================================
            // JSON Settings
            // =====================================
            builder.Services.AddControllers().AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.DefaultIgnoreCondition =
                    System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            // =====================================
            // App Pipeline
            // =====================================
            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionHandler>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowWebApp");

            app.UseAuthentication(); // MUST COME FIRST
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}