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

//Dang ky Service
builder.Services.AddScoped<IServiceTicketService, ServiceTicketService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IGarageServiceService, GarageServiceService>();
builder.Services.AddScoped<IPartService, PartService>();
builder.Services.AddScoped<IPartCategoryService, PartCategoryService>();
builder.Services.AddScoped<ITechnicalTaskService, TechnicalTaskService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IUserService, UserService>();

//Dang ky Repo
builder.Services.AddScoped<IServiceTicketRepo, ServiceTicketRepo>();
builder.Services.AddScoped<IBookingRepo, BookingRepo>();
builder.Services.AddScoped<IGarageServiceRepo, GarageServiceRepo>();
builder.Services.AddScoped<IPartRepo, PartRepo>();
builder.Services.AddScoped<IPartCategoryRepo, PartCategoryRepo>();
builder.Services.AddScoped<ITechnicalTaskRepo, TechnicalTaskRepo>();
builder.Services.AddScoped<IInvoiceRepo, InvoiceRepo>();
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
builder.Services.AddScoped<IVehicleRepo, VehicleRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IBaseRepo<User>, BaseRepo<User>>();
builder.Services.AddScoped<IBaseRepo<Vehicle>, BaseRepo<Vehicle>>();
builder.Services.AddScoped<IBaseRepo<Part>, BaseRepo<Part>>();
builder.Services.AddScoped<IBaseRepo<Booking>, BaseRepo<Booking>>();
builder.Services.AddScoped<IBaseRepo<Customer>, BaseRepo<Customer>>();
builder.Services.AddScoped<IBaseRepo<ServiceTicketDetail>, BaseRepo<ServiceTicketDetail>>();
builder.Services.AddScoped<IBaseRepo<TechnicalTask>, BaseRepo<TechnicalTask>>();
builder.Services.AddScoped<IBaseRepo<GarageService>, BaseRepo<GarageService>>();
builder.Services.AddScoped<IBaseRepo<PartCategory>, BaseRepo<PartCategory>>();
builder.Services.AddScoped<IBaseRepo<Invoice>, BaseRepo<Invoice>>();
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
