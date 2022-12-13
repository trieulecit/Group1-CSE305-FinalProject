using System.ComponentModel.DataAnnotations;

namespace DeliveryAgentManagementApp.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string Product { get; set; }
        public string CustomerName { get; set; }      
        public int? CourierId { get; set; }     
        public Courier Courier { get; set; }
        public string Destination { get; set; }
        public int TotalPrice { get; set; }
        public string Retailer { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateOrdered { get; set; }
        public Status DeliveryStatus { get; set; }
        public ICollection<Message> Messages { get; set; }
       
    }
}
