using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportLibrary;
namespace Driver
{
    public class Build
    {
        private FileIO FileHelper = new FileIO();
        private string _DesPath = null;
        //private string _App = "AppLibrary\\";
        private string _Name = string.Empty;

        //private List<TestCommand> buildCommands = new List<TestCommand>();
        public Build(string desPath)
        {
            _DesPath = desPath;
           // _App = _DesPath + "\\" + _App;
        }

        public Build(string desPath, string scriptName)
        {
            _DesPath = desPath;
            _Name = scriptName;
            // _App = _DesPath + "\\" + _App;
        }
        public bool BuildFolder()
        {
            if (FileHelper.CreateFolder(_DesPath))
                             return true;
            return false;
        }

        public bool BuildConfig(List<TestSuite> listTSuite, List<TestCommand> listTCommand, List<TestConfig> listTConfig)
        {
            XMLBuilder xmlHelper = new XMLBuilder(_DesPath + "\\" + _Name + "Config.xml");
            xmlHelper.BuildRelease(listTSuite, listTCommand,listTConfig);
            List<string> dllFiles = new List<string>();
            List<string> folders = new List<string>();
            foreach (TestCommand tcommand in listTCommand)
            {
                string temp = tcommand.Object;
                string folder = util.getFolder(temp);
                bool flag = true;
                foreach (var item in folders) {
                    if (item.Equals(folder)) {
                        flag = false;
                        break;
                    }
                }
                if (flag) {
                    folders.Add(folder);
                }
            }

            foreach (string folder in folders) {
                string[] tam = FileIO.getFiles(folder);
                dllFiles.AddRange(tam);
            }

            //foreach(string fileDll in dllFiles){
            //    string des = _DesPath + "\\" + _App + util.getFileFromPath(fileDll);
            //    FileHelper.CopyFile(fileDll, des);
            //}
            
            return true;
        }

        public bool BuildInput(List<TestSuite> listTSuite, List<TestData> listTData)
        {
            XMLBuilder xmlHelper = new XMLBuilder(_DesPath + "\\" + _Name + "Input.xml");
            xmlHelper.BuildInputData(listTSuite, listTData);
            return true;
        }

        public bool BuildReport() {
            XMLBuilder xmlHelper = new XMLBuilder(_DesPath + "\\" + _Name + "Report.xml");
            xmlHelper.BuildReport();
            return true;
        }

        public bool BuildExe() {
            CodeDomBuilder codeDom = new CodeDomBuilder(_DesPath + _Name + ".exe");
            codeDom.BuildExe(_Name);
            FileHelper.CopyFile("Driver.dll",_DesPath+"\\Driver.dll");
            FileHelper.CopyFile("SupportLibrary.dll", _DesPath + "\\SupportLibrary.dll");
            FileHelper.CopyFile("log4net.dll", _DesPath + "\\log4net.dll");

            return true;
        }

    }
}
