using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SupportLibrary;
using System.Configuration;

namespace Driver
{
    public class ReflectMethod
    {
        private MethodInfo _Method = null;
        private Type _tClass = null;
        public string _Keyword {get;set;}
        /**
         * @Object: path to file dll
         * @Target: namespace.class name.
         * @Function: the method name.
         * */
        public ReflectMethod(string Object, string Target, string MethodName,string Keyword) {
            Assembly assembly = Assembly.LoadFrom(@Object);
            _tClass = assembly.GetType(Target);
            _Method = _tClass.GetMethod(MethodName);
            _Keyword = Keyword;
        }

        public bool isVoid() {
            if (_Method.ReturnType.Name.Equals(ValueType.VOID))
                return true;
            return false;
        }

        private string getFunc(MethodInfo meth)
        {
            List<string> listTemp = util.getListValueWithToken(meth.ToString(), '(');
            string func = listTemp[0] + "(";
            ParameterInfo[] pis = meth.GetParameters();
            int n = pis.Length;
            if (n > 0)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    ParameterInfo info = pis[j];
                    func += info.ParameterType.Name + " " + info.Name + ",";
                }
                ParameterInfo lastinfo = pis[n - 1];
                func += lastinfo.ParameterType.Name + " " + lastinfo.Name;
            }
            func += ")";
            return func;
        }

        public string getMethodDecription() {
            return getFunc(_Method);
        }

        public int nParameter() {
            ParameterInfo[] pi = _Method.GetParameters();
            return pi.Length;
        }

        public List<string> getArgumentsType() {
            List<string> rs = new List<string>();
            ParameterInfo[] pi = _Method.GetParameters();
            int n = pi.Length;
            for (int i = 0; i < n; i++)
            {
                string temp = pi[i].ParameterType.Name;
                rs.Add(temp);
            }
            return rs;
        }

        /**
         * @values: input values for this method.
         * @output: output value, null if void method.
         * @mes: error from user's code.
         * */
        public bool Excute(object[] values, ref object output, ref string mes, Dictionary<string, string> listConfig, object _InvokeParam1 = null)
        {
            output = null;
            mes = "";
            try
            {
                if (_InvokeParam1 == null)
                {
                    _InvokeParam1 = Activator.CreateInstance(_tClass, listConfig);
                }

                if (isVoid())
                {
                    _Method.Invoke(_InvokeParam1, values);
                }
                else
                {
                    output = _Method.Invoke(_InvokeParam1, values);
                }            
                return true;
            }
            catch (TargetInvocationException target_e)
            {
                mes = target_e.ToString();
                return false;
            }
        }
    }
}
