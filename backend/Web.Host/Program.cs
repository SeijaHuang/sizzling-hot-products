using Microsoft.AspNetCore.Mvc;
using Web.Host.Interfaces;
using Web.Host.Models.Responses;
using Web.Host.Services;

namespace Web.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<Filters.GlobalExceptionFilter>();
            });

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var error = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .Select(e => new
                        {
                            Field = e.Key,
                            Message = e.Value!.Errors.First().ErrorMessage
                        })
                        .FirstOrDefault();

                    var response = new ClientResponse<object>
                    {
                        Success = false,
                        Error = new ClientError
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = $"Invalid request: {error?.Field} - {error?.Message}"
                        }
                    };
                    return new BadRequestObjectResult(response);
                };
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IFileReaderService, FileReaderService>();
            builder.Services.AddScoped<IProductService, ProductService>();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
