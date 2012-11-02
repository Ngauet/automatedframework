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
    //  TestConfig.cs  ------------->This class is added by SonNguyen, not ogriginal framework.
    //
    //*********************************************************************************
    using System.Linq;
    using System.ComponentModel;

    [ExcelSheet(Name = "TConfig")]
    public class TestConfig : INotifyPropertyChanged
    {
        private string _id;
        private string _name;
        private string _value;

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

        [ExcelColumn(Name = "Name", Storage = "_name")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                SendPropertyChanged("Name");
            }
        }

        [ExcelColumn(Name = "Value", Storage = "_value")]
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                SendPropertyChanged("Value");
            }
        }

        public bool isValid()
        {
            if (_id == null || _name == null || _value == null)
                return false;
            if (_id.Equals("") || _name.Equals("") || _value.Equals(""))
                return false;
            return true;
        }
    }
}
