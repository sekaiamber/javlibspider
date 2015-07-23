using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sekaiav.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }
        public string JL_Id { get; set; }
        public DateTime Update { get; set; }
        public string Version { get; set; }
        public DateTime Create { get; set; }
    }
}
