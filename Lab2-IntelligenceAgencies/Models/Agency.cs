using System.ComponentModel.DataAnnotations;

namespace Lab2_IntelligenceAgencies.Models;

[Display(Name = "Агенція")]
public class Agency
{
    [Display(Name = "Ідентифікатор")]
    public int Id { get; set; }
    
    [Display(Name = "Назва")]
    public string Name { get; set; }
    
    [Display(Name = "Головний відділ")]
    public string? Headquarters { get; set; }
    
    [Display(Name = "Опис")]
    public string? Description { get; set; }
}