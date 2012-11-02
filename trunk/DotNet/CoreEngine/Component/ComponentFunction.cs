using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportLibrary;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Configuration;

namespace Driver
{
    /**
     * ComponentFunction
     * Mapping keyword and method
    */
    public class ComponentFunction
    {
        /**
         * /////////////////////////////PRIVATE METHOD////////////////////////////////
         */
        private static List<TestCommand> Commands = null;
        private static List<ReflectMethod> Methods = null;

        private static void init(List<TestCommand> TestCommands) {
            Commands = TestCommands;
            Methods = new List<ReflectMethod>();
            foreach (var item in Commands) { 
                string MethodName = item.Function.Split('(')[0].Split(' ')[1];
                ReflectMethod temp = new ReflectMethod(item.Object, item.Target, MethodName, item.Keyword);
                Methods.Add(temp);
            }
        }

        private static object getOdject(string type, string value)
        {
            object rs = null;
            if (!type.Equals(ValueType.STRING)) {
                value = value.Trim();
            }
            switch (type)
            {
                case ValueType.INT:
                    try
                    {                        
                        rs = Int32.Parse(value);
                    }
                    catch (Exception e)
                    {
                        MessageShow.ExceptionOccur("Parse int error!", e);
                        return null;
                    }
                    break;
                case ValueType.STRING:
                    rs = value;
                    break;
                case ValueType.DOUBLE:
                    try
                    {
                        rs = double.Parse(value);
                    }
                    catch (Exception e)
                    {
                        MessageShow.ExceptionOccur("Parse double error!", e);
                        return null;
                    }
                    break;
                case ValueType.FLOAT://float
                    try
                    {
                        rs = float.Parse(value);
                    }
                    catch (Exception e)
                    {
                        MessageShow.ExceptionOccur("Parse float error!", e);
                        return null;
                    }
                    break;
                case ValueType.BOOL:
                    try
                    {
                        rs = bool.Parse(value);
                    }
                    catch (Exception e)
                    {
                        MessageShow.ExceptionOccur("Parse bool error!", e);
                        return null;
                    }
                    break;
                case ValueType.LONG:
                    try
                    {
                        rs = long.Parse(value);
                    }
                    catch (Exception e)
                    {
                        MessageShow.ExceptionOccur("Parse long error!", e);
                        return null;
                    }
                    break;
                case ValueType.CHAR:
                    try
                    {
                        rs = char.Parse(value);
                    }
                    catch (Exception e)
                    {
                        MessageShow.ExceptionOccur("Parse char error!", e);
                        return null;
                    }
                    break;
                case ValueType.ULONG:
                    try
                    {
                        rs = ulong.Parse(value);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Parse unsigned long error:" + e.ToString());
                        return null;
                    }
                    break;
                case ValueType.UINT:
                    try
                    {
                        rs = uint.Parse(value);
                    }
                    catch (Exception e)
                    {
                        MessageShow.ExceptionOccur("Parse unsigned int error!", e);
                        return null;
                    }
                    break;

                case ValueType.DECIMAL:
                    try
                    {
                        rs = Decimal.Parse(value);
                    }
                    catch (Exception e)
                    {
                        MessageShow.ExceptionOccur("Parse decimal error!", e);
                        return null;
                    }
                    break;

            }
            return rs;
        }

