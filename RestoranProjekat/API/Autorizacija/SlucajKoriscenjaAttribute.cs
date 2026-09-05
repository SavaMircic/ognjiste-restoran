namespace API.Autorizacija;

[AttributeUsage(AttributeTargets.Method)]
public class SlucajKoriscenjaAttribute : Attribute
{
    public string Naziv { get; }
    public string[] DozvoljeneUloge { get; }

    public SlucajKoriscenjaAttribute(string naziv, params string[] dozvoljeneUloge)
    {
        Naziv = naziv;
        DozvoljeneUloge = dozvoljeneUloge;
    }
}
