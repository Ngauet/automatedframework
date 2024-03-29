//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace mynamespace
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using System.Configuration;
    using Driver;
    using SupportLibrary;
    
    
    public class myclass
    {
        
        private string strMessage;
        
        public virtual string Message
        {
            get
            {
                return strMessage;
            }
            set
            {
                strMessage=value;
            }
        }
        
        public static void Main()
        {
             XmlDataAccess XmlHelper1 = new XmlDataAccess("testbuildConfig.xml");
            XmlDataAccess XmlHelper2 = new XmlDataAccess("testbuildInput.xml");
            XmlDataAccess XmlHelper3 = new XmlDataAccess("testbuildReport.xml");
            List<TestCommand> listTCommand = XmlHelper1.getTestCommands();
            ComponentFunction.Initialize(listTCommand);
            List<TestSuite> listTSuite = XmlHelper1.getTestSuites();            
            List<TestData> listTData = new List<TestData>();
            Dictionary<string, string> listTConfig = new Dictionary<string, string>();

            listTConfig = XmlHelper1.getTestConfig();

            foreach (TestSuite tsuite in listTSuite) { 
                string Log = "";
                if (tsuite.Construct(ref Log)) {
                    List<string> listvalues = XmlHelper2.getValues(tsuite.getName());
                    foreach (string values in listvalues) {                        
                        Log = "";
                        TestData temp = new TestData();
                        MessageShow.DebugTCase(tsuite.getName(), tsuite.getParameters(), tsuite.getInputArgumentsTypeInfo(), values);
                        string description = string.Empty;
                        if(tsuite.Debug(values, ref Log, ref description, listTConfig)){
                            temp.TestCase = tsuite.getName();
                            temp.Values = values;
                            temp.Result = Glossary.PASS;
                            temp.Comment = Log;
                        }else{
                            temp.TestCase = tsuite.getName();
                            temp.Values = values;
                            temp.Result = Glossary.FAILURE;
                            temp.Comment = Log;
                        }
                        MessageShow.DebugFinish(tsuite.getName(), temp.Result, temp.Comment);
                        listTData.Add(temp);
                    }
                }    
            }
            XmlHelper3.BuildReport(listTData);
            Console.ReadLine();
        }
    }
}
