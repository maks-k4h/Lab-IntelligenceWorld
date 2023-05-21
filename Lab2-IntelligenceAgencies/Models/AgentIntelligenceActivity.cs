using System.ComponentModel.DataAnnotations;

namespace Lab2_IntelligenceAgencies.Models;

[Display(Name = "Розвідкова діяльність агента")]
public class AgentIntelligenceActivity
{
    [Display(Name = "Ідентифікатор запису")]
    public int Id { get; set; }
    
    [Display(Name = "Ідентифікатор персонажа")]
    public int CoverEntityId { get; set; }
    
    [Display(Name = "Ідентифікатор операції")]
    public int OperationId { get; set; }
    
    [Display(Name = "Ідентифікатор агента")]
    public int AgentId { get; set; }
    
    [Display(Name = "Опис")]
    [RegularExpression(Constants.TextFieldPattern, ErrorMessage = Constants.TextFieldPatternErrorMessage)]
    public string? Description { get; set; }
    
    
    [Display(Name = "Персонаж")]
    public CoverEntity? CoverEntity { get; set; }
    
    [Display(Name = "Операція")]
    public Operation? Operation { get; set; }
    
    [Display(Name = "Агент")]
    public Agent? Agent { get; set; }
    
    [Display(Name = "Працівник агенції")]
    public AgencyWorker? AgencyWorker { get; set; }
}