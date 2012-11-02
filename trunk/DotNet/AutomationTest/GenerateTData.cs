using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Driver;
using SupportLibrary;
namespace AutomationTest
{
    class GenerateTData:ConsoleBase
    {
        private ExcelDataAccess _dtAccess = null;
        private List<TestCase> listTCase = new List<TestCase>();
        private int beginID = 0;
        private List<TestDataset> listTDataset = new List<TestDataset>();
        public GenerateTData(ExcelDataAccess dtAccess)
        {
            _dtAccess = dtAccess;
            listTCase = _dtAccess.getTestcases();
            List<TestData> listTData = _dtAccess.getTestdatas();
            listTDataset = _dtAccess.getTestdatasets();
            beginID = listTData.Count;
        }

        protected override bool Input()
        {
            return true;
        }

        protected override bool Validator()
        {           
            if (_dtAccess == null)
            {
                Error.WriteLine("The ExcelDataAccess object is null.");
                return false;
            }
            int n = listTDataset.Count;
            bool flag = true;
            if (n <= 0)
            {
                Error.WriteLine("The no TDataset available.");
                return false;
            }
            List<TestDataset> temp = listTDataset;
            for (int i = 0; i < n; i++)
            {
                string TCaseName = temp[i].TestCase;
                if (!TCaseExist(TCaseName))
                {
                    Error.WriteLine("TCaseName =" + TCaseName + " isnot exist.");
                    flag = false;
                }
            }
            if (!flag)
            {               
                Error.Write("The TDataSet isnot valid. Do you want to continue? (y/n)");
                string ans = In.ReadLine();
                if (ans.Equals("n") || ans.Equals("N"))
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool Run()
        {
            //Initialize testcases using in TDataset
            int n = listTDataset.Count;
            List<string> TcasesIn = new List<string>();
            for (int i = 0; i < n; i++)
            {
                string TCaseName = listTDataset[i].TestCase;
                bool isExist = false;
                foreach (var item in TcasesIn)
                {
                    if (item.Equals(TCaseName))
                    {
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    TcasesIn.Add(TCaseName);
                }
            }
            //Generate TestDatas for each testcase using
            foreach (string item in TcasesIn)
            {
                List<TestDataset> currentList = new List<TestDataset>();
                List<string> listvalues = new List<string>();
                TestCase inTCase = getTestCase(item);
                List<string> listParams = util.getListValueFromString(inTCase.Parameters);
                List<string> lines = new List<string>();
                for (int i = 0; i < n; i++)
                {
                    if (listTDataset[i].TestCase.Equals(item))
                        currentList.Add(listTDataset[i]);
                }
                if (currentList.Count != listParams.Count)
                {                    
                    Error.Write("The number of arguments input for TCase="+item+" isnot valid. Do you want to continue? (y/n)");
                    string ans = In.ReadLine();
                    if (ans.Equals("n") || ans.Equals("N"))
                    {
                        return false;
                    }
                    else {
                        continue;
                    }
                }
                else
                {
                    foreach (string str in listParams)
                    {
                        bool isValidParam = false;
                        for (int i = 0; i < currentList.Count; i++)
                        {
                            if (currentList[i].Parameter.Equals(str))
                            {
                                string line = str + ":" + currentList[i].Values;
                                lines.Add(line);
                                isValidParam = true;
                            }
                        }
                        if (!isValidParam)
                        {                            
                            Error.Write("The parameter " + str + " isnot exist in TCase =" + inTCase.Name+ " isnot valid. Do you want to continue? (y/n)");
                            string ans = In.ReadLine();
                            if (ans.Equals("n") || ans.Equals("N"))
                            {
                                return false;
                            }
                        }
                    }
                    if (lines.Count == currentList.Count)
                    {
                        pict Pict = new pict();
                        listvalues = Pict.Generate(lines);
                        GenerateTestDatas(listvalues, inTCase);
                    }
                }
            }
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
