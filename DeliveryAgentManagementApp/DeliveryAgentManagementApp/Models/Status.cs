using System.Runtime.CompilerServices;

namespace DeliveryAgentManagementApp.Models
{
    public enum Status
    {
         EXCEPTION, //Request Rescheduling 
         PENDING,
         OUTFORDELIVERY,
         DELIVERED,        
         CANCELED,
    }
}
