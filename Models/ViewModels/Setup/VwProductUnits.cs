using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Setup
{
    public class VwProductUnits
    {
        public long value { get; set; }
        public string label { get; set; }
        public float measure { get; set; }
        public long leviAmount { get; set; }
    }
}
