using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportLibrary;
using System.Configuration;
namespace Driver
{
    public class TestSuite
    {
        private string _Name { get; set; }
        private string _Behavior = null;//SEQUENCE - BYPASS
        private List<TestStep> _listTStep = new List<TestStep>();
        private TestCase _TCase = new TestCase();
        private List<Variable> _ModuleVariables = new List<Variable>();
        private string _State = null;
        private List<TestData> _listTDatas = new List<TestData>();

        public TestSuite(List<TestStep> listTStep, TestCase TCase)
        {
            if (ComponentFunction.NotInitialized()) {
                Console.WriteLine("You have to initialize ComponentFunction first.");
                throw new Exception("Not initialize ComponentFunction yet!");
            }
            _TCase = TCase;
            _Name = TCase.Name;
            _Behavior = TCase.Behavior;
            //Default: Behavior = Sequence
            if (_Behavior.Equals(""))
                _Behavior = Glossary.SEQUENCE;
            _State = TCase.State;
            if (_State.Equals(""))
                _State = Glossary.NOTRUN;
            foreach (var item in listTStep)
            {
                if (item.TestCase.Equals(_Name))
                    _listTStep.Add(item);
            }
        }

        public void inTData(List<TestData> datas) {
            this._listTDatas = datas;
        }

        public void inTData(TestData data) {
            
            this._listTDatas.Add(data);
        }

        public List<TestData> Run() {
            foreach (TestData tdata in _listTDatas)
            {
                if (tdata.Result.Equals("") || tdata.Result.Equals(Glossary.NOTRUN))
                {
                    
                    string temp = "";
                    string description = string.Empty;
                    Dictionary<string, string> listTConfig = new Dictionary<string, string>();
                    string excelPath = ConfigurationSettings.AppSettings["ExcelFile"];
                    ExcelDataAccess exceldataAccess = new ExcelDataAccess(excelPath);
                    listTConfig = exceldataAccess.getTestConfig();
                    if (Debug(tdata.Values, ref temp, ref description, listTConfig))
                    {
                        tdata.Result = Glossary.PASS;
                    }
                    else
                    {
                        tdata.Result = Glossary.FAILURE;
                        tdata.Comment = temp;
                    }                        
                }
            }
            return _listTDatas;
        }

        public string getID() {
            return _TCase.ID;
        }

        public string getName() {
            return _Name;
        }

        public string getParameters()
        {
            return _TCase.Parameters;
        }

        public string getBehavior()
        {
            return _Behavior;
        }

        public string getState()
        {
            return _State;
        }

        public List<TestStep> getSteps()
        {
            return _listTStep;
        }

        public bool Construct(ref string ErrorMes) {
            if (Validate(ref ErrorMes))
                if (InitVariables(ref ErrorMes))
                {
                    SetAllStepsValid();
                    return true;
                }
            return false;
        }

        private void SetAllStepsValid() {
            foreach (var _step in _listTStep) {
                if(!_step.Comment.Equals(""))
                    _step.Comment = Glossary.VALID;
            }
        }
        
        private bool Validate(ref string ErrorMes) {
            if (_listTStep.Count == 0)
            {
                ErrorMes = ErrorMessage.NO_RECORD;
                return false;
            }
            else { 
                //Check keyword exist:
                foreach (var item in _listTStep) {
                    if (!ComponentFunction.KeywordExist(item.Keyword))
                    {                        
                        ErrorMes = ErrorMessage.NO_MAPPING_KEYWORD;
                        item.Comment = ErrorMes;
                        return false;
                    }
                }
                //Check parameter:
                //Not checking for none input param TestCase.
                List<string> Params = util.getListValueFromString(_TCase.Parameters);
                foreach (var item in _listTStep) {
                    //Check number of argument input = number of argument Command mapping with keyword.
                    List<string> _tempParams = util.getListValueFromString(item.Parameters);
                    int n = _tempParams.Count;
                    if (n == 1 && _tempParams[0].Equals(""))
                        n = 0;
                    int nArgument = ComponentFunction.NumberOfArgument(item.Keyword);
                    if (n != nArgument)
                    {
                        ErrorMes = ErrorMessage.INVALID_METHOD_ARGUMENT;
                        item.Comment = ErrorMes;
                        return false;
                    }
                    //Check parameter exist:                   
                    for (int i = 0; i < n; i++) {
                        if (!Params.Contains(_tempParams[i])) {
                            ErrorMes = ErrorMessage.NOT_EXIST_VARIABLE;
                            item.Comment = ErrorMes;
                            return false;
                        }
                    }
                    //Add output to Params
                    if (!item.Output.Equals("")) {
                        if (ComponentFunction.isVoid(item.Keyword))
                        {
                            ErrorMes = "The method mapping for Keyword="+item.Keyword+"is void.";
                            item.Comment = ErrorMes;
                            return false;
                        }
                        else
                        {
                            Params.Add(item.Output);
                        }
                    }
                }                
            }
            return true;
        }


