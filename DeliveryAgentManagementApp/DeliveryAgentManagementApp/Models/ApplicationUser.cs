using Microsoft.AspNetCore.Identity;

namespace DeliveryAgentManagementApp.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public Courier Courier { get; set; }
    }
}
