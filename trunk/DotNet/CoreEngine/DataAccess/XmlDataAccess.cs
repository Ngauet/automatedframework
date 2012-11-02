using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SupportLibrary;
namespace Driver
{
    public class XmlDataAccess
    {
        private string _Path = null;
        private XElement _xElement = null;
        public XmlDataAccess(string path)
        {
            _Path = path;
            _xElement = XElement.Load(path);
        }

        public List<TestSuite> getTestSuites()
        {
            List<TestSuite> rs = new List<TestSuite>();
            foreach (var item in _xElement.Elements("TestCase"))
            {
                TestCase tcase = new TestCase();
                List<TestStep> listStep = new List<TestStep>();
                tcase.Name = item.Attribute("Name").Value;
                tcase.Parameters = item.Attribute("Parameters").Value;
                tcase.Behavior = item.Attribute("Behavior").Value;
                tcase.State = item.Attribute("State").Value;
                tcase.ID = item.Attribute("ID").Value;
                foreach (var step in item.Elements("TestStep"))
                {
                    TestStep temp_step = new TestStep();
                    temp_step.TestCase = tcase.Name;
                    temp_step.ID = step.Attribute("ID").Value;
                    temp_step.Keyword = step.Attribute("Keyword").Value;
                    temp_step.Parameters = step.Attribute("Parameters").Value;
                    temp_step.Output = step.Attribute("Output").Value;
                    temp_step.Comment = "";
                    listStep.Add(temp_step);
                }
                rs.Add(new TestSuite(listStep, tcase));
            }
            return rs;
        }

        public List<TestCommand> getTestCommands()
        {
            List<TestCommand> rs = new List<TestCommand>();
            foreach (var item in _xElement.Elements("TestCommand"))
            {
                TestCommand temp = new TestCommand();
                temp.Keyword = item.Attribute("Keyword").Value;
                temp.Object = item.Attribute("Object").Value;
                temp.Target = item.Attribute("Target").Value;
                temp.Function = item.Attribute("Function").Value;
                temp.ID = item.Attribute("ID").Value;
                rs.Add(temp);
            }
            return rs;
        }

        public List<TestConfig> getTestConfigs()
        {
            List<TestConfig> rs = new List<TestConfig>();
            foreach (var item in _xElement.Elements("TestConfig"))
            {
                TestConfig temp = new TestConfig();
                temp.Name = item.Attribute("Name").Value;
                temp.Value = item.Attribute("Value").Value;
                temp.ID = item.Attribute("ID").Value;
                rs.Add(temp);
            }
            return rs;
        }

        public Dictionary<string, string> getTestConfig()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (var item in _xElement.Elements("TestConfig"))
            {
                TestConfig temp = new TestConfig();
                temp.Name = item.Attribute("Name").Value;
                temp.Value = item.Attribute("Value").Value;
                temp.ID = item.Attribute("ID").Value;
                list.Add(temp.Name, temp.Value);
            }

            return list;
        }
        public List<string> getValues(string Testcase)
        {
            try
            {
                List<string> rs = new List<string>();
                foreach (var item in _xElement.Elements("TestCase"))
                {
                    string Name = item.Attribute("Name").Value;
                    if (Name.Equals(Testcase))
                    {
                        foreach (var data in item.Elements("TestData"))
                        {
                            string values = data.Attribute("Values").Value;
                            rs.Add(values);
                        }
                        break;
                    }
                }
                return rs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public bool BuildReport(List<TestData> listTData)
        {
            int n = listTData.Count;
            if (n > 0)
            {
                XElement root = new XElement("Report", new XAttribute("Date",util.getDateTime()));
                string tCaseName = "";
                XElement Testcase = null;
                string lastTestCase = "";
                for (int i = 0; i < n; i++)
                {
                    TestData tdata = listTData[i];
                    int validNumber = 255;//255 characters
                    int Num = tdata.Comment.Length;
                    if (Num >= validNumber)
                    {
                        string comment = tdata.Comment.Remove(validNumber - 1);
                        tdata.Comment = comment;
                    }
                    if (tCaseName.Equals(tdata.TestCase))
                    {
                        XElement Testdata = new XElement("TestData", new XAttribute("Values", tdata.Values), new XAttribute("Result", tdata.Result), new XAttribute("Comment", tdata.Comment));
                        Testcase.Add(Testdata);
                    }
                    else
                    {
                        if (Testcase != null)
                        {
                            root.Add(Testcase);
                            lastTestCase = tdata.TestCase;
                        }                        
                        tCaseName = tdata.TestCase;
                        Testcase = new XElement("TestCase", new XAttribute("Name", tCaseName));
                        XElement Testdata = new XElement("TestData", new XAttribute("Values", tdata.Values), new XAttribute("Result", tdata.Result), new XAttribute("Comment", tdata.Comment));
                        Testcase.Add(Testdata);                       
                    }
                    if (i == n - 1)
                    {
                        root.Add(Testcase);
                    }
                }
                if (String.IsNullOrEmpty(lastTestCase) && Testcase != null)
                    root.Add(Testcase);

                _xElement.Add(root);
                _xElement.Save(_Path);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
