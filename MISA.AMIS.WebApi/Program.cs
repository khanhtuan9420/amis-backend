using MISA.AMIS.WebApi;
using MISA.AMIS.WebApi.BL;
using MISA.AMIS.WebApi.DL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ValidateModelAttribute));
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
}
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddScoped(typeof(IBaseDL<>), typeof(BaseDL<>));
//builder.Services.AddScoped(typeof(IBaseBL<>), typeof(BaseBL<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAccountDL, AccountDL>();
builder.Services.AddScoped<IAccountBL, AccountBL>();
builder.Services.AddScoped<ISupplierDL, SupplierDL>();
builder.Services.AddScoped<ISupplierBL, SupplierBL>();
builder.Services.AddScoped<IEmployeeDL, EmployeeDL>();
builder.Services.AddScoped<IEmployeeBL, EmployeeBL>();
builder.Services.AddScoped<IPaymentDL, PaymentDL>();
builder.Services.AddScoped<IPaymentBL, PaymentBL>();
builder.Services.AddScoped<IPaymentDetailDL, PaymentDetailDL>();
builder.Services.AddScoped<IPaymentDetailBL, PaymentDetailBL>();

builder.Services.AddCors(p => p.AddPolicy("MyCors", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("MyCors");
app.MapControllers();
app.UseMiddleware<ExceptionMiddleware>();

app.Run();
