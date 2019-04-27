using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiSQL.Models
{
    public class Información
    {
        public int num { get; set; }
        [StringLength(100)]
        public string varchar { get; set; }
        public DateTime tiempo { get; set; }

        public int num2 { get; set; }
        [StringLength(100)]
        public string varchar2 { get; set; }
        public DateTime tiempo2 { get; set; }

        public int num3 { get; set; }
        [StringLength(100)]
        public string varchar3 { get; set; }
        public DateTime tiempo3 { get; set; }
    }
}