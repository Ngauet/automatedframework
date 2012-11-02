using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupportLibrary
{
    public class StringReplace
    {
        public StringReplace(string replace, string value) {
            Replace = replace;
            Value = value;
        }
        public string Replace {get;set;}
        public string Value {get;set;}
    }
}
