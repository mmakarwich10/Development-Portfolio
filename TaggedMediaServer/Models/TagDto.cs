﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public int OriginId { get; set; }
        public List<TagDto>? Implications { get; set;}
        public bool IsDeprecated { get; set; }
    }
}
