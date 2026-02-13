using System.ComponentModel.DataAnnotations;

namespace WorshipManager.Core.Entities;

public class MessageTemplate
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Template { get; set; } = string.Empty;

    public string Content
    {
        get => Template;
        set => Template = value;
    }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
