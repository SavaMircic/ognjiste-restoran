using Microsoft.AspNetCore.Mvc;

namespace API.Greske;

public static class NeispravanUnosOdgovor
{
    public static IActionResult Napravi(ActionContext kontekst)
    {
        var imenaParametara = kontekst.ActionDescriptor.Parameters
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var unosi = kontekst.ModelState
            .Where(u => u.Value != null && u.Value.Errors.Count > 0)
            .Select(u => (Kljuc: u.Key, Poruke: u.Value!.Errors.Select(g => g.ErrorMessage).ToList()))
            .ToList();

        var konkretne = unosi.Where(u => !imenaParametara.Contains(u.Kljuc)).ToList();
        if (konkretne.Count > 0) unosi = konkretne;

        var greske = new Dictionary<string, string[]>();
        foreach (var unos in unosi)
        {
            var (kljuc, poruke) = Ocisti(unos.Kljuc, unos.Poruke, imenaParametara);
            greske[kljuc] = poruke;
        }

        return new BadRequestObjectResult(new { Poruka = "Neispravan unos.", Greske = greske });
    }

    private static (string Kljuc, string[] Poruke) Ocisti(
        string kljuc, List<string> poruke, HashSet<string> imenaParametara)
    {
        if (kljuc == "$")
            return ("Telo", ["Telo zahteva nije ispravan JSON."]);

        if (kljuc.StartsWith("$."))
        {
            var polje = kljuc[2..];
            return (polje, [$"Vrednost nije ispravna za polje {polje}."]);
        }

        if (imenaParametara.Contains(kljuc))
            return ("Telo", ["Telo zahteva je obavezno."]);

        var ociscene = poruke
            .Select(p => string.IsNullOrWhiteSpace(p) ? "Vrednost nije ispravna." : p)
            .ToArray();
        return (kljuc, ociscene);
    }
}
