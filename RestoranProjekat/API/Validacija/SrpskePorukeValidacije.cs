using System.Globalization;
using FluentValidation.Resources;

namespace API.Validacija;

public class SrpskePorukeValidacije : LanguageManager
{
    private static readonly Dictionary<string, string> Poruke = new()
    {
        ["NotNullValidator"] = "{PropertyName} je obavezno polje.",
        ["NotEmptyValidator"] = "{PropertyName} je obavezno polje.",
        ["MaximumLengthValidator"] = "{PropertyName} može imati najviše {MaxLength} karaktera.",
        ["MinimumLengthValidator"] = "{PropertyName} mora imati bar {MinLength} karaktera.",
        ["LengthValidator"] = "{PropertyName} mora imati između {MinLength} i {MaxLength} karaktera.",
        ["EmailValidator"] = "{PropertyName} nije ispravna imejl adresa.",
        ["GreaterThanValidator"] = "{PropertyName} mora biti veće od {ComparisonValue}.",
        ["GreaterThanOrEqualValidator"] = "{PropertyName} ne može biti manje od {ComparisonValue}.",
        ["LessThanValidator"] = "{PropertyName} mora biti manje od {ComparisonValue}.",
        ["LessThanOrEqualValidator"] = "{PropertyName} ne može biti veće od {ComparisonValue}.",
        ["InclusiveBetweenValidator"] = "{PropertyName} mora biti između {From} i {To}.",
        ["ExclusiveBetweenValidator"] = "{PropertyName} mora biti veće od {From} i manje od {To}.",
        ["EqualValidator"] = "{PropertyName} mora biti jednako {ComparisonValue}.",
        ["NotEqualValidator"] = "{PropertyName} ne sme biti jednako {ComparisonValue}.",
        ["RegularExpressionValidator"] = "{PropertyName} nije u ispravnom formatu.",
        ["PredicateValidator"] = "{PropertyName} nije ispravno.",
        ["AsyncPredicateValidator"] = "{PropertyName} nije ispravno.",
        ["EnumValidator"] = "{PropertyName} nema dozvoljenu vrednost.",
        ["ScalePrecisionValidator"] =
            "{PropertyName} ne sme imati više od {ExpectedPrecision} cifara, od toga najviše {ExpectedScale} decimala.",
        ["CreditCardValidator"] = "{PropertyName} nije ispravan broj kartice.",
        ["NullValidator"] = "{PropertyName} mora biti prazno.",
        ["EmptyValidator"] = "{PropertyName} mora biti prazno.",
    };

    public override string GetString(string key, CultureInfo? culture = null) =>
        Poruke.GetValueOrDefault(key) ?? base.GetString(key, culture);
}
