using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DafHukuk.Core.Entities
{
    [Table("Lawyers")]
    public class Lawyer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? MobilePhone { get; set; }

        [MaxLength(20)]
        public string? OfficePhone { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }

        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title_TR { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Title_EN { get; set; }

        [MaxLength(255)]
        public string? Title_AR { get; set; }

        [Required]
        [MaxLength(255)]
        public string Slug_TR { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Slug_EN { get; set; }

        [MaxLength(255)]
        public string? Slug_AR { get; set; }

        public string? Biography_TR { get; set; }
        public string? Biography_EN { get; set; }
        public string? Biography_AR { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }
}