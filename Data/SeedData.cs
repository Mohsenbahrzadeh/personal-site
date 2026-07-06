using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PersonalSite.Data;

public static class SeedData
{
    public const string AdminEmail = "mohsen.bahrzadeh@gmail.com";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await userManager.FindByEmailAsync(AdminEmail);
        if (admin is null)
        {
            var configuration = services.GetRequiredService<IConfiguration>();
            var password = configuration["AdminSeed:Password"];

            if (string.IsNullOrEmpty(password))
            {
                password = $"Admin!{Guid.NewGuid():N}"[..20];
                var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");
                logger.LogWarning(
                    "AdminSeed:Password تنظیم نشده — رمز موقت تولید شد: {Password} (بعد از اولین ورود عوضش کن، یا با dotnet user-secrets مقدار AdminSeed:Password را ست کن)",
                    password);
            }

            admin = new ApplicationUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                DisplayName = "محسن بحرزاده",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, password);
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        if (!await db.Posts.AnyAsync())
        {
            db.Posts.AddRange(
                new Post
                {
                    Title = "یکپارچه‌سازی سازمانی با .NET",
                    Slug = "enterprise-integration-dotnet",
                    Summary = "Enterprise Integration Patterns، Event-Driven Architecture، Saga، Outbox و MassTransit 8 در یک پروژه واقعی",
                    Body = "<p>دوره کامل یکپارچه‌سازی سازمانی: از الگوهای مسیریابی پیام تا Saga و Outbox، همراه با پروژه نمونه فروشگاه آنلاین.</p>",
                    Tags = "EIP, MassTransit, Saga, Outbox",
                    Icon = "🧩",
                    AccentIndex = 1
                },
                new Post
                {
                    Title = "Testing در ASP.NET Core",
                    Slug = "testing-aspnetcore",
                    Summary = "Unit، Integration و E2E Testing با xUnit، Testcontainers و استراتژی‌های تست microservices",
                    Body = "<p>راهنمای عملی تست‌نویسی در ASP.NET Core با ابزارهای مدرن.</p>",
                    Tags = "xUnit, Testcontainers, Integration Test",
                    Icon = "🧪",
                    AccentIndex = 2
                },
                new Post
                {
                    Title = "Identity & Access Management",
                    Slug = "identity-access-management",
                    Summary = "احراز هویت، Authorization، OpenID Connect، OAuth 2.0 و پیاده‌سازی IDP با Duende IdentityServer",
                    Body = "<p>مبانی و پیاده‌سازی عملی IAM در برنامه‌های .NET.</p>",
                    Tags = "OIDC, OAuth 2.0, IdentityServer",
                    Icon = "🔐",
                    AccentIndex = 3
                },
                new Post
                {
                    Title = "Saga Pattern پیشرفته",
                    Slug = "advanced-saga-pattern",
                    Summary = "Orchestration vs Choreography، Saga State Machine، Compensation و Outbox در عمق",
                    Body = "<p>بررسی عمیق الگوی Saga در معماری Event-Driven.</p>",
                    Tags = "Orchestration, Choreography, Compensation",
                    Icon = "🔄",
                    AccentIndex = 4
                },
                new Post
                {
                    Title = "Messaging — RabbitMQ & Kafka",
                    Slug = "messaging-rabbitmq-kafka",
                    Summary = "از مبانی تا پیشرفته: AMQP، Exchange، Consumer Group، Schema Registry و MassTransit",
                    Body = "<p>مقایسه و پیاده‌سازی messaging با RabbitMQ و Kafka.</p>",
                    Tags = "RabbitMQ, Kafka, MassTransit",
                    Icon = "📨",
                    AccentIndex = 5
                },
                new Post
                {
                    Title = "Observability در ASP.NET Core",
                    Slug = "observability-aspnetcore",
                    Summary = "Logs با Serilog، Metrics با Prometheus، Tracing با OpenTelemetry",
                    Body = "<p>پیاده‌سازی مانیتورینگ و ردیابی در سیستم‌های توزیع‌شده.</p>",
                    Tags = "OpenTelemetry, Grafana, Prometheus, Serilog",
                    Icon = "🔭",
                    AccentIndex = 6
                },
                new Post
                {
                    Title = "EF Core پیشرفته برای معماران",
                    Slug = "advanced-efcore",
                    Summary = "Change Tracking، Interceptors، Concurrency، ExecuteUpdate، Multi-tenancy و Zero-Downtime Migration",
                    Body = "<p>موضوعات پیشرفته EF Core برای پروژه‌های سازمانی.</p>",
                    Tags = "Change Tracking, Interceptors, Concurrency, Migrations",
                    Icon = "🗄️",
                    AccentIndex = 1
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
