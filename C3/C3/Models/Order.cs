using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace C3.Models
{
    public class Order
    {
        public int ID { get; set; }
        public Point Destination { get; set; }
        public double Priority { get; set; }
    }
}
