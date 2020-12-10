using System.Linq;

namespace Anthem
{
    /// <summary>
    /// Classe responsável por conter metodos de auxilio a gerenciamento de strings.
    /// </summary>
    class Helper
    {
        /// <summary>
        /// Corrige um codigo mal formatado, com excesso de espaços desnecessários. Ex: lw $s1,              123($zero)
        /// </summary>
        /// <param name="cmd">O comando a ser corrigido</param>
        /// <returns>O comando corrigido</returns>
        public static string RemoveExtraSpaces(string cmd) {
            var count = cmd.ToCharArray().Count(x => x == ' ');
            if (count > 1)
            {
                var firstCmd = cmd.Split(' ')[0];
                cmd = cmd.Replace(" ", "").Replace(firstCmd, firstCmd + " ");
                return cmd;
            }
            else
                return cmd;
        }
    }
}
