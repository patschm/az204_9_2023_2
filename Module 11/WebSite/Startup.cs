namespace WebSite;

public class Startup
{
    private string baseAddress = "https://ps-dataservice.azurewebsites.net";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry(opts=> { 
           // opts.ConnectionString = Configuration["APPINSIGHTS_CONNECTIONSTRING"]; 
        });

        services.AddHttpClient("data", opts => {
            opts.BaseAddress = new Uri($"{baseAddress}");
        }).SetHandlerLifetime(TimeSpan.FromMinutes(10)); ;

        services.AddControllersWithViews();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseStaticFiles();
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}
