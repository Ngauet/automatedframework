using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportLibrary;
namespace Driver
{
    public class CodeDomBuilder
    {
        private string _Path = null;
        private CCodeGenerator codeDom = null;
        private string _Template = @" XmlDataAccess XmlHelper1 = new XmlDataAccess(""###Name###Config.xml"");
            XmlDataAccess XmlHelper2 = new XmlDataAccess(""###Name###Input.xml"");
            XmlDataAccess XmlHelper3 = new XmlDataAccess(""###Name###Report.xml"");
            List<TestCommand> listTCommand = XmlHelper1.getTestCommands();
            ComponentFunction.Initialize(listTCommand);
            List<TestSuite> listTSuite = XmlHelper1.getTestSuites();            
            List<TestData> listTData = new List<TestData>();
            Dictionary<string, string> listTConfig = new Dictionary<string, string>();

            listTConfig = XmlHelper1.getTestConfig();

            foreach (TestSuite tsuite in listTSuite) { 
                string Log = """";
                if (tsuite.Construct(ref Log)) {
                    List<string> listvalues = XmlHelper2.getValues(tsuite.getName());
                    foreach (string values in listvalues) {                        
                        Log = """";
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
            Console.ReadLine()"; 

        public CodeDomBuilder(string path) {
            _Path = path;
            codeDom = new CCodeGenerator();
        }

        public bool BuildExe() {
            List<string> Stats = new List<string>();
            string temp = _Template;
            Stats.Add(temp);
            codeDom.BuildCode(_Path, Stats);
            return true;
        }

        public bool BuildExe(string scriptName)
        {
            List<string> Stats = new List<string>();
            string temp = _Template;
            temp = temp.Replace("###Name###", scriptName);
            Stats.Add(temp);
            codeDom.BuildCode(_Path, Stats);

            return true;
        }
    }
}
