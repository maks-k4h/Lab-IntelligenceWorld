using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Lab2_IntelligenceAgencies.Models;

[Display(Name = "Стаціонарний співробітник")]
public class StationaryWorker
{
    [Display(Name = "Ідентифікатор співробітника")]
    [Remote("CheckId", "StationaryWorkers")]
    public int Id { get; set; }
    
    [Display(Name = "Ідентифікатор департаменту")]
    public int DepartmentId { get; set; }
    
    [Display(Name = "Співробітник")]
    public AgencyWorker? AgencyWorker { get; set; }
    [Display(Name = "Департамент")]
    public Department? Department { get; set; }
}