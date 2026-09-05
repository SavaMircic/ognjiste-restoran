using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using API.Autorizacija;
using API.Filteri;
using API.Greske;
using API.Hubs;
using API.Notifikacije;
using API.Poslovi;
using API.Slike;
using Domain.Entiteti;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository;
using Repository.Interfejsi;
using Services;
using Services.Implementation;

var builder = WebApplication.CreateBuilder(args);

string ObavezanoPodesavanje(string kljuc)
{
    var vrednost = builder.Configuration[kljuc];
    if (string.IsNullOrWhiteSpace(vrednost))
        throw new InvalidOperationException(
            $"Podešavanje '{kljuc}' nije postavljeno.{Environment.NewLine}" +
            $"Ako je tajna, postavi je komandom:{Environment.NewLine}" +
            $"  dotnet user-secrets set \"{kljuc}\" \"<vrednost>\" --project API{Environment.NewLine}" +
            $"Ako nije tajna, upiši je u API/appsettings.json.");
    return vrednost;
}

var konekcioniString = ObavezanoPodesavanje("ConnectionStrings:RestoranBaza");
builder.Services.AddDbContext<RestoranDbContext>(opcije =>
    opcije.UseSqlServer(konekcioniString));

builder.Services.AddIdentity<Korisnik, IdentityRole>(opcije =>
    {
        opcije.Password.RequireDigit = true;
        opcije.Password.RequireUppercase = true;
        opcije.Password.RequireNonAlphanumeric = false;
        opcije.Password.RequiredLength = 8;
        opcije.User.RequireUniqueEmail = true;
        opcije.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<RestoranDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(opcije =>
    {
        opcije.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opcije.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opcije =>
    {
        opcije.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Izdavac"],
            ValidAudience = builder.Configuration["Jwt:Publika"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ObavezanoPodesavanje("Jwt:Kljuc")))
        };

        opcije.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(token) && context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                    context.Token = token;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AutorizacijaRezultatHandler>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(BazniRepository<>));
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IKorisnikAdminRepository, KorisnikAdminRepository>();
builder.Services.AddScoped<IZaposleniRepository, ZaposleniRepository>();
builder.Services.AddScoped<IKategorijaMenijaRepository, KategorijaMenijaRepository>();
builder.Services.AddScoped<IStavkaMenijaRepository, StavkaMenijaRepository>();
builder.Services.AddScoped<IReceptureRepository, ReceptureRepository>();
builder.Services.AddScoped<IStoRepository, StoRepository>();
builder.Services.AddScoped<IPorudzbinaRepository, PorudzbinaRepository>();
builder.Services.AddScoped<IStavkaPorudzbineRepository, StavkaPorudzbineRepository>();
builder.Services.AddScoped<IRezervacijaRepository, RezervacijaRepository>();
builder.Services.AddScoped<IRecenzijaRepository, RecenzijaRepository>();
builder.Services.AddScoped<ILajkJelaRepository, LajkJelaRepository>();
builder.Services.AddScoped<IPorukaRepository, PorukaRepository>();
builder.Services.AddScoped<INamirnicaRepository, NamirnicaRepository>();
builder.Services.AddScoped<ISmenaRepository, SmenaRepository>();
builder.Services.AddScoped<IBonusRepository, BonusRepository>();
builder.Services.AddScoped<IIzvestajRepository, IzvestajRepository>();
builder.Services.AddScoped<IPostavkeRepository, PostavkeRepository>();
builder.Services.AddScoped<IGalerijaRepository, GalerijaRepository>();
builder.Services.AddScoped<IAdminAkcijaRepository, AdminAkcijaRepository>();
builder.Services.AddScoped<IPrijavaProblemaRepository, PrijavaProblemaRepository>();

builder.Services.AddScoped<IAuthServis, AuthServis>();
builder.Services.AddScoped<IKorisnikServis, KorisnikServis>();
builder.Services.AddScoped<IEmailServis, EmailServis>();
builder.Services.AddScoped<IAuditLogServis, AuditLogServis>();
builder.Services.AddScoped<IKorisnikAdminServis, KorisnikAdminServis>();
builder.Services.AddScoped<IZaposleniServis, ZaposleniServis>();
builder.Services.AddScoped<IKategorijaMenijaServis, KategorijaMenijaServis>();
builder.Services.AddScoped<IStavkaMenijaServis, StavkaMenijaServis>();
builder.Services.AddScoped<IReceptureServis, ReceptureServis>();
builder.Services.AddScoped<IStoServis, StoServis>();
builder.Services.AddScoped<IPorudzbinaServis, PorudzbinaServis>();
builder.Services.AddScoped<IStavkaPorudzbineServis, StavkaPorudzbineServis>();
builder.Services.AddScoped<INotifikacijaServis, SignalRNotifikacijaServis>();
builder.Services.AddScoped<IRezervacijaServis, RezervacijaServis>();
builder.Services.AddScoped<IRecenzijaServis, RecenzijaServis>();
builder.Services.AddScoped<ILajkJelaServis, LajkJelaServis>();
builder.Services.AddScoped<IPorukaServis, PorukaServis>();
builder.Services.AddScoped<INamirnicaServis, NamirnicaServis>();
builder.Services.AddScoped<ISmenaServis, SmenaServis>();
builder.Services.AddScoped<IBonusServis, BonusServis>();
builder.Services.AddScoped<IIzvestajServis, IzvestajServis>();
builder.Services.AddScoped<IPostavkeServis, PostavkeServis>();
builder.Services.AddScoped<IGalerijaServis, GalerijaServis>();
builder.Services.AddScoped<IAdminAkcijaServis, AdminAkcijaServis>();
builder.Services.AddScoped<IPrijavaProblemaServis, PrijavaProblemaServis>();
builder.Services.AddScoped<IDashboardServis, DashboardServis>();

builder.Services.AddSignalR();

builder.Services.AddScoped<ISkladisteSlika, LokalnoSkladisteSlika>();

builder.Services.AddHostedService<IstekRezervacijaPosao>();

builder.Services.AddValidatorsFromAssemblyContaining<Services.Validatori.RegistracijaDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

ValidatorOptions.Global.LanguageManager = new API.Validacija.SrpskePorukeValidacije();

builder.Services.AddControllers(opcije =>
{
    opcije.Filters.Add<AuditLogActionFilter>();
    opcije.Filters.Add<ValidacijaActionFilter>();
    opcije.Filters.Add<SlucajKoriscenjaAutorizacijaFilter>();

    var poruke = opcije.ModelBindingMessageProvider;
    poruke.SetAttemptedValueIsInvalidAccessor((vrednost, polje) => $"Vrednost '{vrednost}' nije ispravna za polje {polje}.");
    poruke.SetNonPropertyAttemptedValueIsInvalidAccessor(vrednost => $"Vrednost '{vrednost}' nije ispravna.");
    poruke.SetValueIsInvalidAccessor(vrednost => $"Vrednost '{vrednost}' nije ispravna.");
    poruke.SetUnknownValueIsInvalidAccessor(polje => $"Poslata vrednost za polje {polje} nije ispravna.");
    poruke.SetNonPropertyUnknownValueIsInvalidAccessor(() => "Poslata vrednost nije ispravna.");
    poruke.SetValueMustBeANumberAccessor(polje => $"Polje {polje} mora biti broj.");
    poruke.SetNonPropertyValueMustBeANumberAccessor(() => "Vrednost mora biti broj.");
    poruke.SetMissingBindRequiredValueAccessor(polje => $"Vrednost za polje {polje} nije poslata.");
    poruke.SetMissingRequestBodyRequiredValueAccessor(() => "Telo zahteva je obavezno.");
    poruke.SetValueMustNotBeNullAccessor(vrednost => $"Vrednost '{vrednost}' ne sme biti prazna.");
})
    .AddJsonOptions(opcije => opcije.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.Configure<ApiBehaviorOptions>(opcije =>
{
    opcije.InvalidModelStateResponseFactory = NeispravanUnosOdgovor.Napravi;
});

builder.Services.AddExceptionHandler<GlobalniHendlerGresaka>();
builder.Services.AddProblemDetails();

builder.Services.AddRateLimiter(opcije =>
{
    opcije.AddFixedWindowLimiter(PolitikeOgranicenja.SlanjeMejla, granica =>
    {
        granica.Window = TimeSpan.FromMinutes(15);
        granica.PermitLimit = 5;
        granica.QueueLimit = 0;
    });

    opcije.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    opcije.OnRejected = async (kontekst, token) =>
    {
        var oznaka = kontekst.HttpContext.GetEndpoint()?.Metadata.GetMetadata<SlucajKoriscenjaAttribute>();
        var auditLogServis = kontekst.HttpContext.RequestServices.GetService<IAuditLogServis>();
        if (oznaka != null && auditLogServis != null)
        {
            await auditLogServis.ZabeleziAsync(
                kontekst.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
                oznaka.Naziv, uspesno: false, poruka: "Odbijeno: prekoračen dozvoljen broj zahteva.");
        }

        await kontekst.HttpContext.Response.WriteAsJsonAsync(new
        {
            Poruka = "Previše zahteva. Pokušajte ponovo za nekoliko minuta.",
            Greske = new Dictionary<string, string[]>()
        }, token);
    };
});

builder.Services.AddCors(opcije =>
{
    opcije.AddPolicy("AngularKlijent", politika =>
        politika.WithOrigins(ObavezanoPodesavanje("Frontend:BazniUrl"))
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opcije =>
{
    opcije.SwaggerDoc("v1", new OpenApiInfo { Title = "Restoran API", Version = "v1" });
    opcije.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Unesite samo token, bez reči 'Bearer' — Swagger je sam dodaje."
    });
    opcije.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var kontekst = scope.ServiceProvider.GetRequiredService<RestoranDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Korisnik>>();
    await kontekst.Database.MigrateAsync();
    await PocetniPodaci.PopuniAsync(roleManager, userManager, kontekst);
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = kontekst =>
    {
        kontekst.Context.Response.Headers.CacheControl = "no-cache";
    }
});

app.UseCors("AngularKlijent");
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<KuhinjaHub>("/hubs/kuhinja");

app.Run();
