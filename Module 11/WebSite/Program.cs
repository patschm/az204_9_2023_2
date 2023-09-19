
namespace WebSite;

public class Program
{
    private static string baseAddress = "https://ps-dataservice.azurewebsites.net";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddApplicationInsightsTelemetry(opts => {
             //opts.ConnectionString = builder.Configuration["APPINSIGHTS_CONNECTIONSTRING"]; 
        });

        builder.Services.AddHttpClient("data", opts => {
            opts.BaseAddress = new Uri($"{baseAddress}");
        }).SetHandlerLifetime(TimeSpan.FromMinutes(10)); ;

        builder.Services.AddControllersWithViews();

        var app= builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseStaticFiles();
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
        app.Run();
    }
}
