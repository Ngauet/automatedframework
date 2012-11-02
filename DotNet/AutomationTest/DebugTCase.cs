using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportLibrary;
using Driver;
using System.Configuration;
using System.Reflection;
namespace AutomationTest
{
    class DebugTCase:ConsoleBase
    {
        private ExcelDataAccess _dtAccess = null;
        private List<TestSuite> validTestSuite = new List<TestSuite>();
        private List<TestSuite> invalidTestSuite = new List<TestSuite>();
        private List<TestSuite> blockTestSuite = new List<TestSuite>();
        private List<TestData> listTData = new List<TestData>();
        private List<TestSuite> listTSuite = new List<TestSuite>();
        private Dictionary<string, string> listTConfig = new Dictionary<string, string>();
        public DebugTCase(ExcelDataAccess dtAccess) {
            List<TestCommand> Commands = dtAccess.getTestcommands();
            ComponentFunction.Initialize(Commands);
            _dtAccess = dtAccess;            
        }
        protected override bool Input()
        {
            return true;
        }
        protected override bool Validator()
        {            
            if (_dtAccess == null) {
                Error.WriteLine("The ExcelDataAccess object is null.");
                return false;
            }
            List<TestCase> listTCase = _dtAccess.getTestcases();
            List<TestStep> listTStep = _dtAccess.getTeststeps();
            listTData = _dtAccess.getTestdatas();

            if (listTData == null)
            {
                Error.Write("There are no TData available. Do you want to continue? (y/n)");
                string ans = In.ReadLine();
                if (ans.Equals("n") || ans.Equals("N"))
                {
                    return false;
                }
            }
            else
            {
                if (listTData.Count <= 0)
                {
                    Error.Write("There are no TData available. Do you want to continue? (y/n)");
                    string ans = In.ReadLine();
                    if (ans.Equals("n") || ans.Equals("N"))
                    {
                        return false;
                    }
                }
            }
            //Construct steps:
            foreach (TestCase tcase in listTCase)
            {
                List<TestStep> TStep_temp = new List<TestStep>();
                foreach (TestStep step in listTStep)
                {
                    if (step.TestCase.Equals(tcase.Name))
                        TStep_temp.Add(step);
                }
                TestSuite tsuite = new TestSuite(TStep_temp, tcase);
                listTSuite.Add(tsuite);
            }

            //Validate each Testsuite:
            foreach (TestSuite tsuite in listTSuite)
            {
                string errormes = "";
                if (tsuite.Construct(ref errormes))
                {
                    if (tsuite.getState().Equals(Glossary.BLOCK))
                    {
                        blockTestSuite.Add(tsuite);
                    }
                    else
                    {
                        validTestSuite.Add(tsuite);
                    }
                }
                else
                {
                    invalidTestSuite.Add(tsuite);
                    Error.WriteLine("Error at testcase =" + tsuite.getName());
                    Error.WriteLine(errormes);
                }
            }

            Out.WriteLine("VALID-TESTCASE:");

            Out.WriteLine("ID       :Name         :Parameters                   ");
            foreach (TestSuite v_tsuite in validTestSuite)
            {
                Out.WriteLine(v_tsuite.getID()+" :"+v_tsuite.getName() + " :");
                List<string> listArgumentInfo = v_tsuite.getInputArgumentsTypeInfo();
                foreach (string str in listArgumentInfo)
                {
                    Out.WriteLine(str);
                }
            }

            Out.WriteLine("VALID-TESTCASE");


            Out.WriteLine();

            Out.WriteLine("BLOCK-TESTCASE:");

            //Console.WriteLine("Name         :Parameters                   ");
            foreach (TestSuite b_tsuite in blockTestSuite)
            {
                Out.WriteLine(b_tsuite.getName() + " :");

            }

            Out.WriteLine("BLOCK-TESTCASE:");

            Out.WriteLine();

            Out.WriteLine("INVALID-TESTCASE:");

            //Console.WriteLine("Name         :Parameters                   ");
            foreach (TestSuite v_tsuite in invalidTestSuite)
            {
                Out.WriteLine(v_tsuite.getName() + " :");

            }
           
            Out.WriteLine("INVALID-TESTCASE");
           
            int nInvalid = invalidTestSuite.Count;
            if (nInvalid > 0)
            {
                Error.Write("There are still errors. Do you want to continue? (y/n)");
                string ans = In.ReadLine();
                if (ans.Equals("n") || ans.Equals("N"))
                {
                    UpdateSteps(); 
                    return false;
                }
            }
            int nValid = validTestSuite.Count;
            if (nValid <= 0) {
                Error.WriteLine("There is not valid testcase.");
                UpdateSteps();
                return false;
            }            
            return true;
        }

