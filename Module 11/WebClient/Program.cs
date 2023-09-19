namespace WebClient;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var currentEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var baseUrl = builder.Configuration["Production:Products:DataServiceUrl"];
        builder.Services.AddHttpClient("brands", options => options.BaseAddress = new Uri($"{baseUrl}/brands/"));
        builder.Services.AddHttpClient("products", options => options.BaseAddress = new Uri($"{baseUrl}/products/"));
        builder.Services.AddHttpClient("productgroups", options => options.BaseAddress = new Uri($"{baseUrl}/productgroups/"));
        builder.Services.AddControllersWithViews();

        var app=builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
        app.Run();
    }
}
