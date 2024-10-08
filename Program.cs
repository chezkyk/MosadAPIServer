using Microsoft.EntityFrameworkCore;
using MosadAPIServer.Middleware.Global;
using MosadAPIServer.Middleware.Login;
using MosadAPIServer.Services;

var builder = WebApplication.CreateBuilder(args);
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MosadDbContext>(options => options.UseSqlServer(connectionString));
// Add services to the container.
builder.Services.AddScoped<AgentService>();
builder.Services.AddScoped<TargetService>();
builder.Services.AddScoped<MissionService>();
builder.Services.AddScoped<LocationService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
//
//var myRouts = new[] { "/agents" };
//app.UseMiddleware<GlobalLoginMiddleware>();
//app.UseWhen(
//        context => myRouts.Any(rout => context.Request.Path.StartsWithSegments(rout)),
//    appBuilder =>
//    {
//        appBuilder.UseMiddleware<LoginMiddleware>();
//        appBuilder.UseMiddleware<JwtValidationMiddleware>();
//    });
////
app.MapControllers();

app.Run();
