using System;

namespace Anthem
{
    public class Log
    {
        /// <summary>
        /// Status possíveis
        /// </summary>
        public enum Status { 
            Success = ConsoleColor.Green,
            Fail = ConsoleColor.Red,
            Warn = ConsoleColor.Yellow,

        }
        /// <summary>
        /// Faz um log bonitinho, com cor e tudo. Incrivel né? Acho que nem tanto... :/
        /// </summary>
        /// <param name="status">O codigo de status da mensagem de log.</param>
        /// <param name="message">A mensagem a ser escrita</param>
        public static void WriteLog(Status status, string message) {
            Console.ForegroundColor = (ConsoleColor)status;
            switch (status) {
                case Status.Success:
                    Console.Write("[Success] -> ");
                    break;
                case Status.Fail:
                    Console.Write("[Falha] -> ");
                    break;
                case Status.Warn:
                    Console.Write("[Aviso] -> ");
                    break;
            }
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
        }
    }
}
