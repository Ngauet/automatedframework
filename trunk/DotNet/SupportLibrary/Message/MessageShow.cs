using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using log4net;


namespace SupportLibrary
{
    public static class MessageShow
    {

        

        public static void Writelog(string title,string body){
            LogCentral.Current.LogDebug(title + body);          
        }
         
        public static void DebugTCase(string TCaseName,string inputParam,List<string> ArgumentInfo, string inputValues) {
            string ArgumentType = "";
            foreach (string str in ArgumentInfo) {
                ArgumentType += str + "\t";
            }
            string title = "Debug testcase " + TCaseName+"("+inputParam+")";
            string body = "Arguments:" + ArgumentType + "\r\n Input values = " + inputValues;
            Writelog(title, body);
        }

        public static void StepPass(string step, string func, string resultObject,string InputParams) {
            string title = "Step: " + step + " PASS.";
            string body = "Excute function:" + func + "\r\n Input parameters:" + InputParams +"\r\n" + "Result : " + resultObject ;
            Writelog(title, body);
        }

        public static void StepFail(string step, string func,string InputParams, string Error) {
            string title = "Step: " + step + " FAIL.";
            string body = "Excute function:" + func + "\r\n Input parameters:" + InputParams +"\r\n" +Error;
            Writelog(title, body);
        }

        public static void DebugFinish(string TCaseName, string result, string comment) {
            string title = "Debug testcase " +TCaseName+".";
            string body = result + "\r\n" + comment;
            Writelog(title, body);
        }

        public static void StartFeature(string featureName) {
            string body = "Start feature " + featureName;
            Console.WriteLine(body);
            Writelog("Start feature.",body);
        }
        public static int ERRORCOUNT = 0;  
        //READING DATA FROM DATABASE

        //Errors occur
        public static void FileDoesNotExist(string path){
            string body = "The path file ="+path+" does not exist.";
            Console.WriteLine(body);            
            ERRORCOUNT++;
            Writelog("File doesnot exist.", body);
        }
        
        public static void ExceptionOccur(string Decription,Exception e) {
            Console.WriteLine(Decription);
            Console.WriteLine(e.ToString());
            ERRORCOUNT++;
            Writelog(Decription, e.ToString());
        }

        public static void TestdataError(string ID) {
            string body = "TData id = "+ID+" isnot valid.";
            Console.WriteLine(body);
            ERRORCOUNT++;
            Writelog("TData error.", body);
        }

        public static void TStepError(string TCaseName) {
            string body = "TStep for TCase = " + TCaseName + " isnot valid.";
            Console.WriteLine(body);
            ERRORCOUNT++;
            Writelog("TStep error.",body);
        }

        public static void InvalidRecord(int iRecord) {
            string body = "Record line " + iRecord + " contain invalid field.";
            Console.WriteLine(body);
            ERRORCOUNT++;
            Writelog("Invalid record.", body);
        }

        public static void TCaseNameExist(string TCaseName) {
            string body = "TCase's name ="+TCaseName+" is exist.";
            Console.WriteLine(body);
            ERRORCOUNT++;
            Writelog("TCase is exist.", body);
        }

        public static void KeywordExist(string Keyword) {
            string body = "Keyword =" + Keyword + " is exist.";
            Console.WriteLine(body);
            ERRORCOUNT++;
            Writelog("Keyword is exist.", body);
        }

        
    }
}
