using Microsoft.EntityFrameworkCore;
using SampleApp.Domen.Models;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSassCompiler();

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SampleAppContext>(options => options.UseSqlServer(connection));
builder.Services.AddFlashes();
//builder.Services.AddScoped<HttpClient>();
builder.Services.AddHttpClient("API", o => o.BaseAddress = new Uri("https://localhost:7225/api"));


builder.Services.AddSession(options =>
{
    options.Cookie.Name = "SampleSession";
    //options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.UseSession();


app.Run();
