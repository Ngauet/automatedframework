using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Driver;
using SupportLibrary;




namespace AutomationTest
{
    class Program
    {
       

        static List<TestCase> listTCase = new List<TestCase>();
        static List<TestData> listTData = new List<TestData>();
        static List<TestCommand> listTCommand = new List<TestCommand>();
        static List<TestDataset> listTDataset = new List<TestDataset>();
        static List<TestStep> listTStep = new List<TestStep>();
        //"D:\AutomationTest\APPLICATIONMAP\"
        //static string ExcelPath;
        static ExcelDataAccess dtAccess = null;

       
        //Features of Automation:
        static void CmdExcuteKeyword() {
            CmdExcuteKeyword feature3 = new CmdExcuteKeyword(dtAccess);
            feature3.Main("Cmd Excute Keyword");
            dtAccess.Close();
        }

        static void GenerateCommands() {
            bool isSuccess = false;
            do
            {
                string inDll = "";
                Console.Write("Path to dll file:");
                inDll = Console.ReadLine();
                if (inDll.Equals("/e"))
                    break;
                GenerateTCommandWithFile feature5 = new GenerateTCommandWithFile(inDll, dtAccess);
                isSuccess = feature5.Main("Generate TCommand with file");
            } while (!isSuccess);
            dtAccess.Close();
        }

        static void CmdGenerateDatas() {
            CmdGenerateTData feature4 = new CmdGenerateTData(dtAccess);
            feature4.Main("Cmd Generate TestData");
            dtAccess.Close();            
        }

        static void GenerateDatas() {
            GenerateTData feature2 = new GenerateTData(dtAccess);
            feature2.Main("Generate TestData from excel");
            dtAccess.Close();
        }

        static void DebugTestCases() {
            DebugTCase feature1 = new DebugTCase(dtAccess);
            feature1.Main("DebugTestCases");
            dtAccess.Close();
        }        
        
        static void BuildRelease() {
            BuildRelease feature6 = new BuildRelease(dtAccess, "Script\\");
            feature6.Main("Build Release");
            dtAccess.Close();
        }

        static void DebugTestCasesMultithread() {
            //DebugTCase feature7 = new DebugTCase(dtAccess);
            DebugMultithread feature8 = new DebugMultithread(dtAccess);

            //feature7.Main("Debug in multithread mode");
            feature8.Main("Debug in multithread mode");
            dtAccess.Close();
        }


        static void Refresh() {
            dtAccess.RefreshTDatas();
            dtAccess.Close();
            Console.WriteLine("Refresh successfull.");
        }
        //Features

        static void Main(string[] args)
        {
            
            //List<TestCommand> list = ComponentFunction.GetAllMethodsDLLs(@"d:\AutomationTest\APPLICATIONMAP\AppLibrary.dll");
            
            //string ExcelPath = "test.xls";

            string ExcelPath = ConfigurationSettings.AppSettings["ExcelFile"];

            dtAccess = new ExcelDataAccess(ExcelPath);
            
            //listTCase = dtAccess.getTestcases();
            //listTCommand = dtAccess.getTestcommands();
            //listTDataset = dtAccess.getTestdatasets();
            //listTStep = dtAccess.getTeststeps();
            //listTData = dtAccess.getTestdatas();
            ////ComponentFunction.Initialize(listTCommand);            
            //DebugTestCases();
            //GenerateDatas();
            //GenerateCommands();
            //CmdGenerateDatas();
            //CmdExcuteKeyword();           
            while (true) {
                

                bool flag = true;
                Console.WriteLine("AUTOMATION SOFTWARE V1.");
                Console.WriteLine("1: DebugTestCases.");
                Console.WriteLine("2: Generate TestData from excel.");
                Console.WriteLine("3: Cmd Excute Keyword.");
                Console.WriteLine("4: Cmd Generate TestData.");
                Console.WriteLine("5: Generate Commands.");
                Console.WriteLine("6: Build Release.");
                Console.WriteLine("7: DebugTestCases in multithread mode.");
                Console.WriteLine("8: Refresh.");
                Console.WriteLine("/e: Exit");
                Console.Write("Your choice:");
                
                //string arg = args[0];
                string input = string.Empty;

                if (args.Length > 0)
                {
                    input = args[0];   
                }
                else
                {
                     input = Console.ReadLine();
                }
                Console.Write(input);
                switch (input) { 
                    case "1":
                       
                        DebugTestCases();                       
                        flag = false;
                        break;
                    case "2":
                        GenerateDatas();                        
                        flag = false;                        
                        break;
                    case "3":
                        CmdExcuteKeyword();                       
                        flag = false;     
                        break;
                    case "4":
                        CmdGenerateDatas();                       
                        flag = false;
                        break;
                    case "5":
                        GenerateCommands();                       
                        flag = false;
                        break;
                    case "6":
                        BuildRelease();
                        flag = false;                        
                        break;
                    case "7":
                        DebugTestCasesMultithread();
                        flag = false;
                        break;
                    case "8":
                        Refresh();
                        flag = false;
                        break;
                    case "/e":
                        flag = false;
                        break;
                }
                if (!flag)
                    break;
            }
            
            Console.ReadLine();
        }
    }
}
    
