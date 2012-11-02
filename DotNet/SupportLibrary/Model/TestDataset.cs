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
    //  TestDataset.cs
    //
    //*********************************************************************************

    using System.Linq;
    using System.ComponentModel;

    [ExcelSheet(Name = "TDataSet")]
    public class TestDataset : INotifyPropertyChanged
    {

        private string _id;
        private string _testcase;
        private string _parameter;
        private string _values;
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

        [ExcelColumn(Name = "Parameter", Storage = "_parameter")]
        public string Parameter
        {
            get { return _parameter; }
            set
            {
                _parameter = value;
                SendPropertyChanged("Parameter");
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
            if (_id == null || _testcase == null || _parameter == null)
                return false;
            if (_values == null)
                _values = "";
            if (_id.Equals("") || _testcase.Equals("") || _parameter.Equals(""))
                return false;
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
