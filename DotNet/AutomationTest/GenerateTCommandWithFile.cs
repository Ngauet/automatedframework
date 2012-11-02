using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportLibrary;
using Driver;
namespace AutomationTest
{
    class GenerateTCommandWithFile:ConsoleBase
    {
        private string _pathToFile;
        private ExcelDataAccess _dtAccess;
        //private List<string> ExclusionFunc = new List<string>();
        public GenerateTCommandWithFile(string dllPath,ExcelDataAccess dtAccess) {
            _pathToFile = dllPath;
            _dtAccess = dtAccess;
            //ExclusionFunc = FileIO.ReadFile("ExclusionFunc.txt");
        }
        protected override bool Input()
        {
            return true;
        }
        //private bool isExcluse(string function) {
        //    foreach (string item in ExclusionFunc) {
        //        if (item.Equals(function))
        //            return true;
        //    }
        //    return false;
        //}

        protected override bool Validator()
        {
            if (_dtAccess == null) {
                Error.WriteLine("ExcelDataAccess object is null.");
                return false;
            }
            List<string> temp = util.getListValueWithToken(_pathToFile, '.');
            int n = temp.Count;
            if (n > 1)
            {
                string Extension = temp[n-1];
                if (!Extension.Equals("dll")) {
                    Error.WriteLine(_pathToFile +" isnot .dll file.");
                }
            }
            else {
                Error.WriteLine(_pathToFile +" isnot file.");
                return false;
            }
            FileIO FileHelper = new FileIO();
            bool Flag = FileHelper.IsExist(_pathToFile);
            if (Flag)
            {
                Out.WriteLine(_pathToFile + " is exist.");
                return true;
            }
            else {
                Error.WriteLine(_pathToFile + " isnot exist.");
                return false;
            }
        }



        protected override bool Run()
        {
            List<TestCommand> existCommands = _dtAccess.getTestcommands();
            int Id = existCommands.Count + 1;
            List<TestCommand> commands = ComponentFunction.GetAllMethodsDLLs(_pathToFile);
            int n = commands.Count;
            List<TestCommand> acceptCommands = new List<TestCommand>();
            for (int i = 0; i < n; i++)
            {
                TestCommand temp = commands[i];
                //Not generate exclusion function:
                //if(!isExcluse(temp.Function)){
                    temp.ID = (i+Id).ToString();
                    acceptCommands.Add(temp);
                //}
            }
            if (n > 0)
            {
                _dtAccess.GenerateCommands(acceptCommands);                
                this.Out.WriteLine("There are " + n + " commands exists.");
               return true;
            }
            else
            {
                return false;
            }

        }
    }
}
