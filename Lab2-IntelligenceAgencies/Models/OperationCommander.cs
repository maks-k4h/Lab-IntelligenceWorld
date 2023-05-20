using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Lab2_IntelligenceAgencies.Models;

[Display(Name = "Командир операції")]
public class OperationCommander
{
    [Display(Name = "Ідентифікатор співробітника")]
    [Remote("CheckId", "OperationCommanders")]
    public int Id { get; set; }
    
    [Display(Name = "Ідентифікатор операції")]
    [Remote("CheckOperationId", "OperationCommanders")]
    public int OperationId { get; set; }
    
    [Display(Name = "Співробітник")]
    public AgencyWorker? AgencyWorker { get; set; }
    [Display(Name = "Операція")]
    public Operation? Operation { get; set; }
}