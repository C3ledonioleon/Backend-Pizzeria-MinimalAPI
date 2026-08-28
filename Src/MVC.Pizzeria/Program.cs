using MVC.Pizzeria.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Session para mantener el carrito
builder.Services.AddSession();

// Conexión con la API
builder.Services.AddHttpClient("PizzeriaAPI", client =>
{
client.BaseAddress = new Uri("http://localhost:5260");
});

// Registrar ApiService para la inyección de dependencias
builder.Services.AddScoped<ApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
app.UseExceptionHandler("/Home/Error");
app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// Activar Session
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}")
.WithStaticAssets();

app.Run();
