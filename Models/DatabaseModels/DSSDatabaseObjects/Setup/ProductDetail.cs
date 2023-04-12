using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DatabaseModels.DSSDatabaseObjects.Setup
{
    public class ProductDetail : BaseModel
    {
        [Key]
        public long ProductDetailId { get; set; }

        [ForeignKey("Product")]
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }

        [ForeignKey("ProductType")]
        public long? ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }

        [ForeignKey("ProductSize")]
        public long? ProductSizeId { get; set; }
        public virtual ProductSize ProductSize { get; set; }

        public long? ProductQuantity { get; set; }

        public long? ProductRetailPrice { get; set; }
        
        [StringLength(10)]
        public string ProductStrength { get; set; }

        public DateTime? ProductPriceEffectiveDate { get; set; }

        public float ProductVendFee { get; set; }




    }
}
