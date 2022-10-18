using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchanger
{
    public class Currency
    {
        public string Name { get; set; }
        public double value { get; set; }
        public Currency(string name, double value)
        {
            Name = name;
            this.value = value;
        }
    }
}
