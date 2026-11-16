
using MVC.Pizzeria.Services;

var builder = WebApplication.CreateBuilder(args);

// SERVICIOS MVC
builder.Services.AddControllersWithViews();

// Acceder al contexto HTTP
builder.Services.AddHttpContextAccessor();

// AUTENTICACIÓN MEDIANTE COOKIES
builder.Services
    .AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Usuario/Login";
        options.LogoutPath = "/Usuario/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// SESSION PARA MANTENER EL CARRITO
builder.Services.AddSession();

// CONEXIÓN CON LA API
builder.Services.AddHttpClient("PizzeriaAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5260");
});

// REGISTRAR APISERVICE
builder.Services.AddScoped<ApiService>();

var app = builder.Build();

// CONFIGURAR PIPELINE
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// SESSION
app.UseSession();

// AUTENTICACIÓN Y AUTORIZACIÓN
app.UseAuthentication();
app.UseAuthorization();

// ARCHIVOS ESTÁTICOS
app.MapStaticAssets();

// RUTAS MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
