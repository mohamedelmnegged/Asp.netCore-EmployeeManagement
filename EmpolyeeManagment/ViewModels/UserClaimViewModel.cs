using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmpolyeeManagment.ViewModels
{
    public class UserClaimViewModel
    {
        public UserClaimViewModel()
        {
            Claims = new List<UserClaim>(); 
        }
        public string UserId { get; set; }
        public List<UserClaim> Claims { get; set; }
    }
}
