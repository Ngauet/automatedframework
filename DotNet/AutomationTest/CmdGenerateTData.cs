using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Driver;
using SupportLibrary;
namespace AutomationTest
{
    class CmdGenerateTData:ConsoleBase
    {
         private ExcelDataAccess _dtAccess = null;
        private List<TestCase> listTCase = new List<TestCase>();
        private int beginID = 0;
        
        public CmdGenerateTData(ExcelDataAccess dtAccess)
        {
            _dtAccess = dtAccess;
            listTCase = _dtAccess.getTestcases();
            List<TestData> listTData = _dtAccess.getTestdatas();
            beginID = listTData.Count;
        }
        protected override bool Input()
        {
            return true;
        }

        protected override bool Validator()
        {
            if (_dtAccess != null)
            {
                Error.WriteLine("The ExcelDataAccess object is null.");
                return false;
            }
            return true;
        }

        protected override bool Run()
        {           
            string input;
            do
            {
                do
                {
                    Out.WriteLine("///////////////////////////////////////////////////////////////////////////////////////////////");
                    Out.WriteLine("/////////////////////////////////////TESTCASE//////////////////////////////////////////////////");
                    Out.WriteLine("///////////////////////////////////////////////////////////////////////////////////////////////");
                    Out.WriteLine("Name         :Parameters                   ");
                    foreach (TestCase item in listTCase)
                    {
                        Out.WriteLine(item.Name + ":" + item.Parameters);
                        Out.WriteLine();
                    }
                    Out.WriteLine("///////////////////////////////////////////////////////////////////////////////////////////////");
                    Out.WriteLine("/////////////////////////////////////TESTCASE//////////////////////////////////////////////////");
                    Out.WriteLine("///////////////////////////////////////////////////////////////////////////////////////////////");
                    Out.WriteLine("Please type the testcase's name that you want to generate testdata.");
                    Out.WriteLine("/e: for exit to menu.");
                    Out.Write("TestCase: ");
                    input = In.ReadLine();
                    if (input.Equals("/e"))
                        break;
                } while (!TCaseExist(input));
                if (input.Equals("/e"))
                    break;
                TestCase inTCase = getTestCase(input);
                List<string> listParams = util.getListValueFromString(inTCase.Parameters);
                int nArguments = listParams.Count;
                List<string> lines = new List<string>(nArguments);
                List<string> listvalues = new List<string>();
                do
                {
                    bool flag = true;
                    Out.WriteLine("Please type the input param with syntax: [value1],[value2],..[valuen]");
                    Out.WriteLine("/b: for choose another testcase.");
                    Out.WriteLine("/e: for exit to menu.");
                    for (int i = 0; i < nArguments; i++)
                    {
                        Out.Write("Input for param " + listParams[i] + " :");
                        input = In.ReadLine();

                        if (input.Equals("/b") || input.Equals("/e"))
                            flag = false;
                        string temp = listParams[i] + ":" + input;
                        lines.Add(temp);
                    }
                    if (!flag)
                        break;
                    pict Pict = new pict();
                    listvalues = Pict.Generate(lines);
                } while (!GenerateTestDatas(listvalues, inTCase));
            } while (!input.Equals("/e"));
            return true;
        }

        private TestCase getTestCase(string TestCase)
        {
            foreach (TestCase item in listTCase)
            {
                if (item.Name.Equals(TestCase))
                    return item;
            }
            return null;
        }

        private bool TCaseExist(string TestCase)
        {
            foreach (TestCase item in listTCase)
            {
                if (item.Name.Equals(TestCase))
                    return true;
            }
            return false;
        }

        private bool GenerateTestDatas(List<string> listvalues, TestCase tcase)
        {
            if (_dtAccess != null)
            {                
                if (beginID == 0)
                    beginID++;
                List<string> listParams = util.getListValueFromString(tcase.Parameters);
                int nArgument = listParams.Count;
                if (nArgument == 1 && listParams[0].Equals(""))
                {
                    Out.WriteLine("TCase name=" + tcase.Name + " have no arguments");                   
                    return false;
                }
                else
                {
                    List<TestData> newDatas = new List<TestData>();
                    foreach (string item in listvalues)
                    {
                        int n = util.getListValueFromString(item).Count;
                        if (n != nArgument)
                        {
                            Error.WriteLine("The number of arguments input isnot valid.");
                            return false;
                        }
                        else
                        {
                            TestData temp = new TestData();
                            temp.ID = beginID.ToString();
                            temp.Comment = "Data generate by pict.";
                            temp.TestCase = tcase.Name;
                            temp.Values = item;
                            temp.Result = "";
                            beginID++;
                            newDatas.Add(temp);                            
                        }
                    }
                    if (newDatas.Count > 0)
                    {
                        _dtAccess.GenerateTestdatas(newDatas);
                        Out.WriteLine("Generate TData for TCase=" + tcase.Name + " successful.");
                        Out.WriteLine("There are " + newDatas.Count + " TestDatas generated by pict.");                       
                    }
                    else
                    {
                        Error.WriteLine("Generate TData for TCase=" + tcase.Name + " unsuccessful.");
                        Error.WriteLine("No valid TData to generate.");                        
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
