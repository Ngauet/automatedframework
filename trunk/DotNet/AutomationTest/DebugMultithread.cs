using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using Driver;
using SupportLibrary;

namespace AutomationTest
{
    class DebugMultithread:ConsoleBase
    {
        private bool _isFinish = false;
        private int numOfThread = 1;
        private int rampupPeriod = 0;

        private ClassWorkManager _Manager = null;
        
        private ExcelDataAccess _dtAccess = null;
        private List<TestSuite> _validTestSuite = new List<TestSuite>();
        private List<TestSuite> _invalidTestSuite = new List<TestSuite>();
        private List<TestSuite> _blockTestSuite = new List<TestSuite>();
        private List<TestData> _listTData = new List<TestData>();
        private List<TestSuite> _listTSuite = new List<TestSuite>();

        public DebugMultithread(ExcelDataAccess dtAccess)
        {
            List<TestCommand> Commands = dtAccess.getTestcommands();
            ComponentFunction.Initialize(Commands);
            _dtAccess = dtAccess;  
        }
        

        protected override bool Input()
        {
            //string input = "";
            //int nThread = 1;
            //do{
            //    Out.WriteLine("Number of thread:");
            //    input = In.ReadLine();
            //}while(Int32.TryParse(input,out nThread));
            //numOfThread = nThread;
            try
            {
                int numothread = int.Parse( ConfigurationSettings.AppSettings["NumOfThread"]);
                this.numOfThread = numothread;

                int rampupPeriod = int.Parse(ConfigurationSettings.AppSettings["RampupPeriod"]);
                this.rampupPeriod = rampupPeriod;
            }
            catch (Exception ex)
            { 

            }
            

            return true;
        }
        protected override bool Validator()
        {
            if (_dtAccess == null)
            {
                Error.WriteLine("The ExcelDataAccess object is null.");
                return false;
            }
            List<TestCase> listTCase = _dtAccess.getTestcases();
            List<TestStep> listTStep = _dtAccess.getTeststeps();
            _listTData = _dtAccess.getTestdatas();

            if (_listTData == null)
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
                if (_listTData.Count <= 0)
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
                _listTSuite.Add(tsuite);
            }

            //Validate each Testsuite:
            foreach (TestSuite tsuite in _listTSuite)
            {
                string errormes = "";
                if (tsuite.Construct(ref errormes))
                {
                    if (tsuite.getState().Equals(Glossary.BLOCK))
                    {
                        _blockTestSuite.Add(tsuite);
                    }
                    else
                    {
                        _validTestSuite.Add(tsuite);
                    }
                }
                else
                {
                    _invalidTestSuite.Add(tsuite);
                    Error.WriteLine("Error at testcase =" + tsuite.getName());
                    Error.WriteLine(errormes);
                }
            }
            Out.WriteLine("/////////////////////////////////////VALID-TESTCASE////////////////////////////////////////////");

            Out.WriteLine("ID       :Name         :Parameters                   ");
            foreach (TestSuite v_tsuite in _validTestSuite)
            {
                Out.WriteLine(v_tsuite.getID() + " :" + v_tsuite.getName() + " :");
                List<string> listArgumentInfo = v_tsuite.getInputArgumentsTypeInfo();
                foreach (string str in listArgumentInfo)
                {
                    Out.WriteLine(str);
                }
            }

            Out.WriteLine("/////////////////////////////////////VALID-TESTCASE////////////////////////////////////////////");


            Out.WriteLine();

            Out.WriteLine("/////////////////////////////////////BLOCK-TESTCASE////////////////////////////////////////////");

            //Console.WriteLine("Name         :Parameters                   ");
            foreach (TestSuite b_tsuite in _blockTestSuite)
            {
                Out.WriteLine(b_tsuite.getName() + " :");

            }

            Out.WriteLine("/////////////////////////////////////BLOCK-TESTCASE////////////////////////////////////////////");

            Out.WriteLine();

            Out.WriteLine("/////////////////////////////////////INVALID-TESTCASE////////////////////////////////////////////");

            //Console.WriteLine("Name         :Parameters                   ");
            foreach (TestSuite v_tsuite in _invalidTestSuite)
            {
                Out.WriteLine(v_tsuite.getName() + " :");

            }
            Out.WriteLine("///////////////////////////////////////////////////////////////////////////////////////////////");
            Out.WriteLine("/////////////////////////////////////INVALID-TESTCASE////////////////////////////////////////////");
            Out.WriteLine("///////////////////////////////////////////////////////////////////////////////////////////////");
            int nInvalid = _invalidTestSuite.Count;
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
            int nValid = _validTestSuite.Count;
            if (nValid <= 0)
            {
                Error.WriteLine("There is not valid testcase.");
                UpdateSteps();
                return false;
            }
            foreach (TestData data in _listTData)
            {
                if (data.Result.Equals("") || data.Result.Equals(Glossary.NOTRUN))
                {
                    foreach (TestSuite tsuite in _listTSuite)
                    {
                        if (data.TestCase.Equals(tsuite.getName()))
                        {
                            tsuite.inTData(data);
                            break;
                        }
                    }
                }
            }
            
            return true;
        }

        private void UpdateSteps()
        {
            foreach (TestSuite tsuite in _listTSuite)
            {
                List<TestStep> updateSteps = tsuite.getSteps();
                _dtAccess.UpdateSteps(updateSteps);
            }
        }

        private void UpdateDatas()
        {
            _dtAccess.UpdateDatas(_listTData);
        }

        private void InitThread() {
            //Start Multithread
            ClassWorkManager.Unit[] work = null;
            ClassWorkManager.WorkFinished finished = null;

            ClassWorkManager.WorkToDo worktodo = new ClassWorkManager.WorkToDo(DoWork);
            //List of tasks
            int n = _listTSuite.Count;
            
            // each of work respect with a task
            work = new ClassWorkManager.Unit[n];
            for (int i = 0; i < n; i++)
            {
                work[i] = new ClassWorkManager.Unit(worktodo, _listTSuite[i]);
            }

            finished = new ClassWorkManager.WorkFinished(Finished);
            _Manager = new ClassWorkManager(this.numOfThread);
            _Manager.DoWork(work, finished);
        }

        object DoWork(object param)
        {
            List<TestData> WorkResult = new List<TestData>();
            TestSuite CurrentTSuite = (TestSuite)param;
            WorkResult = CurrentTSuite.Run();
            Thread.Sleep(this.rampupPeriod * 1000);
            
            return WorkResult;
        }
        void Finished(ArrayList result) {
            _listTData = new List<TestData>();
            foreach (ClassWorkManager.Unit item in result)
            {
                //object tam = item.Results;
                List<TestData> list = (List<TestData>)item.Results;
                _listTData.AddRange(list);
            }
            _isFinish = true;
        }

        protected override bool Run()
        {
            _isFinish = false;
            InitThread();
            while (true) {
                Thread.Sleep(5000);
                if (_isFinish) {
                    break;
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
