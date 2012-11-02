using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SupportLibrary;
namespace Driver
{
    public class XMLBuilder
    {
        private string _Path = null;
        public XMLBuilder(string path) {
            _Path = path;
        }
        public bool BuildRelease(List<TestSuite> ListTSuites,List<TestCommand> ListTCommads,string PathApp){
            if (_Path != null)
            {
                XElement root = new XElement("Config");
                int n = ListTCommads.Count;
                for (int i = 0; i < n; i++)
                {
                    TestCommand tam = ListTCommads[i];
                    string _str = tam.Object;
                    string releaseObj = PathApp+util.getFileFromPath(_str);
                    XElement Command = new XElement("TestCommand", new XAttribute("ID",tam.ID),new XAttribute("Keyword", tam.Keyword), new XAttribute("Object", releaseObj), new XAttribute("Target", tam.Target), new XAttribute("Function", tam.Function));
                    root.Add(Command);
                }                

                int nTSuite = ListTSuites.Count;
                for(int i = 0;i<nTSuite;i++){
                    TestSuite tam = ListTSuites[i];
                    XElement TCase = new XElement("TestCase", new XAttribute("ID", tam.getID()), new XAttribute("Name", tam.getName()), new XAttribute("Parameters", tam.getParameters()), new XAttribute("Behavior", tam.getBehavior()), new XAttribute("State", tam.getState()));
                    List<TestStep> listSteps = tam.getSteps();
                    foreach(TestStep step in listSteps){
                        XElement TStep = new XElement("TestStep",new XAttribute("ID",step.ID),new XAttribute("Keyword",step.Keyword),new XAttribute("Parameters",step.Parameters),new XAttribute("Output",step.Output));
                        TCase.Add(TStep);
                    }
                    root.Add(TCase);
                }
                root.Save(_Path);
                return true;
            }
            return false;
        }

        public bool BuildRelease(List<TestSuite> ListTSuites, List<TestCommand> ListTCommads, List<TestConfig> ListTConfigs)
        {
            if (_Path != null)
            {
                XElement root = new XElement("Config");
                int nTconfig = ListTConfigs.Count;
                for (int i = 0; i < nTconfig; i++)
                {
                    TestConfig tam = ListTConfigs[i];
                                        
                    XElement Command = new XElement("TestConfig", new XAttribute("ID", tam.ID), new XAttribute("Name", tam.Name), new XAttribute("Value",tam.Value));
                    root.Add(Command);
                }


                int n = ListTCommads.Count;
                for (int i = 0; i < n; i++)
                {
                    TestCommand tam = ListTCommads[i];
                    string _str = tam.Object;
                    string releaseObj = tam.Object;
                    XElement Command = new XElement("TestCommand", new XAttribute("ID", tam.ID), new XAttribute("Keyword", tam.Keyword), new XAttribute("Object", releaseObj), new XAttribute("Target", tam.Target), new XAttribute("Function", tam.Function));
                    root.Add(Command);
                }

                int nTSuite = ListTSuites.Count;
                for (int i = 0; i < nTSuite; i++)
                {
                    TestSuite tam = ListTSuites[i];
                    XElement TCase = new XElement("TestCase", new XAttribute("ID", tam.getID()), new XAttribute("Name", tam.getName()), new XAttribute("Parameters", tam.getParameters()), new XAttribute("Behavior", tam.getBehavior()), new XAttribute("State", tam.getState()));
                    List<TestStep> listSteps = tam.getSteps();
                    foreach (TestStep step in listSteps)
                    {
                        XElement TStep = new XElement("TestStep", new XAttribute("ID", step.ID), new XAttribute("Keyword", step.Keyword), new XAttribute("Parameters", step.Parameters), new XAttribute("Output", step.Output));
                        TCase.Add(TStep);
                    }
                    root.Add(TCase);
                }
                root.Save(_Path);
                return true;
            }
            return false;
        }

        public bool BuildInputData(List<TestSuite> ListTSuites,List<TestData> ListTDatas )
        {
            if(_Path!=null){
                XElement root = new XElement("Input");
            foreach (TestSuite tsuite in ListTSuites) {
                XElement TCase = new XElement("TestCase",new XAttribute("ID",tsuite.getID()) ,new XAttribute("Name", tsuite.getName()));
                foreach(TestData tdata in ListTDatas){
                    if (tdata.TestCase.Equals(tsuite.getName()))
                    {
                        XElement TData = new XElement("TestData", new XAttribute("Values", tdata.Values));
                        TCase.Add(TData);
                    }
                }
                root.Add(TCase);
            }
            root.Save(_Path);
            return true;
            }
            return false;            
        }

        public bool BuildReport() {
            if (_Path != null)
            {
                XElement root = new XElement("ReportBuild",new XAttribute("Date",util.getDateTime()));
                root.Save(_Path);
                return true;
            }
            return false;
        }

    }
}
