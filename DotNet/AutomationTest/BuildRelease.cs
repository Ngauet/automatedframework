using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Driver;
using SupportLibrary;
namespace AutomationTest
{
    class BuildRelease:ConsoleBase
    {
        private ExcelDataAccess _dtAccess = null;
        private List<TestCase> listTCase = new List<TestCase>();
        private List<TestStep> listTStep = new List<TestStep>();
        private List<TestCommand> Commands = new List<TestCommand>();
        private List<TestData> Datas = new List<TestData>();
        private List<TestSuite> validTestSuite = new List<TestSuite>();
        private List<TestConfig> _ListTestConfig = new List<TestConfig>();

        private string _Name = "Script.exe";
        private string _DesPath = string.Empty;

        private Build build = null;
        public BuildRelease(ExcelDataAccess dtAccess,string DesPath)
        {
            _dtAccess = dtAccess;
            listTCase = _dtAccess.getTestcases();
            listTStep = _dtAccess.getTeststeps();
            Commands = _dtAccess.getTestcommands();
            _ListTestConfig = _dtAccess.getTestConfigs();
            Datas = _dtAccess.getTestdatas();
            _DesPath = DesPath;
            //build = new Build(DesRelease);
        }

        public BuildRelease(ExcelDataAccess dtAccess, string DesPath,string scriptName)
        {
            _dtAccess = dtAccess;
            listTCase = _dtAccess.getTestcases();
            listTStep = _dtAccess.getTeststeps();
            Commands = _dtAccess.getTestcommands();
            Datas = _dtAccess.getTestdatas();
            string DesRelease = DesPath;
            //build = new Build(DesRelease, scriptName);
        }

        protected override bool Input()
        {
            Console.WriteLine("Please input script name:");
            _Name = Console.ReadLine();
            build = new Build(_DesPath, _Name);
            return true;
        }
        protected override bool Validator()
        {
            ComponentFunction.Initialize(Commands);
            if (_dtAccess == null)
            {
                Error.WriteLine("The ExcelDataAccess object is null.");
                return false;
            }
            List<TestSuite> listTSuite = new List<TestSuite>();
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
            List<TestSuite> invalidTestSuite = new List<TestSuite>();
            //Validate each Testsuite:
            foreach (TestSuite tsuite in listTSuite)
            {
                string errormes = "";
                if (tsuite.Construct(ref errormes))
                {
                    validTestSuite.Add(tsuite);
                }
                else
                {
                    invalidTestSuite.Add(tsuite);
                    Error.WriteLine("Error at testcase =" + tsuite.getName());
                    Error.WriteLine(errormes);
                }
            }
            int n = invalidTestSuite.Count;
            if (n > 0)
            {
                Error.Write("There are errors. Do you want to continue?(Y,N)");
                string ans = In.ReadLine();
                if (ans.Equals("N") || ans.Equals("n"))
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool Run()
        {
            if (!build.BuildFolder())
                return false;
            if(!build.BuildConfig(validTestSuite, Commands,_ListTestConfig))
                return false;
            if(!build.BuildInput(validTestSuite, Datas))
                return false;
            if(!build.BuildReport())
                return false;
            if (!build.BuildExe())
                return false;            
            return true;
        }
    }
}
