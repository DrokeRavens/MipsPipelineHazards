using System;
using System.Collections.Generic;
using System.Linq;

namespace Anthem.Controller
{
    public static class MipsConflictController
    {
        private static readonly string[] REGNAMES =
        {
            "$zero", "$at", "$v0", "$v1", "$a0", "$a1", "$a2", "$a3", "$t0", "$t1", "$t2", "$t3", "$t4", "$t5", "$t6", "$t7", "$s0", "$s1", "$s2", "$s3", "$s4", "$s5", "$s6", "$s7", "$t8", "$t9", "$k0", "$k1", "$gp", "$sp", "$fp", "$ra"
        };
        //Instruçoes que definitamente não fazem escrita
        private static readonly string[] LEITURACERTAIN = new string[] {
            "JR",
            "JALR",
            "BLTZ",
            "BGEZ",
            "BLTZAL",
            "BGEZAL",
            "J",
            "JAL",
            "BEQ",
            "BNE",
            "BLEZ",
            "BGTZ",


            "SB",
            "SH",
            "SWL",
            "SW",
            "SWR",
        };

        private static readonly string[] DESVIOS = new string[]{
            "JR",
            "JALR",
            "BLTZ",
            "BGEZ",
            "BLTZAL",
            "BGEZAL",
            "BEQ",
            "BNE",
            "BLEZ",
            "BGTZ",
        };
        private static readonly char[] DELIMITERS = new char[] { ' ', ',' };

        //Logica: Se a linha atual possui conflito com a proxima linha, do tipo RAW, insere-se dois NOP.
        public static string[] MipsResolveBolha(string[] mipsInput, List<ConflictData> conflicts, int nopAmount = 2)
        {
            List<string> resolved = new List<string>();
            for(int i =0 ; i < mipsInput.Length; i++){
                resolved.Add(mipsInput[i]);
                if(conflicts.Any(x => x.conflictLine == i)){
                    for(int j = 0; j < nopAmount; j++)
                        resolved.Add("NOP");
                }
                    
            }
            return resolved.ToArray();
        }

