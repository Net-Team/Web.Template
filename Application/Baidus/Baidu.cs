using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Baidus
{
    public  class Baidu
    {
        [Required]
        [StringLength(50,MinimumLength =10)]
        public string Id { get; set; }
       
        public int Age { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string Name { get; set; }
    }
}
