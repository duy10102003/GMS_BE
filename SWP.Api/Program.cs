using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;
using SWP.Core.Services;
using SWP.Core.Entities;
using SWP.Infrastructure.Repositories;
using MISA.QLSX.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
// Cấu hình Dapper để tự động map snake_case sang PascalCase
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Đăng ký Service
builder.Services.AddScoped<IServiceTicketService, ServiceTicketService>();

//Đăng ký Repo
builder.Services.AddScoped<IServiceTicketRepo, ServiceTicketRepo>();
builder.Services.AddScoped<IBaseRepo<User>, BaseRepo<User>>();
builder.Services.AddScoped<IBaseRepo<Vehicle>, BaseRepo<Vehicle>>();
builder.Services.AddScoped<IBaseRepo<Part>, BaseRepo<Part>>();
builder.Services.AddScoped<IBaseRepo<Booking>, BaseRepo<Booking>>();
builder.Services.AddScoped<IBaseRepo<Customer>, BaseRepo<Customer>>();
builder.Services.AddScoped<IBaseRepo<ServiceTicketDetail>, BaseRepo<ServiceTicketDetail>>();
builder.Services.AddScoped<IBaseRepo<TechnicalTask>, BaseRepo<TechnicalTask>>();
builder.Services.AddScoped<IBaseRepo<GarageService>, BaseRepo<GarageService>>();
builder.Services.AddScoped<IBaseRepo<Inventory>, BaseRepo<Inventory>>();
//Khai báo cross để FE gọi đến 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ValidateExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();
