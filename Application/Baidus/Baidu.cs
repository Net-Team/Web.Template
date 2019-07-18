using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Baidus
{
    public  class Baidu
    {
        [Required]
        public string Id { get; set; }
       
        public int Age { get; set; }

        public string Name { get; set; }
    }
}
