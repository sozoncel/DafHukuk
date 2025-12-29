using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DafHukuk.Core.Entities
{
    [Table("SolutionPartners")]
    public class SolutionPartner : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        [MaxLength(255)]
        public string? WebsiteUrl { get; set; }

        [Required]
        public string Description_TR { get; set; } = string.Empty;

        public string? Description_EN { get; set; }

        public string? Description_AR { get; set; }

        [Required]
        [MaxLength(255)]
        public string Slug_TR { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Slug_EN { get; set; }

        [MaxLength(255)]
        public string? Slug_AR { get; set; }
    }
}