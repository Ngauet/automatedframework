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
    //  TestData.cs
    //
    //*********************************************************************************

    using System.Linq;
    using System.ComponentModel;

    [ExcelSheet(Name = "TData")]
    public class TestData : INotifyPropertyChanged
    {

        private string _id;
        private string _testcase;
        private string _values;
        private string _result;
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

        [ExcelColumn(Name = "Values", Storage = "_values")]
        public string Values
        {
            get { return _values; }
            set
            {
                _values = value;
                SendPropertyChanged("Values");
            }
        }

       

        [ExcelColumn(Name = "Result", Storage = "_result")]
        public string Result
        {
            get { return _result; }
            set
            {
                _result = value;
                SendPropertyChanged("Result");
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

        public bool isValid()
        {           
            if (_id == null || _testcase == null)
                return false;           
            if (_id.Equals("") || _testcase.Equals(""))
                return false;
            if (_values == null)
                _values = "";
           
            if (_result == null)
                _result = "";
            if (_comment == null)
                _comment = "";
            if (!_values.Equals(""))
            {
                if (_values[0].Equals('"'))
                {
                    _values = _values.Remove(0, 1);
                }
            }
            int n_val = _values.Length;
            if (!_values.Equals(""))
            {
                if (_values[n_val - 1].Equals('"'))
                {
                    _values = _values.Remove(n_val - 1, 1);
                }
            }           
            return true;
        }
    }

}