        public bool InitVariables(ref string ErrorMes)
        {
            List<string> Params = util.getListValueFromString(_TCase.Parameters);
            int n = Params.Count;
            //Input parameters:
            for (int i = 0; i < n; i++)
            {
                bool flag = true;
                foreach (var step in _listTStep)
                {                    
                    List<string> temp = util.getListValueFromString(step.Parameters);
                    if (temp.Count == 1 && temp[0].Equals("")) {
                        continue;
                    }
                    int ntemp = temp.Count;
                    for (int j = 0; j < ntemp; j++) {                        
                            if (temp[j].Equals(Params[i]))
                            {
                                List<string> ArgumentTypes = null;
                                ComponentFunction.getArgumentInfo(step.Keyword, ref ArgumentTypes);
                                if (ArgumentTypes != null)
                                {
                                    //ArgumentTypes.Count == 0 : The keyword have no argument.
                                    if (ArgumentTypes.Count != 0)
                                    {
                                        Variable _tvar = new Variable(Params[i]);
                                        _tvar.Type = ArgumentTypes[j];
                                        _ModuleVariables.Add(_tvar);                                        
                                        flag = false;
                                        break;
                                    }
                                    else
                                    {
                                        ErrorMes = ErrorMessage.INVALID_METHOD_ARGUMENT;
                                        step.Comment = ErrorMes;
                                        return false;
                                    }
                                }
                                else
                                {
                                    ErrorMes = ErrorMessage.NO_MAPPING_KEYWORD;
                                    step.Comment = ErrorMes;
                                    return false;
                                }
                            }
                        
                    }
                    if (!flag)
                        break;
                }
            }      
            //Output parameters:
            foreach(TestStep step in _listTStep){
                if (!step.Output.Equals(""))
                {
                    string type = ComponentFunction.ReturnType(step.Keyword);
                    if (type != null) {
                        bool flag = true;
                        foreach (Variable item in _ModuleVariables) {
                            if (step.Output.Equals(item.Name))
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            Variable var = new Variable(step.Output);
                            var.Type = type;
                            _ModuleVariables.Add(var);
                        }
                    }
                }
            }
            return true;
        }



