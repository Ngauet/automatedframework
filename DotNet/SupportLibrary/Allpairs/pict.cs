using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace SupportLibrary
{
    public class pict
    {
        private string txtTemp = "pict.txt";
        private string exeTemp = "pict.exe";
        private ProcessClass _proc;

        public pict() {            
            _proc = new ProcessClass(exeTemp);
            _proc.AddArguments(txtTemp);
        }

        /**
         * @lines: with structure
         * [param1] : [value1],[value2] ... [valuen]
         * [param2] : [value1],[value2] ... [valuen]
         * ...
         * [paramn] : [value1],[value2] ... [valuen]
         * */
        public List<string> Generate(List<string> lines) {
            List<string> rs = new List<string>();
            FileIO.WriteToText(txtTemp, lines);
            string result = _proc.Run();
            int i = 0;
            using (StringReader reader = new StringReader(result))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (i != 0)
                    {
                        line = line.Replace("\t", ",");
                        rs.Add(line);
                    }
                    i++;
                }
            }
            return rs;
        }

    }
}