        private void UpdateSteps() {
            foreach (TestSuite tsuite in listTSuite)
            {
                List<TestStep> updateSteps = tsuite.getSteps();
                _dtAccess.UpdateSteps(updateSteps);
            }    
        }

        private void UpdateDatas() {
            _dtAccess.UpdateDatas(listTData);
        }

        protected override bool Run()
        {
            
            List<TestSuite> listRunTSuite = new List<TestSuite>();
            do{
                Out.WriteLine("Please type the ID of TestCase you want to run.");
                Out.WriteLine("<IdTestcase1>,<IdTestcase2>,..,<IdTestcaseN>");
                Out.WriteLine("/a for all.");
                Out.Write("Choose testcase to run:");
                string input = In.ReadLine();
                if (input.Equals("/a")) {
                    listRunTSuite = validTestSuite;
                }
                else
                {               
                    foreach (string item in util.getListValueFromString(input))
                    {
                        foreach (TestSuite tsuite in validTestSuite)
                        {
                            if (tsuite.getID().Equals(item))
                            {
                                listRunTSuite.Add(tsuite);
                            }
                        }
                    }
                }
            }while(listRunTSuite.Count<=0);

            string excelPath = ConfigurationSettings.AppSettings["ExcelFile"];
            ExcelDataAccess exceldataAccess = new ExcelDataAccess(excelPath);
            listTConfig = exceldataAccess.getTestConfig();
            List<TestCommand> listCommands = exceldataAccess.getTestcommands();
            string dllTarget = string.Empty;
            string dllObject = string.Empty;
            for (int i = 0; i < listCommands.Count; i++)
            {
                if (listCommands[i].isValid())
                {
                    dllTarget = listCommands[i].Target;
                    dllObject = listCommands[i].Object;
                    break;
                }
            }

            Assembly assembly = Assembly.LoadFrom(@dllObject);
            Type _tClass = assembly.GetType(dllTarget);
            Object _InvokeParam1 = null;
            try
            {
                _InvokeParam1 = Activator.CreateInstance(_tClass, listTConfig);
            }
            catch
            {
                _InvokeParam1 = null;
            }

            int n = listRunTSuite.Count;
            foreach (TestData tdata in listTData)
            {
                if (tdata.Result.Equals("") || tdata.Result.Equals(Glossary.NOTRUN))
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (tdata.TestCase.Equals(listRunTSuite[i].getName()))
                        {
                            TestSuite tsuite = listRunTSuite[i];
                            string temp = "";
                            MessageShow.DebugTCase(tsuite.getName(), tsuite.getParameters(), tsuite.getInputArgumentsTypeInfo(), tdata.Values);
                            string description = string.Empty;
                            if (tsuite.Debug(tdata.Values, ref temp, ref description, listTConfig, _InvokeParam1))
                            {
                                tdata.Result = Glossary.PASS;
                                tdata.Comment = description;
                            }
                            else
                            {
                                tdata.Result = Glossary.FAILURE;
                                //tdata.Comment = temp;
                                tdata.Comment = description;
                            }
                            MessageShow.DebugFinish(tsuite.getName(), tdata.Result, tdata.Comment);                           
                        }
                    }
                }
            }
            UpdateDatas();
            Out.WriteLine("Updated TData in excel file.");
            UpdateSteps();
            Out.WriteLine("Updated TStep in excel file.");
            return true;
        }
    }
}