        //Logica: Se a linha atual tem o valor disponivel no estágio 2, ocorre o adiantamento e não precisa de bolha
        //Se a linha atual tem o valor disponivel somente no estágio 3, realiz a bolha e ocorre adiantamento.
        // Ainda é preciso pensar em outras situações que possam ocorrer, mas imagino que somente lw é suficiente?
        public static string[] MipsResolveBolhaAdiantamento(string[] mipsInput, List<ConflictData> conflicts)
        {
            List<string> resolved = new List<string>();
            for(int i =0 ; i < mipsInput.Length; i++){
                resolved.Add(mipsInput[i]);
                if(conflicts.Any(x => x.conflictLine == i))
                    if(!AdiantamentoIsPossible(mipsInput[i]))
                        resolved.Add("NOP");
            }
            return resolved.ToArray();
        }
        public static string[] MipsResolveReordenacaoNovo(string[] mipsInput){
            var toRemove = new List<int>();
            var resolved = mipsInput.ToList();
            for(int i = 0; i < mipsInput.Length; i++){
                //Primeiro, vamos procurar uma linha com NOP
                if(mipsInput[i].Equals("NOP", StringComparison.InvariantCultureIgnoreCase)){
                    //Beleza, temos um NOP aqui.
                    //Vou tentar usando a logica:
                    /*
                        Se existe um NOP aqui, significa que a linha anterior (i - 1), possui conflito com a linha posterior (i + 1)
                        Vamos então selecionar a linha anterior, e ignorar a posterior.
                        Verificamos então, se existe alguma linha, que não seja i + 1 e nem i, e que não tem conflito com i - 1,
                        Caso encontrado, verificamos a dependencia da linha Classificada
                            O que define uma dependencia da linha: 
                                -  Linha classificada escreve? 
                                        Sim: 
                                            Verifica se as linhas anteriores a ela até o NOP, não dependem do valor que ela escreve.
                                            Verifica se os valores que ela lê, não são escritos entre o NOP e ela.
                                        Não: 
                                            Verifica se os valores que ela lê, não são escritos entre o NOP e ela.
                                Se as condições forem atendidas, realizamos então a troca do NOP com a linhaClassificada, e então adicionamos o index do nop para
                                a fila de remoção
                    */

                    var linhaAnterior = i - 1; //Não vai dar crash, considerando que nenhuma array começará com NOP.
                    var linhaPosterior = i + 1;

                    for(int j = 0; j < mipsInput.Length; j++){
                        if(j != i && j != linhaPosterior){ //A linha a ser procurada Não pode ser i + 1 e nem i 
                            var linhaClassificada = mipsInput[j];
                            //Vamos controlar se a linha classificavel é de fato elegivel
                            bool elegivel = false;
                            if(MipsIsWrite(linhaClassificada))
                            {
                                var escrita = Helper.RemoveDecimals(Helper.RemoveExtraSpaces(linhaClassificada)).Split(DELIMITERS).ToList()[1]; 
                                    var leitura = Helper.RemoveDecimals(Helper.RemoveExtraSpaces(linhaClassificada)).Split(DELIMITERS).ToList();
                                        leitura.RemoveAt(0); //Remove a instrução
                                        leitura.RemoveAt(0); //Remove a escrita
                                    //Verificando todos os valores anteriores a linha Selecionada até o NOP
                                    var kValue = j > i ?  i : j;
                                    var toOffset = j > i ? j : i;
                                    for(int k = kValue; k < toOffset; k++){
                                        var linhaAtual = mipsInput[k];
                                        if(linhaAtual == "NOP")
                                            continue;
                                        if(k == j)
                                            continue;
                                        var linhaAtualSplit = Helper.RemoveDecimals(Helper.RemoveExtraSpaces(linhaAtual)).Split(DELIMITERS).ToList();
                                        var linhaAtualSplitEscrita = Helper.RemoveDecimals(Helper.RemoveExtraSpaces(linhaAtual)).Split(DELIMITERS).ToList();
                                        //Só me importa as leituras.
                                        if(MipsIsWrite(linhaAtual)){
                                            linhaAtualSplit.RemoveAt(1); //Remove o valor que ela está escrevendo
                                        }
                                        //Verifica se a linha atual depende da escrita da linha selecionada
                                        if(!linhaAtualSplit.Contains(escrita)){
                                            //Não é elegivel, pois a linha atual depende da escrita da linha classificada.
                                            elegivel = true;
                                        }
                                        else
                                            elegivel = false;
                                        if(!leitura.Contains(linhaAtualSplitEscrita[1])){
                                            //Nao é elegivel, pois a linha classificada, lê algo que a linha atual escreve.
                                            elegivel = true;
                                        }
                                        else
                                            elegivel = false;
                                    }
                            }
                            else{
                                var leitura = Helper.RemoveDecimals(Helper.RemoveExtraSpaces(linhaClassificada)).Split(DELIMITERS).ToList();
                                leitura.RemoveAt(0);
                                var kValue = j > i ?  i : j;
                                var toOffset = j > i ? j : i;
                                for(int k = kValue; k < toOffset; k++)
                                {
                                    var linhaAtual = mipsInput[k];
                                    if(linhaAtual == "NOP")
                                        continue;
                                    var linhaAtualSplit = Helper.RemoveDecimals(Helper.RemoveExtraSpaces(linhaAtual)).Split(DELIMITERS).ToList();
                                    if(MipsIsWrite(linhaAtual)){
                                        var escritaLinhaAtual = linhaAtualSplit[1];

                                        if(!leitura.Contains(escritaLinhaAtual))
                                            elegivel = true;
                                    }
                                }
                            }

                            if(elegivel){
                                
                                var tmp = resolved[i];
                                resolved[i] = resolved[j];
                                resolved[j] = tmp;
                                toRemove.Add(j);
                                break; //Break pois resolvemos o NOP atual.
                            }
                        }
                    }
                }
            }
            foreach(var removeIndex in toRemove){
                resolved.RemoveAt(removeIndex);
            }
            return resolved.ToArray();
        }
        public static string[] MipsResolveReordenacao(string[] mipsInput)
        {
            //Isso aqui vai fritar meu cerebro ctz kkkk, vamo lá.
            List<string> resolved = mipsInput.ToList();
            List<int> toRemove = new List<int>();
            for(int i =0 ; i < mipsInput.Length; i++){
                if(mipsInput[i].ToUpper() == "NOP"){ //Vamos tentar substituir esse carinha
                    //SOCORR
                    //Espero que o arquivo do professor nao seja grande, porque isso aqui tem 3 for nestado
                    var linhaNOP = mipsInput[i];
                    for(int j = 0; j < mipsInput.Length; j++){
                        bool swapAllowed = false;
                        bool nopResolved = false;
                        if(j != i && j != i - 1){
                            if(nopResolved)
                                break;
                            var linhaJSplit = Helper.RemoveDecimals(Helper.RemoveExtraSpaces(mipsInput[j])).Split(DELIMITERS).ToList();
                            linhaJSplit.RemoveAt(0); //Tira o tipo da instrução e só deixa as variaveis, Ex: lw $s1, 1200($zero) passa a ser $s1, 1200($zero)
                            
                            if(mipsInput[j] != "NOP"){
                                if(MipsIsWrite(mipsInput[j])){
                                    linhaJSplit = new List<string> { linhaJSplit[0]};   
                                } //Só me importa a escrita
                                    
                                for(int k = 0; k < mipsInput.Length; k++)
                                {
                                    if(k!=i && k!=j){
                                        var linhaKSplit = Helper.RemoveDecimals(Helper.RemoveExtraSpaces(mipsInput[k])).Split(DELIMITERS).ToList();
                                        linhaKSplit.RemoveAt(0);
                                        if(MipsIsWrite(mipsInput[k])){
                                            linhaKSplit = new List<string> { linhaKSplit[0]};   
                                        }
                                        if(!linhaJSplit.Any(x => linhaKSplit.Contains(x))){ //Se a linha J possuir alguma variavel da linha K que estamos verificando, não podemos reordenar, e podemos caso contrario
                                            swapAllowed = true;
                                        }
                                    }
                                    if((k == mipsInput.Length -1) && swapAllowed){
                                        var tmp = resolved[j];
                                        resolved[j] = resolved[k];
                                        resolved[k] = tmp;
                                        toRemove.Add(i);
                                        nopResolved = true;
                                    }
                                }
                            }
                        }
                            
                    }
                }
            }
            foreach(var removeOffset in toRemove){
                resolved.RemoveAt(removeOffset);
            }
            return resolved.ToArray();
        }

