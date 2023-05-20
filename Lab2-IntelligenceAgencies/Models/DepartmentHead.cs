using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Lab2_IntelligenceAgencies.Models;

[Display(Name = "Голова департаменту")]
public class DepartmentHead
{
    [Display(Name = "Ідентифікатор співробітника")]
    [Remote("CheckId", "DepartmentHeads")]
    public int Id { get; set; }
    
    [Display(Name = "Ідентифікатор департаменту")]
    [Remote("CheckDepartmentId", "DepartmentHeads")]
    public int DepartmentId { get; set; }
    
    [Display(Name = "Співробітник")]
    public AgencyWorker? AgencyWorker { get; set; }
    [Display(Name = "Департамент")]
    public Department? Department { get; set; }
}