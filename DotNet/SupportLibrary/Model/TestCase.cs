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
    //  TestCase.cs
    //
    //*********************************************************************************

    using System.Linq;
    using System.ComponentModel;

    [ExcelSheet(Name = "TCase")]
    public class TestCase : INotifyPropertyChanged
    {

        private string _id;
        private string _name;
        private string _parameters;
        private string _behavior;
        private string _state;

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

        [ExcelColumn(Name = "Behavior", Storage = "_behavior")]
        public string Behavior
        {
            get { return _behavior; }
            set
            {
                _behavior = value;
                SendPropertyChanged("Behavior");
            }
        }

        [ExcelColumn(Name = "State", Storage = "_state")]
        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
                SendPropertyChanged("State");
            }
        }

        public bool isValid() {
            if (_id == null || _name == null || _behavior == null || _state == null)
                return false;
            if (_id.Equals("") || _name.Equals("") || _behavior.Equals("") || _state.Equals(""))
                return false;
            //Standardlize:
            if (_parameters == null)
                _parameters = "";
            _id = _id.Replace(" ", "");
            _name = _name.Replace(" ", "");
            _behavior = _behavior.Replace(" ", "");
            _state = _state.Replace(" ", "");
            _parameters = _parameters.Replace(" ", "");
            return true;
        }
    }

}
