using System.IO;
using System.Threading.Tasks;

namespace Anthem.Controller{
    public class FileController
    {
    
    private readonly string filePath = "";
    public FileController(string filePath) =>
        this.filePath = filePath;
    

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
    
    public void WriteLines(string newFileAddName, string[] lines) => 
        File.AppendAllLines(filePath + newFileAddName, lines);
    

    public void AppendLine(string newFileAddName, string line) => 
        File.AppendText(line);

    }
}
