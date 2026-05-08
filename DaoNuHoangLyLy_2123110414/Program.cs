using System.Text;
using System.Text.Json.Serialization;
using DaoNuHoangLyLy_2123110414.Data;
using DaoNuHoangLyLy_2123110414.Models;
using DaoNuHoangLyLy_2123110414.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

#region Add Services

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var messages = context.ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? "Dữ liệu không hợp lệ."
                    : error.ErrorMessage)
                .Distinct();

            return new BadRequestObjectResult(ServiceResult.Fail(string.Join(" ", messages)));
        };
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

#endregion

#region JWT Authentication

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception("Jwt:Key is missing in appsettings.json");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

#endregion

#region Dependency Injection

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<ITimeSlotService, TimeSlotService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IPatientService, PatientService>();

#endregion

#region Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "ClinicBooking API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập token dạng: Bearer {token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

var app = builder.Build();

#region Seed Data (Role + Admin)

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedDataAsync(services);
}

#endregion

#region Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Content("""
<!doctype html>
<html lang="vi">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Clinic Booking API</title>
    <style>
        :root {
            --ink: #18332f;
            --muted: #61706d;
            --paper: #fff9ed;
            --leaf: #7eb77f;
            --deep: #174840;
            --sun: #f4b860;
            --card: rgba(255, 255, 255, .78);
        }

        * { box-sizing: border-box; }

        body {
            min-height: 100vh;
            margin: 0;
            display: grid;
            place-items: center;
            padding: 32px;
            color: var(--ink);
            font-family: Candara, "Trebuchet MS", sans-serif;
            background:
                radial-gradient(circle at 14% 18%, rgba(126, 183, 127, .38), transparent 30%),
                radial-gradient(circle at 85% 14%, rgba(244, 184, 96, .38), transparent 28%),
                linear-gradient(135deg, #fff9ed 0%, #eaf5e5 52%, #dceeea 100%);
        }

        main {
            width: min(980px, 100%);
            border: 1px solid rgba(24, 51, 47, .13);
            border-radius: 34px;
            padding: clamp(26px, 5vw, 54px);
            background: var(--card);
            box-shadow: 0 30px 80px rgba(23, 72, 64, .16);
            backdrop-filter: blur(16px);
        }

        .eyebrow {
            color: var(--deep);
            font-weight: 800;
            letter-spacing: .18em;
            text-transform: uppercase;
        }

        h1 {
            max-width: 760px;
            margin: 12px 0;
            font-family: Georgia, "Times New Roman", serif;
            font-size: clamp(42px, 7vw, 86px);
            line-height: .94;
        }

        p {
            max-width: 690px;
            margin: 0;
            color: var(--muted);
            font-size: 19px;
            line-height: 1.65;
        }

        .actions {
            display: flex;
            flex-wrap: wrap;
            gap: 14px;
            margin-top: 34px;
        }

        a {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 48px;
            padding: 0 22px;
            border-radius: 999px;
            color: #fff;
            background: var(--deep);
            font-weight: 800;
            text-decoration: none;
            box-shadow: 0 14px 24px rgba(23, 72, 64, .18);
        }

        a.secondary {
            color: var(--deep);
            background: #fff;
        }

        .grid {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 14px;
            margin-top: 36px;
        }

        .card {
            min-height: 132px;
            padding: 20px;
            border-radius: 24px;
            background: rgba(255, 255, 255, .62);
            border: 1px solid rgba(24, 51, 47, .1);
        }

        .card strong {
            display: block;
            margin-bottom: 8px;
            color: var(--deep);
            font-size: 18px;
        }

        .card span {
            color: var(--muted);
            line-height: 1.5;
        }

        @media (max-width: 760px) {
            body { padding: 18px; }
            .grid { grid-template-columns: 1fr; }
        }
    </style>
</head>
<body>
    <main>
        <div class="eyebrow">Backend online</div>
        <h1>Clinic Booking API</h1>
        <p>
            API quản lý đặt lịch khám bệnh: xác thực JWT, phân quyền Admin/Doctor/Patient,
            quản lý chuyên khoa, bác sĩ, lịch làm việc, khung giờ và lịch hẹn.
        </p>
        <div class="actions">
            <a href="/swagger">Mở Swagger UI</a>
            <a class="secondary" href="http://localhost:5173">Mở React Frontend</a>
        </div>
        <section class="grid" aria-label="API modules">
            <div class="card"><strong>Admin</strong><span>Tạo bác sĩ, chuyên khoa, lịch làm việc và xác nhận lịch hẹn.</span></div>
            <div class="card"><strong>Patient</strong><span>Đăng ký, chọn bác sĩ, chọn slot và đặt/hủy lịch khám.</span></div>
            <div class="card"><strong>Doctor</strong><span>Xem lịch hẹn của mình, hoàn tất khám hoặc đánh dấu NoShow.</span></div>
        </section>
    </main>
</body>
</html>
""", "text/html"));

app.MapControllers();

#endregion

app.Run();

#region Seed Method

static async Task SeedDataAsync(IServiceProvider services)
{
    var context = services.GetRequiredService<ApplicationDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    await context.Database.MigrateAsync();

    var roles = new[]
    {
        SystemRoles.Admin,
        SystemRoles.Doctor,
        SystemRoles.Patient
    };

    foreach (var role in roles)
    {
        var exists = await roleManager.RoleExistsAsync(role);
        if (!exists)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var adminEmail = "admin@clinic.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "123456");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, SystemRoles.Admin);
        }
    }

    await SeedDefaultSpecialtiesAsync(context);
}

static async Task SeedDefaultSpecialtiesAsync(ApplicationDbContext context)
{
    var specialties = new[]
    {
        new Specialty { Name = "Nội tổng quát", Description = "Khám và tư vấn các bệnh lý nội khoa thường gặp." },
        new Specialty { Name = "Tim mạch", Description = "Khám, theo dõi và tư vấn bệnh lý tim mạch." },
        new Specialty { Name = "Nhi khoa", Description = "Khám bệnh cho trẻ em và tư vấn chăm sóc sức khỏe trẻ nhỏ." },
        new Specialty { Name = "Da liễu", Description = "Khám và điều trị các vấn đề về da, tóc, móng." },
        new Specialty { Name = "Tai mũi họng", Description = "Khám các bệnh lý tai, mũi, họng thường gặp." }
    };

    foreach (var specialty in specialties)
    {
        var exists = await context.Specialties.AnyAsync(x => x.Name == specialty.Name);
        if (!exists)
        {
            context.Specialties.Add(specialty);
        }
    }

    await context.SaveChangesAsync();
}

#endregion
