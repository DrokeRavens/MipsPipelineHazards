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

        /// <summary>
        /// Pega somente os dados importantes para esse trabalho, então por exemplo, essa linha: lw $s1, 1234($t0)
        /// Será convertida em: lw $s1, $t0. Obviamente isso causaria erro, mas não estamos convertendo, somente procurando conflitos.
        /// </summary>
        /// <param name="cmd">O comando a ser verificado</param>
        /// <returns>O comando corrigido ou o comando original, caso a situação não seja atingida.</returns>
        public static string RemoveDecimals(string cmd){
            var delimiters = new char[] {'(', ')'};
            if(cmd.Contains("(")){
                var splited = cmd.Split(delimiters);
                var parentesisValue = splited[1];
                var splited2 = cmd.Split(',').ToList();
                splited2.RemoveAt(splited2.Count - 1);
                cmd = string.Join(',', splited2);
                cmd += ',' + parentesisValue;
                
            }
            return cmd;
        }
    }
}
