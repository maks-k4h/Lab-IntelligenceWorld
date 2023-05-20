using System.ComponentModel.DataAnnotations;

namespace Lab2_IntelligenceAgencies.Models;

[Display(Name = "Співробітник")]
public class AgencyWorker
{
    [Display(Name = "Ідентифікатор")]
    public int Id { get; set; }
    
    [Display(Name = "Повне імʼя")]
    [RegularExpression(Constants.TextFieldPattern, ErrorMessage = Constants.TextFieldPatternErrorMessage)]
    public string FullName { get; set; } = null!;
}