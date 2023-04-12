//using Models.DatabaseModels.VehicleRegistration.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DatabaseModels.DSSDatabaseObjects.Core
{
    public class DSPerson : BaseModel
    {
        [Key]
        public long PersonId { get; set; }

        //[ForeignKey("Country")]
        public long? CountryId { get; set; } // for PR-II
        //public virtual Country Country { get; set; }

        public string? City { get; set; } 
        public long? CityId { get; set; } 

        [Required]
        [StringLength(100)]
        public string PersonName { get; set; }

        [StringLength(100)]
        public string FatherHusbandName { get; set; }
        public string Address { get; set; }
        public DateTime DateofBirth { get; set; }
        public string CellNo { get; set; }

        //public string Religion { get; set; }

        [StringLength(13)]
        public string? CNIC { get; set; }
        [StringLength(20)]
        public string? PassportNo { get; set; } // for PR-II
        [StringLength(50)]
        public string? Nationality { get; set; } // for PR-II
        public DateTime? VisaExpiryDate { get; set; } // for PR-II
        
        

        

        
    }
}
