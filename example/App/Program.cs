using App.Endpoints;
using MinimalREPR.Core.Endpoints;
using MinimalREPR.ExtensionMethods;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.SetupMinimalREPR();
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

app.MapEndpoints();
//app.MapControllers();

app.Run();

