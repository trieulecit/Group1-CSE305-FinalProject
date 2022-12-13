using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryAgentManagementApp.Models
{
    public class Courier
    {
        [Key]

        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Name { get; set; }
        public int ShippingFee { get; set; }

        public ICollection<Order> Orders { get; set; }

    }
}