        public bool Debug(string values, ref string ErrorMes, ref string description, Dictionary<string, string> listConfig, object _InvokeParam1 = null)
        {
            //Fix
            List<StringReplace> listReplace = util.getReplaceString(ref values);
            //Fix

            List<Variable> ModuleVariables = _ModuleVariables;            
            //init values:
            List<string> inputParams = util.getListValueFromString(_TCase.Parameters);
            int nArgument = inputParams.Count;
            if (nArgument == 1 && inputParams[0].Equals(""))
            {
                nArgument = 0;
            }
            List<string> listvalues = util.getListValueFromString(values);
            int n = listvalues.Count;
            if (n == 1 && listvalues[0].Equals(""))
            {
                n = 0;
            }
            if (nArgument != n)
            {
                //error mes
                ErrorMes ="Initialize variables:"+ ErrorMessage.INVALID_METHOD_ARGUMENT;
                description += "<error>" + ErrorMes + "</error>";
                return false;
            }
            
            for (int i = 0; i < n; i++) {
                foreach (StringReplace strReplace in listReplace)
                {
                    if (listvalues[i].Equals(strReplace.Replace))
                    {
                        listvalues[i] = strReplace.Value;
                        break;
                    }
                }
                string value = listvalues[i];

                string name = inputParams[i];
                foreach (var variable in _ModuleVariables) {
                    if (variable.Name.Equals(name)) {
                        object temp = null;
                        if (ComponentFunction.getOdject(variable.Type, value, ref temp))
                        {
                            variable.Value = temp;
                            break;
                        }
                        else {
                            ErrorMes = "Initialize variables:" + 
                                ErrorMessage.CONVERSION_TYPE;
                            //error mes
                            description += "<error>" + ErrorMes + "</error>";
                            return false;
                        }
                    }
                }
            }
            //Run each step
            ErrorMes = "";
            bool flag = true;

            foreach (TestStep step in _listTStep) {                    
                string output = null;
                if (!String.IsNullOrWhiteSpace(step.Output))
                {
                    output = step.Output;
                }
                List<string> listParams = util.getListValueFromString(step.Parameters);
                string InputParams = "";
                if (n == 0)
                {
                    Console.WriteLine("Method has no arguments.");
                }
                else
                {
                    foreach (string str in listParams)
                    {
                        if (!String.IsNullOrWhiteSpace(str))
                        {
                            Variable temp = ModuleVariables.Single(w => w.Name.Equals(str));
                            string Variable = temp.Name + " = " + temp.Value.ToString();
                            InputParams += Variable + "\t";
                            Console.WriteLine(Variable);
                        }
                    }
                }
                object outputObject = null;
                if (ComponentFunction.ExcuteFunction(step.Keyword, listParams, output, ref ModuleVariables, ref ErrorMes, ref outputObject, listConfig, _InvokeParam1))
                {//Chay step thanh cong:
                    Console.WriteLine("Step = " + step.ID + ".");
                    Console.WriteLine("Excute keyword =" + step.Keyword + ".");
                    Console.WriteLine("Input:");
                    //Have no input argument                   
                    string result = "";
                    if (output!=null) {
                        Variable temp = ModuleVariables.Single(w => w.Name.Equals(output));
                        Console.WriteLine("Result:");
                        result = temp.Name + " = " + temp.Value.ToString();
                        Console.WriteLine(result);
                    }
                    MessageShow.StepPass(step.Keyword,ComponentFunction.getCommandInfo(step.Keyword).Function,result,InputParams);
                }else {//Chay step that bai:
                    Console.WriteLine(ErrorMes);
                    //FileIO.MakeErrorLog(ErrorMes);
                    string temp = ErrorMessage.USERCODE + " at stepID =" + step.ID + ":" + ErrorMes + ".";
                    MessageShow.StepFail(step.Keyword, ComponentFunction.getCommandInfo(step.Keyword).Function, InputParams, temp);
                    //Behavior here.                    
                    if (_Behavior.Equals(Glossary.SEQUENCE))
                    {                          
                        ErrorMes = temp;
                        description += "<error>" + ErrorMes + "</error>";
                        return false;
                    }
                    else
                    {                        
                        ErrorMes += temp;
                        flag = false;
                    }                    
                }

                if (outputObject == null)
                {
                    outputObject = string.Empty;
                }

                if (!outputObject.ToString().ToLower().Equals("PASS".ToLower()))
                {
                    description += "<" + step.Keyword.ToString() + ">" + outputObject.ToString() + "</" + step.Keyword.ToString() + ">";
                }
            }
            Console.WriteLine("Excute value="+values+" SUCCESSFUL.");
            return flag;
        }

       

        public List<string> getInputArgumentsTypeInfo() {
            List<string> listvalues = util.getListValueFromString(_TCase.Parameters);            
            int n = listvalues.Count;
            List<string> rs = new List<string>();
            if (n == 1 && listvalues[0].Equals(""))
                n = 0;
            if (n == 0) {
                string str = "Method have no arguments.";
                rs.Add(str);
            }
            for (int i = 0; i < n; i++) {
                Variable temp = _ModuleVariables.Single(w => w.Name.Equals(listvalues[i]));
                string _str = "Param "+(i+1)+":"+temp.Type + " " + temp.Name;
                rs.Add(_str);
            }
            return rs;
        }

    }
}
