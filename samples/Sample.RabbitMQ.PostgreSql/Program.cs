using Sample.RabbitMQ.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

var rabbitMqOptions = builder.Configuration.GetSection("RabbitMqOptions").Get<RabbitMqOptions>();
builder.Services.Configure<DbOptions>(builder.Configuration.GetSection("DbOptions"));

builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddCap(x =>
{
    x.UseEntityFramework<AppDbContext>();

    x.UseRabbitMQ(z =>
    {
        z.HostName = rabbitMqOptions.HostName;
        z.UserName = rabbitMqOptions.UserName;
        z.Password = rabbitMqOptions.Password;
    });
    x.UseDashboard();
    x.FailedRetryInterval = 10;
    x.UseStorageLock = true;
    x.FailedRetryCount = 2;
    x.FailedThresholdCallback = failed => { };
});

builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();