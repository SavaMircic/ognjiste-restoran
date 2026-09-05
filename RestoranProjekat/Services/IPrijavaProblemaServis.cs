using Services.DTO;

namespace Services;

public interface IPrijavaProblemaServis
{
    Task<(bool Uspesno, string? Greska, PrijavaProblemaDto? Prijava)> PrijaviAsync(
        string trenutniKorisnikId, PrijaviProblemDto dto);

    Task<(bool Uspesno, string? Greska, List<PrijavaProblemaDto>? Prijave)> MojePrijaveAsync(
        string trenutniKorisnikId);

    Task<PaginiranaListaDto<PrijavaProblemaDto>> PretraziAsync(PrijaveProblemaPretragaDto filter);

    Task<(bool Uspesno, string? Greska)> ResiAsync(
        int id, string trenutniKorisnikId, ResiPrijavuProblemaDto dto);
}
