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
    //  TestCommand.cs
    //
    //*********************************************************************************

    using System.Linq;
    using System.ComponentModel;

    [ExcelSheet(Name = "TCommand")]
    public class TestCommand : INotifyPropertyChanged
    {

        private string _id;
        private string _keyword;
        private string _object;
        private string _target;
        private string _function;
        

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

        [ExcelColumn(Name = "Object", Storage = "_object")]
        public string Object
        {
            get { return _object; }
            set
            {
                _object = value;
                SendPropertyChanged("Object");
            }
        }

        [ExcelColumn(Name = "Target", Storage = "_target")]
        public string Target
        {
            get { return _target; }
            set
            {
                _target = value;
                SendPropertyChanged("Target");
            }
        }

        [ExcelColumn(Name = "Function", Storage = "_function")]
        public string Function
        {
            get { return _function; }
            set
            {
                _function = value;
                SendPropertyChanged("Function");
            }
        }
        
        public bool isValid()
        {           
            if (_id == null || _keyword == null || _object == null || _target == null || _function == null)
                return false;
            if (_id.Equals("") || _keyword.Equals("") || _object.Equals("") || _target.Equals("") || _function.Equals(""))
                return false;
            _keyword = _keyword.Replace(" ", "");
            return true;
        }
        
    }

}
