using System.ComponentModel.DataAnnotations;

namespace DeliveryAgentManagementApp.Models
{
    public class Message
    {
        [Key]

        public int Id { get; set; }
        public int CourierId { get; set; }
        public Courier Courier { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateSent { get; set; }
    }
}
