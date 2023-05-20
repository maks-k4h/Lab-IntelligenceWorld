using System.ComponentModel.DataAnnotations;

namespace Lab2_IntelligenceAgencies.Models;

[Display(Name = "Рівень доступу")]
public class AccessLevel
{
    [Display(Name = "Ідентифікатор")]
    public int Id { get; set; }
    
    [Display(Name = "Ідентифікатор країни")]
    public int CountryId { get; set; }
    
    [Display(Name = "Назва")]
    [RegularExpression(Constants.TextFieldPattern, ErrorMessage = Constants.TextFieldPatternErrorMessage)]
    public string Name { get; set; } = null!;
    
    [Display(Name = "Опис")]
    [RegularExpression(Constants.TextFieldPattern, ErrorMessage = Constants.TextFieldPatternErrorMessage)]
    public string? Description { get; set; }

    [Display(Name = "Країна")]
    [RegularExpression(Constants.TextFieldPattern, ErrorMessage = Constants.TextFieldPatternErrorMessage)]
    public Country? Country { get; set; } = null;
}