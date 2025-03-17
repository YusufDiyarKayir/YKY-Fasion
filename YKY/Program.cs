using Microsoft.AspNetCore.Session;

var builder = WebApplication.CreateBuilder(args);

// Session middleware'i ekliyoruz
builder.Services.AddDistributedMemoryCache(); // Oturum için bellek tabanlý cache kullanýyoruz
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturumun süresi (30 dakika)
    options.Cookie.HttpOnly = true; // Sadece HTTP istekleri üzerinden eriþilebilir olacak
    options.Cookie.IsEssential = true; // Cookie'nin temel olmasýný saðlýyoruz
});

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseSession(); // Session middleware'ini kullanýma alýyoruz

// Diðer middleware'ler
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();
