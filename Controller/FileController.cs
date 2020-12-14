using System.IO;
using System;
namespace Anthem.Controller
{
    public class FileController
    {
        private string filePath = "";
        public FileController(string filePath){
            if(filePath.Contains(":") && filePath.Contains(@"\"))
                this.filePath = filePath;
            else
                this.filePath = Environment.CurrentDirectory + @"\"+ filePath ;

        }

    public bool CheckFile()
        => File.Exists(filePath);            
        

        public string[] ReadLines(){
            if(File.Exists(filePath))
                return File.ReadAllLines(filePath); 
            else
                return new string[0];
        }

        public string ReadLine(int lineOffset){
            if(File.Exists(filePath))
                return  File.ReadAllLines(filePath)[lineOffset];
            else
                return "";
        }
        
        public void WriteLines(string newFileAddName, string[] lines) 
            => File.WriteAllLines(filePath.Split(".txt")[0] + newFileAddName, lines);

        public string GetFilePathNotExt() => filePath.Split(".txt")[0];
    }
}
