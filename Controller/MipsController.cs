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
        };
        private static readonly char[] DELIMITERS = new char[] { ' ', ',' };

        //Logica: Se a linha atual possui conflito com a proxima linha, do tipo RAW, insere-se dois NOP.
        public static string[] MipsResolveBolha(string[] mipsInput, List<ConflictData> conflicts, int nopAmount = 2)
        {
            List<string> resolved = new List<string>();
            for(int i =0 ; i < mipsInput.Length; i++){
                resolved.Add(mipsInput[i]);
                if(conflicts.Any(x => x.conflictLine == i))
                    for(int j = 0; j < nopAmount; j++)
                        resolved.Add("NOP");
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
        public static string[] MipsResolveReordenacao(string[] mipsInput)
        {
            throw new NotImplementedException();
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
        public static List<ConflictData> MipsConflictLines(string[] mipsInput)
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