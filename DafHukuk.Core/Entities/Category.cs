using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DafHukuk.Core.Entities
{
    public class Category : BaseEntity
    {
        public string Name_TR { get; set; }
        public string Name_EN { get; set; }
        public string Name_AR { get; set; }


        public ICollection<Post> Posts { get; set; }
    }
}
