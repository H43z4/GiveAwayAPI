using Models.DatabaseModels.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DatabaseModels.DSSDatabaseObjects.Setup
{
    public class DSUserRole : BaseModel
    {
        [Key]
        public long UserRoleId { get; set; }

        [ForeignKey("AssociatedUser")]
        public long UserId { get; set; }
        public virtual DSUser AssociatedUser { get; set; }

        [ForeignKey("Role")]
        public long RoleId { get; set; }
        public virtual DSRole Role { get; set; }

        //public bool IsTimeBased { get; set; }

        //public DateTime? ExpiryDateTime { get; set; }
    }
}
