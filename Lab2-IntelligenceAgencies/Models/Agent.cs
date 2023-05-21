using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Lab2_IntelligenceAgencies.Models;

[Display(Name = "Агент")]
public class Agent
{
    [Display(Name = "Ідентифікатор співробітника")]
    [Remote("CheckId", "Agents")]
    public int Id { get; set; }
    
    [Display(Name = "Співробітник")]
    public AgencyWorker? AgencyWorker { get; set; }
}