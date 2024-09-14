using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace WebApplication2;

public static class Program
{
    public static WebApplication BuildWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddMvc();
        builder.Services.AddControllers();
        var app = builder.Build();
        return app;
    }

    public static void Configure(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.MapControllers();
        app.UseAuthorization();
    }

    public static int Main(string[] args)
    {
        var app = BuildWebApplication(args);
        Configure(app);
        app.Run();
        return 0;
    }
}
