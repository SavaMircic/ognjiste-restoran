namespace Services.DTO;

public class DatotekaZaUploadDto
{
    public Stream Sadrzaj { get; set; } = Stream.Null;

    public string ImeDatoteke { get; set; } = string.Empty;

    public string TipSadrzaja { get; set; } = string.Empty;
    public long Velicina { get; set; }
}
