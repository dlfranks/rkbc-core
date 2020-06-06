using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.core.models
{
    public class Item
    {
        public string Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
