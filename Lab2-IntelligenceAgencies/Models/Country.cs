using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Lab2_IntelligenceAgencies.Models;

[Display(Name = "Країна")]
public class Country
{
    [Display(Name = "Ідентифікатор")]
    public int Id { get; set; }

    [Display(Name = "Назва")]
    [MinLength(2, ErrorMessage = "Назва надто коротка")]
    
    [Remote("CheckName", "Countries")]
    public string Name { get; set; } = null!;
}

