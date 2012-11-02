using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace SupportLibrary
{
    public class CCodeGenerator
    {
        CodeNamespace mynamespace;
        CodeTypeDeclaration myclass;
        CodeCompileUnit myassembly;
        public CCodeGenerator() { }

        public void BuildCode(string Path,List<string> stats){
            try
            {
                CreateNameSpace();
                CreateImports();
                CreateClass();
                CreateMember();
                CreateProperty();
                CreateMethod();
                CreateEntryPoint(stats);
                SaveAssembly(Path);
            }
            catch (Exception ex)
            {
                MessageShow.Writelog("CCodeGenerator::", ex.Message);
            }
            
        }
        private void CreateNameSpace()
        {
            mynamespace = new CodeNamespace("mynamespace");
        }

        private void CreateImports()
        {
            mynamespace.Imports.Add(new CodeNamespaceImport("System"))  ;
            mynamespace.Imports.Add(new CodeNamespaceImport("System.Text"));
            mynamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            mynamespace.Imports.Add(new CodeNamespaceImport("System.Linq"));
            mynamespace.Imports.Add(new CodeNamespaceImport("System.Configuration"));
            mynamespace.Imports.Add(new CodeNamespaceImport("Driver"));
            mynamespace.Imports.Add(new CodeNamespaceImport("SupportLibrary"));
        }

        private void CreateClass()
        {
            myclass = new CodeTypeDeclaration("myclass");
            myclass.IsClass = true;
            myclass.Attributes = MemberAttributes.Public;
            mynamespace.Types.Add(myclass);

        }

        private void CreateMember()
        {
            CodeMemberField mymemberfield = new CodeMemberField(typeof(System.String), "strMessage");
            myclass.Members.Add(mymemberfield);
        }

        private void CreateProperty()
        {
            CodeMemberProperty mymemberproperty = new CodeMemberProperty();
            //Name of the property
            mymemberproperty.Name = "Message";
            //Data type of the property
            mymemberproperty.Type = new CodeTypeReference(typeof(System.String));
            //Access modifier of the property
            mymemberproperty.Attributes = MemberAttributes.Public;
            //Add the property to the class
            myclass.Members.Add(mymemberproperty);
            //Add the code-snippets to the property. 
            //If required, we can also add some custom validation code.
            //using the CodeSnippetExpression class.
            //Provide the return <propertyvalue> statement-For getter
            CodeSnippetExpression getsnippet = new CodeSnippetExpression("return strMessage");
            //Assign the new value to the property-For setter
            CodeSnippetExpression setsnippet = new CodeSnippetExpression("strMessage=value");
            //Add the code snippets into the property
            mymemberproperty.GetStatements.Add(getsnippet);
            mymemberproperty.SetStatements.Add(setsnippet);
        }

        private void CreateMethod()
        {
            ////Create an object of the CodeMemberMethod
            //CodeMemberMethod mymethod = new CodeMemberMethod();
            ////Assign a name for the method.
            //mymethod.Name = "AddNumbers";
            ////create two parameters
            //CodeParameterDeclarationExpression cpd1 = new CodeParameterDeclarationExpression(typeof(int), "a");
            //CodeParameterDeclarationExpression cpd2 = new CodeParameterDeclarationExpression(typeof(int), "b");
            ////Add the parameters to the method.
            //mymethod.Parameters.AddRange(new CodeParameterDeclarationExpression[] { cpd1, cpd2 });
            ////Provide the return type for the method.
            //CodeTypeReference ctr = new CodeTypeReference(typeof(System.Int32));
            ////Assign the return type to the method.
            //mymethod.ReturnType = ctr;
            ////Provide definition to the method (returns the sum of two //numbers)
            //CodeSnippetExpression snippet1 = new CodeSnippetExpression("System.Console.WriteLine(\" Adding :\" + a + \" And \" + b )");
            ////return the value
            //CodeSnippetExpression snippet2 = new CodeSnippetExpression("return a+b");
            ////Convert the snippets into Expression statements.
            //CodeExpressionStatement stmt1 = new CodeExpressionStatement(snippet1);
            //CodeExpressionStatement stmt2 = new CodeExpressionStatement(snippet2);
            ////Add the expression statements to the method.
            //mymethod.Statements.Add(stmt1);
            //mymethod.Statements.Add(stmt2);
            ////Provide the access modifier for the method.
            //mymethod.Attributes = MemberAttributes.Public;
            ////Finally add the method to the class.
            //myclass.Members.Add(mymethod);
        }
        //Create main method
        private void CreateEntryPoint(List<string> statements)
        {
            //Create an object and assign the name as "Main"
            CodeEntryPointMethod mymain = new CodeEntryPointMethod();
            mymain.Name = "Main";
            //Mark the access modifier for the main method as Public and //static
            mymain.Attributes = MemberAttributes.Public | MemberAttributes.Static;            
            //Change string to statements
            if(statements != null){
               
                foreach (string item in statements) {
                    if (item != null) {                       
                        CodeSnippetExpression exp1 = new CodeSnippetExpression(@item);
                        CodeExpressionStatement ces1 = new CodeExpressionStatement(exp1);
                        mymain.Statements.Add(ces1);
                    }
                }
            }            
            myclass.Members.Add(mymain);
        }

        private void SaveAssembly(string Path)
        {
            //Create a new object of the global CodeCompileUnit class.
            myassembly = new CodeCompileUnit();
            //Add the namespace to the assembly.
            myassembly.Namespaces.Add(mynamespace);
            //Add the following compiler parameters. (The references to the //standard .net dll(s) and framework library).
            CompilerParameters comparam = new CompilerParameters(new string[] { "mscorlib.dll" });
            comparam.ReferencedAssemblies.Add("System.dll");
            comparam.ReferencedAssemblies.Add("System.Core.dll");
            comparam.ReferencedAssemblies.Add("System.Data.dll");
            comparam.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");
            comparam.ReferencedAssemblies.Add("System.Xml.dll");
            comparam.ReferencedAssemblies.Add("System.Xml.Linq.dll");
            comparam.ReferencedAssemblies.Add("System.Core.dll");
            //comparam.ReferencedAssemblies.Add("System.Collections.Generic.dll");
            comparam.ReferencedAssemblies.Add("Driver.dll");
            comparam.ReferencedAssemblies.Add("SupportLibrary.dll");
            comparam.ReferencedAssemblies.Add("log4net.dll");
            //Indicates Whether the compiler has to generate the output in //memory
            comparam.GenerateInMemory = false;
            //Indicates whether the output is an executable.
            comparam.GenerateExecutable = true;
            //provide the name of the class which contains the Main Entry //point method
            comparam.MainClass = "mynamespace.myclass";
            //provide the path where the generated assembly would be placed 
            comparam.OutputAssembly = @Path;
            //Create an instance of the c# compiler and pass the assembly to //compile
            Microsoft.CSharp.CSharpCodeProvider ccp = new Microsoft.CSharp.CSharpCodeProvider();
            
            //Build to cs file

            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sourceWriter = new StreamWriter( @"release.cs"))
            {
                ccp.GenerateCodeFromCompileUnit(
                myassembly, sourceWriter, options);
            }

            ICodeCompiler icc = ccp.CreateCompiler();
            ////The CompileAssemblyFromDom would either return the list of 
            ////compile time errors (if any), or would create the 
            ////assembly in the respective path in case of successful //compilation
            CompilerResults compres = icc.CompileAssemblyFromDom(comparam, myassembly);
            if (compres == null || compres.Errors.Count > 0)
            {
                for (int i = 0; i < compres.Errors.Count; i++)
                {
                    //Console.WriteLine(compres.Errors[i]);
                    MessageShow.Writelog("CCodeGenerator.cs::", compres.Errors[i].ToString());
                }
            }
        }


    }
}

