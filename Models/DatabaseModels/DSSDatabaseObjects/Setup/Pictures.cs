using System.ComponentModel.DataAnnotations.Schema;

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
