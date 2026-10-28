
using InventoryManagement.Data;

var builder = WebApplication.CreateBuilder(args);

// Register MVC services
builder.Services.AddControllersWithViews();

// Register the inventory database service
builder.Services.AddSingleton<InventoryDatabase>();

var app = builder.Build();

// Create the database and Products table when the application starts
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<InventoryDatabase>();
    await database.InitializeAsync();
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inventory}/{action=Index}/{id?}");

app.Run();
