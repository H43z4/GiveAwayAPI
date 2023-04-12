using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DatabaseModels.DSSDatabaseObjects.Setup
{
    public class DSUser : BaseModel
    {
        [Key]
        public long UserId { get; set; }

        
        public long? PersonId { get; set; }
        public long UserStatusId { get; set; }

        
        [StringLength(50)]
        public string FullName { get; set; }

        [Required]
        [StringLength(20)]
        public string UserName { get; set; }

        [Required]
        [StringLength(200)]
        public string Password { get; set; }

        [StringLength(50)]
        public string Designation { get; set; }

        public long OrganizationId { get; set; }
        public long RoleId { get; set; }
        public new long? CreatedBy { get; set; }

        public string CellNo { get; set; }
        public string Email { get; set; }


    }
}