        /**
         * Return the Argument's type of the function mapping with Keyword
         * */
        private static List<string> getArgumentInfo(string Keyword)
        {
            List<string> rs = new List<string>();
            try
            {
                ReflectMethod meth = Methods.Single(w=>w._Keyword.Equals(Keyword));
                rs = meth.getArgumentsType();
            }
            catch (Exception e)
            {
                return null;
            }
            return rs;
        }
        /**
         * Return the methods in dll file
         * */
        public static List<TestCommand> GetAllMethodsDLLs(string dllPath)
        {
            Assembly _Assemblies = null;
            try
            {
                _Assemblies = Assembly.LoadFrom(dllPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\nError - couldn't obtain assemblies from " + dllPath);
                Console.WriteLine("EXCEPTION OUTPUT\n" + ex.Message + "\n" + ex.InnerException);
            }
            List<TestCommand> _Temp = new List<TestCommand>();
            if (_Assemblies != null)
            {
                Type[] _AllTypes = _Assemblies.GetTypes();              

                foreach (Type t in _AllTypes)
                {
                    int i = 0;
                    MethodInfo[] Methods = t.GetMethods();
                    foreach (MethodInfo meth in Methods)
                    {
                        bool flag = false;
                        object[] attributes = meth.GetCustomAttributes(false);
                        if (attributes.Length > 0) {
                            foreach (var item in attributes) {
                                List<string> _temp = util.getListValueWithToken(item.ToString(), '.');
                                int n = _temp.Count;
                                string AttributeName = "";
                                if (n > 1) { 
                                    AttributeName = _temp[n-1];
                                }
                                if (AttributeName.Equals("Automation"))
                                {
                                    flag = true;
                                }
                            }
                        }
                        if (flag)
                        {
                            i++;
                            TestCommand temp = new TestCommand();
                            temp.Object = dllPath;
                            temp.Target = t.ToString();
                            string function = getFunc(meth);
                            temp.Function = function;
                            temp.Keyword = meth.Name;
                            _Temp.Add(temp);
                        }
                    }
                }

            }
            return _Temp;
        }

        private static string getFunc(MethodInfo meth) {
            List<string> listTemp = util.getListValueWithToken(meth.ToString(), '(');
            string func = listTemp[0] + "(";
            ParameterInfo[] pis = meth.GetParameters();
            int n = pis.Length;
            if (n > 0)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    ParameterInfo info = pis[j];
                    func += info.ParameterType.Name + " " + info.Name+",";
                }
                ParameterInfo lastinfo = pis[n - 1];
                func += lastinfo.ParameterType.Name + " " + lastinfo.Name;
            }
            func += ")";
            return func;
        }
        /**
         * /////////////////////////////PUBLIC METHOD/////////////////////////////////
         */
        public static bool NotInitialized(){
            if (Commands == null)
                return true;
            return false;
        }

        public static void Initialize(List<TestCommand> TestCommands)
        {            
                init(TestCommands);
        }
        /**
        * Return the Argument's type of the function mapping with Keyword
         * @rs: null if the Keyword isnot exist (return false).
         * @rs: rs.Count = 0 if the function mapping with Keyword dont have argument.
        * */
        public static bool getArgumentInfo(string Keyword,ref List<string> ArgumentTypes){
            List<string> rs = getArgumentInfo(Keyword);
            if (rs == null)
                return false;
            ArgumentTypes = rs;
            return true;
        }

        /**
         * Return false if cannot convert value with type.
         * Return the obj with value and correct type.
         * */
        public static bool getOdject(string type, string value, ref object obj)
        {
            object temp = getOdject(type, value);
            if (temp == null)
                return false;
            obj = temp;
            return true;
        }
        
