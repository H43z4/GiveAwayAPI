using System;

namespace Models.ViewModels.DSAuth.Setup
{
    public class VwDSPerson
    {
        public long PersonId { get; set; }
        public string PersonName { get; set; }
        public string CNIC_PASSPORT { get; set; }
        public string CellNo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public DateTime? DateofBirth { get; set; }
        public DateTime? VisaExpiryDate { get; set; }
        public int? Age { get; set; }
    }
}
