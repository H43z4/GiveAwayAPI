using Models.DatabaseModels.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels.DSAuth.Setup
{
    public class VwDSAUser
    {
        public long? UserId { get; set; }

        //public long? UserDistrictId { get; set; }

        //public long? PersonId { get; set; }
        //public long? RoleId { get; set; }

        //public string RoleName { get; set; }
        //public string UserType { get; set; }
        //public long? UserStatusId { get; set; }
        //public string UserStatus { get; set; }

        //public long? OrganizationId { get; set; }

        //public string OrganizationName { get; set; }
        //public List<Role> UserRoles { get; set; }

        //public string IP { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
    }
}
