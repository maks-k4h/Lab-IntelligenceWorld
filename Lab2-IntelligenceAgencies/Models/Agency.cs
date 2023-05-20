using System.ComponentModel.DataAnnotations;

namespace Lab2_IntelligenceAgencies.Models;

[Display(Name = "Агенція")]
public class Agency
{
    [Display(Name = "Ідентифікатор")]
    public int Id { get; set; }
    
    [Display(Name = "Назва")]
    [RegularExpression(Constants.TextFieldPattern, ErrorMessage = Constants.TextFieldPatternErrorMessage)]
    public string Name { get; set; }
    
    [Display(Name = "Головний відділ")]
    [RegularExpression(Constants.TextFieldPattern, ErrorMessage = Constants.TextFieldPatternErrorMessage)]
    public string? Headquarters { get; set; }
    
    [Display(Name = "Опис")]
    [RegularExpression(Constants.TextFieldPattern, ErrorMessage = Constants.TextFieldPatternErrorMessage)]
    public string? Description { get; set; }
}