        /**
         * Excute function with input = values, then return output result.
         * Use this method to excute not void function.
         * */
        public static bool ExcuteFunction(string keyword, string values,ref string error_mes, ref object output)
        {                    
            try
            {
                List<StringReplace> listReplace = util.getReplaceString(ref values);
                
                ReflectMethod meth = Methods.Single(w => w._Keyword.Equals(keyword));
                List<string> listValue = util.getListValueFromString(values);
                int n = listValue.Count;
                if (n == 1 && listValue[0].Equals("")) {
                    n = 0;
                }
                
                List<string> ArgTypes = meth.getArgumentsType();
                if (n != ArgTypes.Count) {
                    error_mes = ErrorMessage.INVALID_METHOD_ARGUMENT;
                    return false;
                }
                for (int i = 0; i < n; i++)
                {
                    foreach (StringReplace strReplace in listReplace)
                    {
                        if (listValue[i].Equals(strReplace.Replace))
                        {
                            listValue[i] = strReplace.Value;
                            break;
                        }
                    }
                }
                object[] mParam = new object[n];                               
                for (int i = 0; i < n; i++)
                {
                    object tam = null;
                    if (!getOdject(ArgTypes[i], listValue[i], ref tam))
                    {
                        error_mes = ErrorMessage.CONVERSION_TYPE;
                        return false;
                    }
                    mParam[i] = tam;
                }
                string excuteMes = "";
                Dictionary<string, string> listTConfig = new Dictionary<string, string>();
                string excelPath = ConfigurationSettings.AppSettings["ExcelFile"];
                ExcelDataAccess exceldataAccess = new ExcelDataAccess(excelPath);
                listTConfig = exceldataAccess.getTestConfig();
                bool flag = meth.Excute(mParam, ref output, ref excuteMes, listTConfig);
                if (flag == false)
                {
                    error_mes = ErrorMessage.USERCODE + "\n" + excuteMes;
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                error_mes = ErrorMessage.USERCODE;
                return false;
            }             
        }
        
        /**
         * @outputParam: null if no output.
         * */
        public static bool ExcuteFunction(string keyword, List<string> inputParams, string outputParam, ref List<Variable> Variables, ref string error_mes, ref object outputObject, Dictionary<string, string> listConfig, object _InvokeParam1)
        {
            try
            {
                ReflectMethod meth = Methods.Single(w => w._Keyword.Equals(keyword));
                int n = inputParams.Count;
                if (n == 1 && inputParams[0].Equals(""))
                {
                    n = 0;
                }
                List<string> ArgTypes = meth.getArgumentsType();
                if (n != ArgTypes.Count)
                {
                    error_mes = ErrorMessage.INVALID_METHOD_ARGUMENT;
                    return false;
                }
                object[] mParam = new object[n];
                for (int i = 0; i < n; i++)
                {
                    string paramName = inputParams[i];
                    Variable variable = Variables.Single(w => w.Name.Equals(paramName));
                    mParam[i] = variable.Value;
                }
                
                string excuteMes = "";
                object output = null;
                bool flag = meth.Excute(mParam, ref output, ref excuteMes, listConfig, _InvokeParam1);
                if (flag == false)
                {
                    string Error = ErrorMessage.USERCODE + ":\n" + excuteMes;
                    error_mes = Error;
                    return false;
                }
                else
                {
                    try
                    {
                        outputObject = (object)output;
                    }
                    catch
                    {
                    }
                    if (outputParam != null)
                    {
                        foreach (var variable in Variables)
                        {
                            if (variable.Name.Equals(outputParam))
                            {
                                variable.Value = output;
                                return true;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e) {                
                error_mes = ErrorMessage.USERCODE;
                return false;
            }           
        }
      
        /**
         * Checking if the keyword is mapped to a void function
         * */
        public static bool isVoid(string keyword) {
            try
            {
                ReflectMethod meth = Methods.Single(w => w._Keyword.Equals(keyword));
                return meth.isVoid();
            }
            catch (Exception e)
            {
                MessageShow.ExceptionOccur("Checking return type of " + keyword + " fail.", e);
                return false;
            }            
        }

        public static string ReturnType(string keyword) {
            
            try
            {
                TestCommand item = Commands.Single(w => w.Keyword.Equals(keyword));
                Assembly assembly = Assembly.LoadFrom(@item.Object);
                Type tClass = assembly.GetType(item.Target);
                string MethodName = item.Function.Split('(')[0].Split(' ')[1];
                MethodInfo mMethod = tClass.GetMethod(MethodName);
                return mMethod.ReturnType.Name;
            }
            catch (Exception e)
            {
                MessageShow.ExceptionOccur("Checking return type of " + keyword + " fail.", e);
                return null;
            }
            
        }

        public static int NumberOfArgument(string Keyword) {
            int n = 0;
            List<string> rs = getArgumentInfo(Keyword);
            if (rs != null)
                n = rs.Count;
            return n;
        }
        /**
         * Return List<TestCommand> generate by looking for *.dll files in AppMapFolder
         * */
        public static List<TestCommand> getCommandInfos(string AppMapFolder) {
            List<TestCommand> rs = new List<TestCommand>();
            string[] filePaths = Directory.GetFiles(@AppMapFolder,"*.dll");
            int n = filePaths.Length;
            for(int i = 0;i<n;i++){
                List<TestCommand> temp = GetAllMethodsDLLs(filePaths[i]);
                rs.AddRange(temp);
            }
            int nMethod = rs.Count;
            for (int i = 0; i < nMethod; i++) {
                rs[i].ID = (i + 1).ToString();
            }
            return rs;
        }
        /**
         * Checking if the folder is exist
         * */
        public static bool FolderExist(string folderPath) {
           return Directory.Exists(@folderPath);
        }
        /**
         * @Commands: List<TestCommand>
         * Return this.Commands
         * */
        public static List<TestCommand> getCommandsInfo() {
            return Commands;
        }
        /**
         * Checking if the keyword is exist
         * */
        public static bool KeywordExist(string keyword) {
            foreach (TestCommand item in Commands) {
                if (item.Keyword.Equals(keyword))
                    return true;
            }
            return false;
        }
        /**
         * Return the TestCommand that contains keyword
         * */
        public static TestCommand getCommandInfo(string keyword) {
            foreach (TestCommand item in Commands)
            {
                if (item.Keyword.Equals(keyword))
                    return item;
            }
            return null;
        }
    


    }
}
