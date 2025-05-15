
using AnnouncementBoard.Data;
using Microsoft.EntityFrameworkCore;

namespace AnnouncementBoard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<AnnouncementsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection")));
            builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
            builder.Services.AddControllers();
            var mvcAppBaseUrl = builder.Configuration.GetConnectionString("MvcAppBaseUrl");
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMvcApp", builder =>
                {
                    builder.WithOrigins(mvcAppBaseUrl ?? "")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors("AllowMvcApp");

            app.MapControllers();

            app.Run();
        }
    }
}
