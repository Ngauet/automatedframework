using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Driver;
using SupportLibrary;
namespace AutomationTest
{
    class CmdExcuteKeyword:ConsoleBase
    {
        private List<TestCommand> listTCommand = new List<TestCommand>();
        ExcelDataAccess _dtAccess;
        public CmdExcuteKeyword(ExcelDataAccess dtAccess)
        {
            _dtAccess = dtAccess;
            listTCommand = dtAccess.getTestcommands();
            ComponentFunction.Initialize(listTCommand);
        }
        protected override bool Input()
        {
            return true;
        }
        protected override bool Validator()
        {
            if (_dtAccess == null)
            {
                Error.WriteLine("The ExcelDataAccess object is null.");
                return false;
            }
            if (listTCommand.Count <= 0) {
                Error.WriteLine("There are no TCommand available.");
                return false;
            }
            return true; 
        }

        protected override bool Run()
        {
            string input;

            List<TestCommand> commands = ComponentFunction.getCommandsInfo();
            do
            {
                do
                {
                    Out.WriteLine("///////////////////////////////////////////////////////////////////////////////////////////////");
                    Out.WriteLine("/////////////////////////////////////KEYOWORD - MAPPING////////////////////////////////////////");
                    Out.WriteLine("///////////////////////////////////////////////////////////////////////////////////////////////");
                    Out.WriteLine("Keyword         :Function                   ");
                    foreach (TestCommand item in commands)
                    {
                        Console.WriteLine(item.Keyword + ":" + item.Function);
                        Console.WriteLine();
                    }
                    Out.WriteLine("///////////////////////////////////////////////////////////////////////////////////////////////");
                    Out.WriteLine("/////////////////////////////////////KEYOWORD - MAPPING////////////////////////////////////////");
                    Out.WriteLine("///////////////////////////////////////////////////////////////////////////////////////////////");
                    Out.WriteLine("Please type the keyword that you want to test.");
                    Out.WriteLine("/e: for exit to menu.");
                    Out.Write("Keyword: ");
                    input = In.ReadLine();
                    if (input.Equals("/e"))
                        break;
                } while (!ComponentFunction.KeywordExist(input));
                TestCommand command = ComponentFunction.getCommandInfo(input);
                while (!input.Equals("/b"))
                {
                    if (input.Equals("/e"))
                        break;
                    Out.WriteLine("-----------------------------------------------------------");
                    Out.WriteLine("Keyword =" + command.Keyword);
                    Out.WriteLine(command.Function);
                    Out.WriteLine("Please type the input values with syntax: [param1,param2,param3,..,paramn].");
                    Out.WriteLine("/b: for choose another keyword");
                    Out.WriteLine("/e: for exit to menu.");
                    Out.Write("Value: ");
                    input = In.ReadLine();
                    if (input.Equals("/b") || input.Equals("/e"))
                        break;
                    bool flag;
                    object output = null;
                    string error_mes = null;
                    if (ComponentFunction.isVoid(command.Keyword))
                    {
                        flag = ComponentFunction.ExcuteFunction(command.Keyword, input, ref error_mes, ref output);
                    }
                    else
                    {
                        flag = ComponentFunction.ExcuteFunction(command.Keyword, input, ref error_mes, ref output);
                        if (output != null)
                            Console.WriteLine("RETURN VALUE =" + output.ToString());
                    }
                    if (flag)
                    {
                        Out.WriteLine("SUCCESSFUL!");
                    }
                    else
                    {
                        if (error_mes != null)
                            Out.WriteLine(error_mes);
                        Out.WriteLine("UNSUCCESSFUL!");
                    }
                }
            } while (!input.Equals("/e"));
            return true;
        }
    }
}
