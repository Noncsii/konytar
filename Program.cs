using Microsoft.EntityFrameworkCore;
using KonyvtarBackend.Data;
using KonyvtarBackend.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<KonyvtarContext>(options =>
    options.UseInMemoryDatabase("KonyvtarDb"));

builder.Services.AddControllers();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<KonyvtarContext>();
    SeedData(context);
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


void SeedData(KonyvtarContext context)
{
    if (context.Olvasok.Any() || context.Konyvek.Any()) return;

    var o1 = new Olvaso { Nev = "Nagy Ádám", Email = "adam@email.com" };
    var o2 = new Olvaso { Nev = "Kiss Éva", Email = "eva@email.com" };
    context.Olvasok.AddRange(o1, o2);
    context.SaveChanges();

    var k1 = new Konyv { Cim = "Egri Csillagok", Szerzo = "Gárdonyi Géza", ISBN = "9789632345671", KiadasEve = 1901, OlvasoId = o1.Id };
    var k2 = new Konyv { Cim = "A Pál utcai fiúk", Szerzo = "Molnár Ferenc", ISBN = "9789632345672", KiadasEve = 1906, OlvasoId = null };
    var k3 = new Konyv { Cim = "Harry Potter és a bölcsek köve", Szerzo = "J.K. Rowling", ISBN = "9789632345673", KiadasEve = 1997, OlvasoId = null };
    
    context.Konyvek.AddRange(k1, k2, k3);
    context.SaveChanges();
}
