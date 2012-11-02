using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportLibrary;
namespace AutomationTest
{
    public abstract class ConsoleBase
    {
      
        protected string Name;
        protected System.IO.TextReader In = null;
        protected System.IO.TextWriter Out = null;
        protected System.IO.TextWriter Error = null;
        protected void Initializer(string FeatureName) {
            this.In = System.Console.In;
            this.Out = System.Console.Out;
            this.Error = System.Console.Out;
            Name = FeatureName;
        }
        protected abstract bool Input();
        protected abstract bool Validator();
        protected abstract bool Run();
        public bool Main(string FeatureName)
        {
            MessageShow.StartFeature(FeatureName);
            Initializer(FeatureName);
            if (Input())
            {
                if (Validator())
                {
                    if (Run())
                    {
                        Finish(true);
                        return true;
                    }
                    else { }
                }
                else { }
            }
            else { }
            Finish(false);
            return false;   
        }
        private void Finish(bool result) {
            if (result)
            {
                this.Out.WriteLine(Name + " SUCCESSFUL.");
            }
            else {
                this.Out.WriteLine(Name + " UNSUCCESSFUL.");
            }
        }
    }
}
