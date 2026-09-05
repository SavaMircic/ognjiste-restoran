namespace Services.Validatori;

public static class DrustveneMreze
{
    public static readonly string[] Facebook = ["facebook.com", "fb.com", "fb.me"];
    public static readonly string[] Instagram = ["instagram.com", "instagr.am"];
    public static readonly string[] LinkedIn = ["linkedin.com", "lnkd.in"];

    public static bool JeLink(string? vrednost, string[] domeni)
    {
        if (string.IsNullOrWhiteSpace(vrednost)) return true;

        if (!Uri.TryCreate(vrednost, UriKind.Absolute, out var uri)) return false;
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) return false;

        var host = uri.Host.ToLowerInvariant();
        return domeni.Any(d => host == d || host.EndsWith("." + d, StringComparison.Ordinal));
    }

    public static string Poruka(string mreza, string domen, string primer) =>
        $"{mreza} link mora voditi na {domen}. Na primer: {primer}";
}
