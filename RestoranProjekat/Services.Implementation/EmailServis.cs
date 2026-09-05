using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Services.Implementation;

public class EmailServis : IEmailServis
{
    private readonly IConfiguration _konfiguracija;
    private readonly ILogger<EmailServis> _log;

    public EmailServis(IConfiguration konfiguracija, ILogger<EmailServis> log)
    {
        _konfiguracija = konfiguracija;
        _log = log;
    }

    public async Task PosaljiAsync(string naAdresu, string naslov, string sadrzajHtml)
    {
        var host = _konfiguracija["Email:Host"];
        var port = int.Parse(_konfiguracija["Email:Port"]!);
        var korisnickoIme = _konfiguracija["Email:KorisnickoIme"];
        var lozinka = _konfiguracija["Email:Lozinka"];
        var posiljalac = _konfiguracija["Email:AdresaPosiljaoca"];

        using var klijent = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(korisnickoIme, lozinka),
            EnableSsl = true
        };

        using var poruka = new MailMessage(posiljalac!, naAdresu, naslov, sadrzajHtml)
        {
            IsBodyHtml = true
        };

        await klijent.SendMailAsync(poruka);
    }

    public async Task PosaljiBezPrekidaAsync(string naAdresu, string naslov, string sadrzajHtml)
    {
        try
        {
            await PosaljiAsync(naAdresu, naslov, sadrzajHtml);
        }
        catch (Exception greska)
        {
            _log.LogWarning(greska, "Slanje emaila na {Adresa} nije uspelo (naslov: {Naslov}).", naAdresu, naslov);
        }
    }
}
