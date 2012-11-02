using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Driver
{
    public class Variable
    {
        public string Name { get; set; }        
        public object Value { get; set; }
        public string Type { get; set; }
        public Variable(string name)
        {
            Name = name;           
            Value = null;
            Type = null;
        }       
    }
}
