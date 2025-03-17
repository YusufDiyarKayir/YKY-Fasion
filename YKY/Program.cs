using Microsoft.AspNetCore.Session;

var builder = WebApplication.CreateBuilder(args);

// Session middleware'i ekliyoruz
builder.Services.AddDistributedMemoryCache(); // Oturum i�in bellek tabanl� cache kullan�yoruz
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturumun s�resi (30 dakika)
    options.Cookie.HttpOnly = true; // Sadece HTTP istekleri �zerinden eri�ilebilir olacak
    options.Cookie.IsEssential = true; // Cookie'nin temel olmas�n� sa�l�yoruz
});

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseSession(); // Session middleware'ini kullan�ma al�yoruz

// Di�er middleware'ler
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
