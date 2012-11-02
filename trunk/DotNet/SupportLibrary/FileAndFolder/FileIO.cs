using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace SupportLibrary
{
    public class FileIO
    {
        public bool CreateFolder(string Path){
            try
            {
                if (!System.IO.Directory.Exists(Path))
                {
                    System.IO.Directory.CreateDirectory(Path);
                }
                return true;
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public static string[] getFiles(string Folder) {
            try
            {
                string[] filePaths = Directory.GetFiles(Folder, "*.*");
                return filePaths;
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                return null;
            }
            
        }

        public bool IsExist(string File) {
            try
            {
                return System.IO.File.Exists(File);
            }
            catch (Exception e) { 
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public bool CopyFile(string sourceFile, string desFile) {
            try
            {
                
                // Copy the file. The destination file can be overwritten
                File.Copy(sourceFile, desFile,true);
                Console.WriteLine("{0} copied to {1}", sourceFile, desFile);
                return true;
            }
            catch
            {
                Console.WriteLine("Double copy is not allowed, which was not expected.");
                return false;
            }
        }
        /**
         * @error: the error occur when excute a command.
         * Save in [CurrentDate].txt
         * if [CurrentDate].txt is exist it will create it.
         * else: it will write to the last line of [CurrentDate].txt
         * */
        public static bool MakeErrorLog( string error)
        {
            string CurrentDate = util.getDate();
            string path = CurrentDate + ".txt";//Ngay hien tai
            FileInfo f = new FileInfo(@path);
            StreamWriter w;
            if (f == null)
            {
               w = f.CreateText();
            }
            else {
                w = f.AppendText();
            }
            w.WriteLine(error);
            w.Close();
            return true;
        }

        public static void WriteLog(string path,string title,string body){
            FileInfo f = new FileInfo(@path);
            StreamWriter w;
            if (f == null)
            {
                w = f.CreateText();
            }
            else
            {
                w = f.AppendText();
            }
            w.WriteLine(title);
            w.WriteLine(body);
            w.Close();
        }

        /**
         * @path: path to textfile
         * @lines: data for writing to textfile
         * Write to textfile lines data, clear textfile first.
         * */
        public static bool WriteToText(string path, List<string> lines)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path))
            {
                foreach (string line in lines)
                {
                    file.WriteLine(line);
                }
                file.Close();
            }
            return true;
        }

        public static List<string> ReadFile(string Path)
        {
            List<string> result = new List<string>();
            string[] lines = File.ReadAllLines(@Path);

            // Display the file contents by using a foreach loop.

            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                result.Add(line);
            }
            return result;
        }
    }
}
