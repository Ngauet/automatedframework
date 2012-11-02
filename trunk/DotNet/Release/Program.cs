using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportLibrary;
using Driver;
using System.Configuration;

namespace Release
{
    class Program
    {
        static void Main(string[] args)
        {            
            XmlDataAccess XmlHelper1 = new XmlDataAccess("performanceConfig.xml");
            XmlDataAccess XmlHelper2 = new XmlDataAccess("performanceInput.xml");
            XmlDataAccess XmlHelper3 = new XmlDataAccess("performanceReport.xml");
            List<TestCommand> listTCommand = XmlHelper1.getTestCommands();
            ComponentFunction.Initialize(listTCommand);
            List<TestSuite> listTSuite = XmlHelper1.getTestSuites();            
            List<TestData> listTData = new List<TestData>();
            Dictionary<string, string> listTConfig = new Dictionary<string, string>();

            string excelPath = ConfigurationSettings.AppSettings["ExcelFile"];
            
            ExcelDataAccess exceldataAccess = new ExcelDataAccess(excelPath);
            listTConfig = exceldataAccess.getTestConfig();

            listTConfig = XmlHelper1.getTestConfig();

            foreach (TestSuite tsuite in listTSuite) { 
                string Log = "";
                if (tsuite.Construct(ref Log)) {
                    List<string> listvalues = XmlHelper2.getValues(tsuite.getName());
                    foreach (string values in listvalues) {                        
                        Log = "";
                        TestData temp = new TestData();

                        string description = string.Empty;
                        if (tsuite.Debug(values, ref Log, ref description, listTConfig))
                        {
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
                        listTData.Add(temp);
                    }
                }    
            }
            XmlHelper3.BuildReport(listTData);
            Console.ReadLine();
        }
    }
}
