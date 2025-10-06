using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherSubscriptionService.Models
{
    public class SubscriptionRequest
    {
        public string Email { get; set; }
        public string City { get; set; }
    }
}