        private static bool MipsIsWrite(string instruction)
        {
            var instType = GetTypeFromString(instruction);
            if ((byte)instType == 2) // J
                return false;
            if ((byte)instType == 1)
            {
                foreach (var str in LEITURACERTAIN)
                    if (instruction.ToUpper().Contains(str))
                        return false;
            }

            return true;
        }

        private static bool AdiantamentoIsPossible(string instruction){
            //Instruçoes aritméticas possuem o resultado disponivel no final do estágio 3
            //Usando isso como logica, não será preciso inserir nop na proxima leitura, pois ocorrerá adiantamento
            //Levando em conta que a proxima instrução irá precisar da informação no estágio 2
            //LW estará disponivel apenas no estágio 3, e não no estágio 2 como em op aritméticas(final de execução)

            var instType = GetTypeFromString(instruction);
            var stage3Avaiable = new string[] { //Essas instruções só tem o valor disponivel no estágio 3, então precisará de bolha
                "LW",
                "LH",
                "LWL",
                "LW",
                "LBU",
                "LHU",
                "LWR",

            };
            //Checando se a instrução pertence a stage3Avaiable
            foreach(string str in stage3Avaiable)
                if(instruction.ToUpper().Contains(str))
                    return false; //Então precisa de bolha

            return true;

        }
        public static List<ConflictData> MipsConflictLines_RAW(string[] mipsInput)
        {
            List<ConflictData> conflictDatas = new List<ConflictData>();
            for (int i = 0; i < mipsInput.Length; i++)
            {
                if (MipsIsWrite(mipsInput[i]))
                {
                    string[] splitedCommand = Helper.RemoveExtraSpaces(mipsInput[i]).Split(DELIMITERS);
                    ConflictData conflict = new ConflictData(i);
                    bool conflicted = false;
                    if (i + 1 < mipsInput.Length)
                    {
                        List<string> splitedCommandNext =  Helper.RemoveDecimals(Helper.RemoveExtraSpaces(mipsInput[i + 1])).Split(DELIMITERS).ToList();
                        
                        splitedCommandNext.RemoveAt(0);

                        if(MipsIsWrite(mipsInput[i + 1])){
                            if(!mipsInput[i+1].Contains("("))
                                splitedCommandNext.RemoveAt(0);
                        }

                        if (splitedCommandNext.Any(x => x.ToUpper().Contains(splitedCommand[1].ToUpper())))
                        { //Read After Write - Se a proxima linha conter leitura e a linha anterior for escrita = conflito
                            conflict.AddConflictWith(i + 1);
                            conflicted = true;
                        }
                    }
                    if (i + 2 < mipsInput.Length)
                    {
                        List<string> splitedCommandNext = Helper.RemoveDecimals(Helper.RemoveExtraSpaces(mipsInput[i + 2])).Split(DELIMITERS).ToList();

                        splitedCommandNext.RemoveAt(0);

                        if(MipsIsWrite(mipsInput[i + 2])){
                            if(!mipsInput[i+2].Contains("("))
                                splitedCommandNext.RemoveAt(0);
                        }

                        if (splitedCommandNext.Any(x => x.ToUpper().Contains(splitedCommand[1].ToUpper())))
                        { //Read After Write - Se a proxima linha conter leitura e a linha anterior for escrita = conflito
                            conflict.AddConflictWith(i + 2);
                            conflicted = true;
                        }
                    }
                    if(conflicted)
                        conflictDatas.Add(conflict);

                }
            }

            return conflictDatas;
        }

        public static InstructionType GetTypeFromString(string mipsCommand)
        {
            mipsCommand = Helper.RemoveExtraSpaces(mipsCommand);
            string[] splitedCommand = mipsCommand.Split(DELIMITERS);
            InstructionType type;
            if (!InstructionType.TryParse(splitedCommand[0], true, out type))
            {
                throw new Exception("Unknown instruction!");
            }

            return type;
        }
    }
}