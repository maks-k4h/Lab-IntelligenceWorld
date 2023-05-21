using System.ComponentModel.DataAnnotations;

namespace Lab2_IntelligenceAgencies.Models;

[Display(Name = "Проведення операцій")]
public class OperationConduction
{
    [Display(Name = "Ідентифікатор країни")]
    public int CountryId { get; set; }
    
    [Display(Name = "Ідентифікатор агенції")]
    public int AgencyId { get; set; }
    
    [Display(Name = "Ідентифікатор операції")]
    public int OperationId { get; set; }

    [Display(Name = "Країна")] 
    public Country? Country { get; set; }
    
    [Display(Name = "Агенція")]
    public Agency? Agency { get; set; }
    
    [Display(Name = "Операці")]
    public Operation? Operation { get; set; }
}