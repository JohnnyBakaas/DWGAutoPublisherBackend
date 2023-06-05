using DWGAutoPublisherBackend.Model;
using DWGAutoPublisherBackend.Model.AutoCAD_Handeler;

//LayoutReader.Read(@"C:\Test for Autocad greier\P-10001\DWG\P-10001 2.etg.dwg");

Console.WriteLine(Config.LayoutReaderScript);


var dB = DB.Instance;

Console.WriteLine("Publis test");
Console.WriteLine();
Console.WriteLine(DB.DWGs[4].ToString());
Console.WriteLine();
LayoutPublisher.Publish(DB.DWGs[4].FilePath, DB.DWGs[4].Layouts);
Console.WriteLine("Publis test slutt");

// Testing over

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins",
            builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    });
}

// Add services to the container.

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
    app.UseCors("AllowAllOrigins");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

