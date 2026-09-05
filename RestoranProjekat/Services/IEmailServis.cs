namespace Services;

public interface IEmailServis
{
    Task PosaljiAsync(string naAdresu, string naslov, string sadrzajHtml);

    Task PosaljiBezPrekidaAsync(string naAdresu, string naslov, string sadrzajHtml);
}
