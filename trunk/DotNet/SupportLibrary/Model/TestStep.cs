using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupportLibrary
{
    //*********************************************************************************
    //
    //
    //
    //  TestStep.cs
    //
    //*********************************************************************************

    using System.Linq;
    using System.ComponentModel;

    [ExcelSheet(Name = "TStep")]
    public class TestStep : INotifyPropertyChanged
    {

        private string _id;
        private string _testcase;
        private string _keyword;
        private string _parameters;
        private string _output;
        private string _comment;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        [ExcelColumn(Name = "ID", Storage = "_id")]
        public string ID
        {
            get { return _id; }
            set
            {
                _id = value;
                SendPropertyChanged("ID");
            }
        }

        [ExcelColumn(Name = "Comment", Storage = "_comment")]
        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                SendPropertyChanged("Comment");
            }
        }

        [ExcelColumn(Name = "TestCase", Storage = "_testcase")]
        public string TestCase
        {
            get { return _testcase; }
            set
            {
                _testcase = value;
                SendPropertyChanged("TestCase");
            }
        }

        [ExcelColumn(Name = "Keyword", Storage = "_keyword")]
        public string Keyword
        {
            get { return _keyword; }
            set
            {
                _keyword = value;
                SendPropertyChanged("Keyword");
            }
        }

        [ExcelColumn(Name = "Parameters", Storage = "_parameters")]
        public string Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value;
                SendPropertyChanged("Parameters");
            }
        }

        [ExcelColumn(Name = "Output", Storage = "_output")]
        public string Output
        {
            get { return _output; }
            set
            {
                _output = value;
                SendPropertyChanged("Output");
            }
        }

        public bool isValid()
        {
            if (_id == null || _testcase == null || _keyword == null )
                return false;
            if (_id.Equals("") || _testcase.Equals("") || _keyword.Equals(""))
                return false;
            if (_output == null)
                _output = "";
            if (_parameters == null)
                _parameters = "";
            if (_comment == null)
                _comment = "";
            return true;
        }
    }

}
