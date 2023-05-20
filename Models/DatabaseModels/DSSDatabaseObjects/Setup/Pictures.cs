using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DatabaseModels.DSSDatabaseObjects.Setup
{
    public class Pictures
    {
        public int Id { get; set; }
        public int PostId { get; set; }

        [Column(TypeName = "varbinary(max)")]
        public byte[] ImageData { get; set; }
    }
}
