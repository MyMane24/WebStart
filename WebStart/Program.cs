
using Microsoft.EntityFrameworkCore;
using WebStart.Data;
using WebStart.Services;


//using WebStart.Data;
//using WebStart.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebStart
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<WebStartContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("WebStart"));
            });
            builder.Services.AddTransient<IUserService, UserService>();
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
            app.UseCors("cors");    app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}




