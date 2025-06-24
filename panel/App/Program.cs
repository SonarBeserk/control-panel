using App;
using Htmx.TagHelpers;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register AppSettings
builder.Services.AddOptions<AppSettings>()
    .BindConfiguration(string.Empty)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Delegate the settings resolver so the IOptions call is not required
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<AppSettings>>().Value);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();
app.MapHtmxAntiforgeryScript();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
