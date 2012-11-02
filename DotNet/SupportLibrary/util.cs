using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupportLibrary
{
    public static class util
    {

        public static List<StringReplace> getReplaceString(ref string values) {
            List<StringReplace> listReplace = new List<StringReplace>();
            int StartIndex = 0;
            int EndIndex = 0;
            string Start = "\\" + "[";
            string End = "\\" + "]";
            while (StartIndex != -1 || EndIndex != -1)
            {
                StartIndex = values.IndexOf(Start);
                EndIndex = values.IndexOf(End);
                if (StartIndex != -1 && EndIndex != -1)
                {
                    string Replace = "@{" + listReplace.Count + "}";
                    string Value = values.Substring(StartIndex + 2, EndIndex - StartIndex - 2);
                    listReplace.Add(new StringReplace(Replace, Value));
                    values = values.Replace(Value, Replace);
                    values = values.Replace(Start, "").Replace(End, "");
                }
            }
            return listReplace;
        }
        /**
         * GET LIST PARAMETERS NAME
         * */
        public static List<string> getListValueFromString(string str)
        {
            List<string> listparameters = str.Split(',').ToList();
            return listparameters;
        }
        public static List<string> getListValueWithToken(string str, char token) {
            List<string> listValue = str.Split(token).ToList();
            return listValue;
        }
        /**
         * GET METHOD'S NAME
         * @func: [return type] [MethodName]([param_type],[param_type],[param_type],..)
         * */
        public static string getMethodName(string func) { 
            string rs = func.Split('(')[0].Split(' ')[1];
            return rs;
        }
        

        public static string getDateTime() {
            return System.DateTime.Now.ToString("dd_mm_yyyy : hh_mm_ss");
        }

        public static string getDate() {
            return System.DateTime.Now.ToString("dd_MM_yyyy");
        }

        public static string getFolder(string file) {
            string[] listTam = file.Split(new string[] { "\\", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int nTam = listTam.Length;
            string rs = "";
            for (int i = 0; i < nTam-1; i++) {
                rs += listTam[i]+"\\";
            }
            return rs;
        }

        public static string getFileFromPath(string Path)
        {
            string[] listTam = Path.Split(new string[] { "\\", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int nTam = listTam.Length;
            string Object = listTam[nTam - 1];
            return Object;
        }
    }
}
