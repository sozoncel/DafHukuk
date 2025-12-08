using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DafHukuk.Core.Entities
{
    public class Post : BaseEntity
    {
        public string? CoverImageUrl { get; set; }
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public string? Title_TR { get; set; }
        public string? Content_TR { get; set; }
        public string? Slug_TR { get; set; }

        public string? Title_EN { get; set; }
        public string? Content_EN { get; set; }
        public string? Slug_EN { get; set; }

        public string? Title_AR { get; set; }
        public string? Content_AR { get; set; }
        public string? Slug_AR { get; set; }
    }
}