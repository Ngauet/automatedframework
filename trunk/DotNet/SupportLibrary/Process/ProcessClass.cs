using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;

namespace SupportLibrary
{
    enum ProcessType
    {
        Application, ApplicationWithArguments
    }
    /**
     * Execute a process (.exe file) then take the output of this process.(Commandline only).
     * */
    public class ProcessClass
    {

        private ProcessType _TypeApplication;
        ProcessStartInfo _StartInfo;
        public ProcessClass(string ProcessInfo)
        {
            _StartInfo = new ProcessStartInfo(@ProcessInfo);
            _TypeApplication = ProcessType.Application;

        }
        /**
         * Example: [process] [arguments]
         *          pict.exe input.txt > output.txt
         * */
        public void AddArguments(string args)
        {
            _StartInfo.Arguments = args;
            _TypeApplication = ProcessType.ApplicationWithArguments;
        }

        public string Run()
        {
            string result = null;
            _StartInfo.CreateNoWindow = false;
            _StartInfo.UseShellExecute = false;
            _StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(_StartInfo);
            _StartInfo.RedirectStandardOutput = true;
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process process = Process.Start(_StartInfo))
                {
                    //
                    // Read in all the text from the process with the StreamReader.
                    //
                    using (StreamReader reader = process.StandardOutput)
                    {
                        result = reader.ReadToEnd();

                    }
                }
            }
            catch
            {
                // Log error.
            }
            return result;
        }
    }
}
