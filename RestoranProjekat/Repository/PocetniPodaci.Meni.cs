using Domain.Entiteti;
using Domain.Enumi;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public static partial class PocetniPodaci
{
    private static async Task PopuniMenijAsync(RestoranDbContext kontekst)
    {
        var kategorije = new[]
        {
            new KategorijaMenija { Naziv = "Predjela", Opis = "Za sto, dok se glavno jelo sprema", Redosled = 1, Odrediste = Odrediste.Kuhinja },
            new KategorijaMenija { Naziv = "Supe i čorbe", Opis = "Od jutros i od kostiju — ne od kockice", Redosled = 2, Odrediste = Odrediste.Kuhinja },
            new KategorijaMenija { Naziv = "Glavna jela", Opis = "Domaća kuhinja, porcije za pravu glad", Redosled = 3, Odrediste = Odrediste.Kuhinja },
            new KategorijaMenija { Naziv = "Roštilj", Opis = "Sa žara, po porudžbini — nikad unapred", Redosled = 4, Odrediste = Odrediste.Kuhinja },
            new KategorijaMenija { Naziv = "Dezerti", Opis = "Pravimo ih sami, svakog jutra", Redosled = 5, Odrediste = Odrediste.Kuhinja },
            new KategorijaMenija { Naziv = "Pića", Opis = "Sa šanka — topla i hladna", Redosled = 6, Odrediste = Odrediste.Sank }
        };

        foreach (var kategorija in kategorije)
        {
            if (!await kontekst.KategorijeMenija.AnyAsync(k => k.Naziv == kategorija.Naziv))
                kontekst.KategorijeMenija.Add(kategorija);
        }
        await kontekst.SaveChangesAsync();

        var poNazivu = await kontekst.KategorijeMenija.ToDictionaryAsync(k => k.Naziv, k => k.Id);

        var jela = new[]
        {
            Jelo("Šopska salata", "Paradajz, krastavac, feta sir i crni luk, prelivena maslinovim uljem.", 350, "sopska-salata", "Predjela", poNazivu),
            Jelo("Srpska salata", "Paradajz, luk i ljuta paprika — bez sira, za one koji vole čist ukus.", 320, "srpska-salata", "Predjela", poNazivu),
            Jelo("Kajmak i proja", "Domaći kajmak uz toplu proju sa čvarcima.", 480, "kajmak-i-proja", "Predjela", poNazivu),

            Jelo("Pileća supa", "Domaća supa sa rezancima i korenastim povrćem.", 250, "pileca-supa", "Supe i čorbe", poNazivu),
            Jelo("Riblja čorba", "Kuvana od tri vrste rečne ribe, ljuta po želji.", 420, "riblja-corba", "Supe i čorbe", poNazivu, popust: 10),

            Jelo("Karađorđeva šnicla", "Punjena kajmakom, panirana, uz pomfrit i tartar sos.", 950, "karadjordjeva-snicla", "Glavna jela", poNazivu),
            Jelo("Punjena piletina", "File punjen suvim mesom i kačkavaljem, uz grilovano povrće.", 880, "punjena-piletina", "Glavna jela", poNazivu),
            Jelo("Pastrmka na žaru", "Sveža pastrmka sa blitvom i krompirom.", 1100, "pastrmka-na-zaru", "Glavna jela", poNazivu, dostupno: false),
            Jelo("Đuveč", "Posno jelo od pirinča i povrća, kuvano u rerni.", 620, "djuvec", "Glavna jela", poNazivu, popust: 15),
            Jelo("Sarma", "Kiseli kupus punjen mlevenim mesom i pirinčem, krčkana sa suvim rebrima.", 780, "sarma", "Glavna jela", poNazivu),

            Jelo("Ćevapi (10 kom)", "Sa lepinjom, kajmakom i sitno seckanim lukom.", 700, "cevapi", "Roštilj", poNazivu),
            Jelo("Pljeskavica", "Punjena kačkavaljem, uz lepinju i ajvar.", 650, "pljeskavica", "Roštilj", poNazivu),
            Jelo("Mešano meso", "Ćevapi, pljeskavica, vešalica i kobasica — porcija za dvoje.", 1250, "mesano-meso", "Roštilj", poNazivu, popust: 20),

            Jelo("Palačinke sa Nutellom", "Dve palačinke, šlag po želji.", 400, "palacinke-nutella", "Dezerti", poNazivu, popust: 10),
            Jelo("Baklava", "Domaća, sa orasima i medom.", 380, "baklava", "Dezerti", poNazivu),
            Jelo("Krempita", "Klasična, sa hrskavim listovima.", 320, "krempita", "Dezerti", poNazivu),

            Jelo("Coca Cola 0.33l", "Rashlađena.", 200, "coca-cola", "Pića", poNazivu),
            Jelo("Espresso", "Jedna doza, italijanska mešavina.", 150, "espresso", "Pića", poNazivu),
            Jelo("Domaća limunada", "Sveže ceđen limun, nana i med.", 280, "domaca-limunada", "Pića", poNazivu)
        };

        foreach (var jelo in jela)
        {
            if (jelo != null && !await kontekst.StavkeMenija.AnyAsync(s => s.Naziv == jelo.Naziv))
                kontekst.StavkeMenija.Add(jelo);
        }
        await kontekst.SaveChangesAsync();

        await PopuniDetaljneOpiseAsync(kontekst);
        await PopuniDodatneSlikeAsync(kontekst);
    }

    private static readonly Dictionary<string, string> DetaljniOpisi = new()
    {
        ["sopska-salata"] =
            "Paradajz uzimamo od dobavljača iz Futoga, od juna do septembra; van sezone je " +
            "plastenički, pa to i kažemo. Krastavac se ljušti na trake, a crni luk seče na tanko " +
            "i kratko drži u hladnoj vodi da izgubi oštrinu. Feta se ne meša u salatu nego renda " +
            "odozgo, tik pred iznošenje, da ostane suva i da se oseti zasebno. Preliv je samo " +
            "maslinovo ulje, so i malo vinskog sirćeta — bez majoneze i bez šećera.",

        ["srpska-salata"] =
            "Ista osnova kao šopska, ali bez sira — za goste koji uz jelo sa žara ne žele još " +
            "jednu masnu komponentu. Ljuta paprika ide sveža, na kolutove, i može da se izostavi. " +
            "Ovo je salata koju stalni gosti najčešće naručuju uz mešano meso, jer čisti nepce " +
            "između zalogaja umesto da se takmiči sa jelom.",

        ["kajmak-i-proja"] =
            "Kajmak nabavljamo sa Zlatibora, star oko tri nedelje — mlad je blag i izgubi se uz " +
            "proju, stariji ima onu slanu oštrinu koja se pamti. Proja se peče svakog jutra, sa " +
            "čvarcima i kiselim mlekom u testu, i iznosi se topla. Ako dođete posle 22h, moguće " +
            "je da je ostala samo jučerašnja — reći ćemo vam pre nego što poručite.",

        ["pileca-supa"] =
            "Kuva se od celog pileta, ne od kockice i ne od belog mesa: kosti daju ono što supu " +
            "drži zajedno. Krčka se od šest ujutru, tri sata na tihoj vatri, sa šargarepom, " +
            "peršunovim korenom i celerom. Rezanci se prave u kući i ubacuju u tanjir, ne u " +
            "lonac — tako ne upiju svu tečnost dok stoje. Kad se dnevna količina potroši, nema " +
            "je do sutra.",

        ["riblja-corba"] =
            "Od tri vrste rečne ribe — šaran, som i smuđ — kuvana onako kako je uobičajeno uz " +
            "Dunav: riba se ne cedi kroz sito nego ostaje u čorbi. Aleva paprika ide na kraju, " +
            "van vatre, da ne zagorči. Ljuto je podrazumevano blago; recite konobaru ako želite " +
            "jače, dodaje se u tanjir. Uz čorbu ide parče domaćeg hleba, bez naplate.",

        ["karadjordjeva-snicla"] =
            "Teleći but se otvara u jedan list, puni kajmakom i uvija — bez šunke i bez " +
            "kačkavalja, kako je i zamišljena. Panira se u prezlama koje meljemo sami, pa je kora " +
            "grublja i ostaje hrskava duže. Porcija je oko 350 grama i iskreno je velika za jednu " +
            "osobu. Tartar sos pravimo u kući, sa kiselim krastavcima i kaparima.",

        ["punjena-piletina"] =
            "File se puni suvim vratom i mladim kačkavaljem, pa peče u rerni, ne u tiganju — meso " +
            "ostane sočno, a punjenje se ne izlije. Povrće sa žara menja se po sezoni: preko leta " +
            "tikvice i paprika, zimi koren i pečurke. Lakše je od Karađorđeve, pa je čest izbor " +
            "za ručak kad se posle radi.",

        ["pastrmka-na-zaru"] =
            "Kalifornijska pastrmka iz uzgajališta na Uvcu, stiže dva puta nedeljno i drži se na " +
            "ledu, nikad zamrznuta. Peče se cela, na žaru, sa limunom i ruzmarinom u trbuhu, i " +
            "iznosi se sa kožom — koja je najbolji deo. Uz nju idu blitva i krompir sa belim " +
            "lukom. Kad je nema u dostavi, radije je skinemo sa menija nego da vam damo smrznutu.",

        ["djuvec"] =
            "Posno jelo koje se kuva u zemljanom sudu, u rerni, sat i po. Pirinač, paprika, " +
            "paradajz, patlidžan i tikvice — sve na krupno, jer đuveč nije rižoto i ne treba da " +
            "se raspadne. Bez mesa i bez masti životinjskog porekla, pa je i za posne dane. " +
            "Podgrejan je lošiji nego svež, zato ga i pravimo u manjim šaržama.",

        ["sarma"] =
            "Kupus kiselimo sami, u kaci, od oktobra — kupovni list je tanak i puca pri uvijanju. " +
            "Fil je mešavina junetine i svinjetine sa pirinčem, bez jaja, jer jaje stegne nadev i " +
            "sarma posle bude gumena. Krčka se tri i po sata na tihoj vatri, sa suvim rebrima na " +
            "dnu lonca i lovorom. Ovo je jedino jelo na meniju koje je bolje sutradan, pa ga " +
            "kuvamo dan ranije i to ne krijemo.",

        ["cevapi"] =
            "Mešavina junetine i jagnjetine, melje se svakog jutra i odstoji hladna do večeri — " +
            "bez toga se ne vežu kako treba. Deset komada, sa lepinjom koja upija sok sa žara, " +
            "kajmakom i sitno seckanim crnim lukom. Peku se na bukovom drvetu, ne na briketima; " +
            "razlika se oseti u dimu. Ako volite pečenije, recite pre nego što odu na žar.",

        ["pljeskavica"] =
            "Isto meso kao za ćevape, samo drugačije oblikovano i punjeno kačkavaljem koji se " +
            "istopi do trenutka kad stigne na sto. Oko 250 grama, na lepinji, sa ajvarom koji " +
            "pečemo i teglimo u septembru za celu godinu. Ne pritiskamo je lopaticom na žaru — " +
            "time bi iscurelo upravo ono zbog čega se i puni.",

        ["mesano-meso"] =
            "Ćevapi, pljeskavica, vešalica i domaća kobasica na jednom tanjiru, sa pomfritom i " +
            "ajvarom sa strane. Deklarisano je kao porcija za dvoje i to nije marketing — oko " +
            "600 grama mesa. Najnaručivanije je jelo u kući i jedino koje sa žara ide u dva " +
            "navrata, da poslednji komad ne stigne hladan.",

        ["palacinke-nutella"] =
            "Testo se pravi po porudžbini, ne stoji u frižideru: palačinka koja je odstajala je " +
            "gumena i to se nadevom ne može sakriti. Dva komada, sa Nutellom, i šlagom ako " +
            "želite — šlag tučemo sami, bez sprejeva. Traje oko petnaest minuta, pa ih poručite " +
            "kad konobar donese glavno jelo, ne posle.",

        ["baklava"] =
            "Kore razvlačimo u kući, orasi se melju krupno da se oseti zalogaj, a agda je od meda " +
            "i limuna, ne od samog šećera. Preliva se hladna preko tople baklave — obrnuto se " +
            "razmekša. Odstoji preko noći pre nego što izađe na meni, pa je ono što jedete danas " +
            "pečeno juče. Jedini je kolač kod nas gde je to bolje.",

        ["krempita"] =
            "Klasična novosadska, sa dva reda lisnatog testa i kremom koji se kuva na pari. " +
            "Gornja kora mora da pukne kad se pritisne viljuškom — ako se savija, nije naša. " +
            "Seče se na porcije od deset centimetara, što je više nego što većina očekuje. Ide " +
            "uz espresso bolje nego uz bilo šta drugo.",

        ["coca-cola"] =
            "Limenka od 0.33l, rashlađena, sa ledom i kriškom limuna ako želite. Držimo i " +
            "Coca-Cola Zero — pitajte konobara, nije zasebno na karti.",

        ["espresso"] =
            "Italijanska mešavina, 80% arabika, pržena u Novom Sadu i nikad starija od tri " +
            "nedelje od datuma prženja. Melje se po šoljici. Jedna doza je 7 grama, ekstrakcija " +
            "oko 25 sekundi. Ako vam je kiselo ili gorko, recite — podešavanje mlina je naš " +
            "posao, ne vaš.",

        ["domaca-limunada"] =
            "Sveže ceđen limun, nana, med i gazirana voda — bez sirupa, bez koncentrata i bez " +
            "veštačkih zaslađivača. Pravi se po porudžbini, pa nije trenutna. Preko leta se " +
            "prodaje bolje od svih gaziranih pića zajedno, i po tome je Sanja sa šanka poznata."
    };

    private static async Task PopuniDetaljneOpiseAsync(RestoranDbContext kontekst)
    {
        var bezOpisa = await kontekst.StavkeMenija
            .Where(s => s.DetaljanOpis == null)
            .ToListAsync();

        if (bezOpisa.Count == 0) return;

        var promenjeno = false;

        foreach (var jelo in bezOpisa)
        {
            var slug = Path.GetFileNameWithoutExtension(jelo.SlikaUrl);
            if (!DetaljniOpisi.TryGetValue(slug, out var opis)) continue;

            jelo.DetaljanOpis = opis;
            promenjeno = true;
        }

        if (promenjeno) await kontekst.SaveChangesAsync();
    }

    private static StavkaMenija? Jelo(
        string naziv, string opis, decimal cena, string slug, string kategorija,
        Dictionary<string, int> kategorije, decimal? popust = null, bool dostupno = true)
    {
        if (!kategorije.TryGetValue(kategorija, out var kategorijaId)) return null;

        return new StavkaMenija
        {
            Naziv = naziv,
            Opis = opis,
            DetaljanOpis = DetaljniOpisi.GetValueOrDefault(slug),
            Cena = cena,
            SlikaUrl = $"/slike/jela/{slug}.jpg",
            KategorijaId = kategorijaId,
            Popust = popust,
            Dostupno = dostupno
        };
    }

    private static async Task PopuniDodatneSlikeAsync(RestoranDbContext kontekst)
    {
        var galerije = new (string Jelo, string Slug, string[] Opisi)[]
        {
            ("Karađorđeva šnicla", "karadjordjeva-snicla", new[] { "Presek kroz šniclu", "Uz pomfrit i tartar", "Sa strane, ceo tanjir" }),
            ("Ćevapi (10 kom)", "cevapi", new[] { "Sa lepinjom i kajmakom", "Izbliza, sa žara", "Porcija za dvoje" }),
            ("Palačinke sa Nutellom", "palacinke-nutella", new[] { "Sa šlagom", "Presek", "Posluženo uz kafu" }),
            ("Mešano meso", "mesano-meso", new[] { "Cela porcija", "Detalj sa žara", "Sa prilozima" }),
            ("Šopska salata", "sopska-salata", new[] { "Odozgo", "Sa fetom izbliza" })
        };

        foreach (var galerija in galerije)
        {
            var jelo = await kontekst.StavkeMenija.FirstOrDefaultAsync(s => s.Naziv == galerija.Jelo);
            if (jelo == null) continue;
            if (await kontekst.SlikeStavkiMenija.AnyAsync(s => s.StavkaMenijaId == jelo.Id)) continue;

            for (var i = 0; i < galerija.Opisi.Length; i++)
            {
                kontekst.SlikeStavkiMenija.Add(new SlikaStavkeMenija
                {
                    StavkaMenijaId = jelo.Id,
                    SlikaUrl = $"/slike/jela/{galerija.Slug}-{i + 1}.jpg",
                    Opis = galerija.Opisi[i],
                    Redosled = i + 1
                });
            }
        }

        await kontekst.SaveChangesAsync();
    }
}
