using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportLibrary;
namespace Driver
{
    public class ExcelDataAccess
    {
        private ExcelProvider _exProvider;
        private string _Path;
        public ExcelDataAccess(string path) {
            _Path = path;
            _exProvider = ExcelProvider.Create(@path);
            if (_exProvider == null)
                MessageShow.FileDoesNotExist(path);
        }

        public void RefreshTDatas() {
            try
            {
                if (_exProvider != null)
                {
                    var TestDatas = _exProvider.GetSheet<TestData>();
                    
                    if (TestDatas != null)
                    {
                        foreach (TestData item in TestDatas)
                        {
                            if (item.ID != null)
                            {                                
                                if (item.isValid())
                                {
                                    if (!item.Result.Equals("NOTRUN")) {
                                        item.Result = "NOTRUN";
                                        try
                                        {
                                            _exProvider.GetSheet<TestData>().UpdateOnSubmit(item);
                                        }
                                        catch (Exception e) { }
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }
            catch (Exception e)
            {
                MessageShow.ExceptionOccur("Read TData sheet exception.", e);
            }
        }

        public int getTestDataIndex() {
            int i = 0;
            try
            {
                if (_exProvider != null)
                {
                    var TestDatas = _exProvider.GetSheet<TestData>();
                    if (TestDatas != null)
                    {
                        foreach (TestData item in TestDatas)
                        {
                            if (item.ID != null)
                                i++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageShow.ExceptionOccur("Read TData sheet exception.", e);
            }
            return i+1;
        }

        public List<TestCase> getTestcases() {
            List<TestCase> list = new List<TestCase>();
            Console.WriteLine("READING SHEET TCase.");
            try
            {
                if (_exProvider != null)
                {
                    var TestCases = _exProvider.GetSheet<TestCase>();
                    int i = 0;
                    if (TestCases != null)
                    {
                        foreach (TestCase item in TestCases)
                        {
                            if (item.ID != null)
                            {
                                i++;
                                if (item.isValid())
                                {
                                    int n = list.Count;
                                    for (int j = 0; j < n; j++) {
                                        if (list[j].Name.Equals(item.Name)) {
                                            MessageShow.TCaseNameExist(item.Name);
                                            return null;
                                        }
                                    }
                                   list.Add(item);
                                }
                                else
                                {
                                    MessageShow.InvalidRecord(i + 1);
                                }
                            }
                        }
                    }
                    Console.WriteLine(i + " records were read in sheet TCase.");
                }
            }
            catch (Exception e) {
                MessageShow.ExceptionOccur("Read TCase sheet exception.", e);
            }
            
            return list;
        }

        public List<TestData> getTestdatas() {
            List<TestData> list = new List<TestData>();
            Console.WriteLine("READING SHEET TData.");
            try
            {
                if (_exProvider != null)
                {
                    var TestDatas = _exProvider.GetSheet<TestData>();
                    int i = 0;
                    if (TestDatas != null)
                    {
                        foreach (TestData item in TestDatas)
                        {
                            if (item.ID != null)
                            {
                                i++;
                                if (item.isValid())
                                    list.Add(item);
                                else
                                {
                                    MessageShow.InvalidRecord(i + 1);
                                }
                            }
                        }
                    }
                    Console.WriteLine(i + " records were read in sheet TData.");
                }
            }
            catch (Exception e)
            {
                MessageShow.ExceptionOccur("Read TData sheet exception.", e);
            }            
            return list;
        }

        //this function is added by SonNguyen, not ogriginal framework.
        public Dictionary<string, string> getTestConfig()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();

            try
            {
                if (_exProvider != null)
                {
                    var TestConfigs = _exProvider.GetSheet<TestConfig>();
                    int i = 0;
                    if (TestConfigs != null)
                    {
                        foreach (TestConfig item in TestConfigs)
                        {
                            if (item.ID != null)
                            {
                                i++;
                                if (item.isValid())
                                {
                                    list.Add(item.Name, item.Value);
                                }
                                else
                                {
                                    MessageShow.InvalidRecord(i + 1);
                                }
                            }
                        }
                    }
                    Console.WriteLine(i + " records were read in sheet TConfig.");
                }
            }
            catch(Exception e)
            {
                MessageShow.ExceptionOccur("Read TConfig sheet exception.", e);
            }

            return list;
        }
        
        // This function is added by QuangNguyen to get List TConfig
        public List<TestConfig> getTestConfigs()
        {
            List<TestConfig> list = new List<TestConfig>();

            try
            {
                if (_exProvider != null)
                {
                    var TestConfigs = _exProvider.GetSheet<TestConfig>();
                    int i = 0;
                    if (TestConfigs != null)
                    {
                        foreach (TestConfig item in TestConfigs)
                        {
                            if (item.ID != null)
                            {
                                i++;
                                if (item.isValid())
                                {
                                    list.Add(item);
                                }
                                else
                                {
                                    MessageShow.InvalidRecord(i + 1);
                                }
                            }
                        }
                    }
                    Console.WriteLine(i + " records were read in sheet TConfig.");
                }
            }
            catch (Exception e)
            {
                MessageShow.ExceptionOccur("Read TConfig sheet exception.", e);
            }

            return list;
        }

        public List<TestCommand> getTestcommands() {
            List<TestCommand> list = new List<TestCommand>();
            Console.WriteLine("READING SHEET TCommand.");
            try
            {
                if (_exProvider != null)
                {
                    var TestCommands = _exProvider.GetSheet<TestCommand>();
                    int i = 0;
                    if (TestCommands != null)
                    {                       
                        foreach (var item in TestCommands)
                        {
                            if (item.ID != null)
                            {
                                i++;
                                if (item.isValid())
                                {
                                    int n = list.Count;
                                    for (int j = 0; j < n; j++)
                                    {
                                        if (list[j].Keyword.Equals(item.Keyword))
                                        {
                                            MessageShow.KeywordExist(item.Keyword);
                                            return null;
                                        }
                                    }
                                    list.Add(item);
                                }
                                else
                                {
                                    MessageShow.InvalidRecord(i + 1);
                                }
                            }
                        }
                    }
                    Console.WriteLine(i + " records were read in sheet TCommand.");
                }
            }
            catch (Exception e)
            {
                MessageShow.ExceptionOccur("Read TCommand sheet exception.", e);
            }            
            return list;
        }

        public List<TestDataset> getTestdatasets() {
            List<TestDataset> list = new List<TestDataset>();
            Console.WriteLine("READING SHEET TDataset.");
            try
            {
                if (_exProvider != null)
                {
                    var TestDatasets = _exProvider.GetSheet<TestDataset>();
                    int i = 0;
                    if (TestDatasets != null)
                    {
                        foreach (TestDataset item in TestDatasets)
                        {
                            if (item.ID != null)
                            {
                                i++;
                                if (item.isValid())
                                    list.Add(item);
                                else
                                {
                                    MessageShow.InvalidRecord(i + 1);
                                }
                            }
                        }
                    }
                    Console.WriteLine(i + " records were read in sheet TDataset.");
                }
            }
            catch (Exception e)
            {
                MessageShow.ExceptionOccur("Read TDataSet sheet exception.", e);
            }         
            return list;
        }

        public List<TestStep> getTeststeps() {
            List<TestStep> list = new List<TestStep>();
            Console.WriteLine("READING SHEET TStep.");
            try
            {
                if (_exProvider != null)
                {
                    var TestSteps = _exProvider.GetSheet<TestStep>();
                    int i = 0;
                    if (TestSteps != null)
                    {
                        foreach (TestStep item in TestSteps)
                        {
                            if (item.ID != null)
                            {
                                i++;
                                if (item.isValid())
                                    list.Add(item);
                                else
                                {
                                    MessageShow.InvalidRecord(i + 1);
                                }
                            }
                        }
                    }
                    Console.WriteLine(i + " records were read in sheet TStep.");
                }
            }
            catch (Exception e)
            {
                MessageShow.ExceptionOccur("Read TStep sheet exception.", e);
            }            
            return list;
        }

        public void GenerateCommands(List<TestCommand> Commands) {
            try
            {
                if (_exProvider != null)
                {
                    int count = 0;
                    foreach (var item in Commands)
                    {                        
                        _exProvider.GetSheet<TestCommand>().InsertOnSubmit(item);
                        count++;
                    }                   
                }
            }
            catch (Exception e)
            {
                MessageShow.ExceptionOccur("Generate TCommand sheet exception.", e);
            }            
        }

        public void UpdateSteps(List<TestStep> Steps)
        {
            try
            {
                if (_exProvider != null)
                {
                    foreach (var item in Steps)
                    {
                        try
                        {
                            _exProvider.GetSheet<TestStep>().InsertOnSubmit(item);
                        }
                        catch (Exception ex) { 
                            
                        }
                    }

                }
            }
            catch (Exception e)
            {
                MessageShow.ExceptionOccur("Update TStep sheet exception.", e);
            }
        }

        public void UpdateDatas(List<TestData> Datas)
        {
            try
            {
                if (_exProvider != null)
                {

                    foreach (var item in Datas)
                    {
                        try
                        {
                            int validNumber = 255;//255 characters
                            int Num = item.Comment.Length;
                            if (Num >= validNumber)
                            {
                                string comment = item.Comment.Remove(validNumber - 1);
                                item.Comment = comment;
                            }
                            _exProvider.GetSheet<TestData>().InsertOnSubmit(item);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                }
            }
            catch (Exception e)
            {
                MessageShow.ExceptionOccur("Update TStep sheet exception.", e);
            }
        }


        public void GenerateTestdatas(List<TestData> tdatas)
        {
            try
            {
                if (_exProvider != null)
                {
                    foreach (var item in tdatas)
                    {
                        _exProvider.GetSheet<TestData>().InsertOnSubmit(item);
                    }                   
                }
            }
            catch (Exception e)
            {
                MessageShow.ExceptionOccur("Generate TData sheet exception.", e);
            }
        }

        public void Close() {
            _exProvider.SubmitChanges();
        }
    }
}
