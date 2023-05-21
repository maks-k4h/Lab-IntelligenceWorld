using System.ComponentModel.DataAnnotations;

namespace Lab2_IntelligenceAgencies.Models;

[Display(Name = "Участь стаціонарних співробітників у операціях")]
public class StationaryWorkerToOperation
{
    [Display(Name = "Ідентифікатор стац. співробітника")]
    public int StationaryWorkerId { get; set; }
    
    [Display(Name = "Ідентифікатор операції")]
    public int OperationId { get; set; }
    
    [Display(Name = "Ідентифікатор рівня доступу")]
    public int AccessLevelId { get; set; }
    
    [Display(Name = "Стаціонарний співробітник")]
    public StationaryWorker? StationaryWorker { get; set; }
    
    [Display(Name = "Співробітник")]
    public AgencyWorker? AgencyWorker { get; set; }
    
    [Display(Name = "Операція")]
    public Operation? Operation { get; set; }
    
    [Display(Name = "Рівень доступу")]
    public AccessLevel? AccessLevel { get; set; }
}