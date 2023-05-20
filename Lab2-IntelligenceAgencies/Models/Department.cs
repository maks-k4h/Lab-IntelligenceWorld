using System.ComponentModel.DataAnnotations;

namespace Lab2_IntelligenceAgencies.Models;

[Display(Name = "Департамент")]
public class Department
{
    [Display(Name = "Ідентифікатор")]
    public int Id { get; set; }
    
    [Display(Name = "Ідентифікатор агенції")]
    public int AgencyId { get; set; }
    
    [Display(Name = "Назва")]
    [RegularExpression(Constants.TextFieldPattern, ErrorMessage = Constants.TextFieldPatternErrorMessage)]
    public string Name { get; set; }

    [Display(Name = "Агенція")]
    public Agency? Agency { get; set; }
}