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
            "SW",
            "LB",
            "LH",
            "LWL",
            "LW",
            "LBU",
            "LHU",
            "LWR",
            "SB",
            "SH",
            "SWL",
            "SWR"
        };
        private static readonly char[] DELIMITERS = new char[] { ' ', ',' };
        public static string[] MipsResolveBolha(string[] mipsInput)
        {
            throw new NotImplementedException();
        }
        public static string[] MipsResolveBolhaAdiantamento(string[] mipsInput)
        {
            throw new NotImplementedException();
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
                foreach (var str in LEITURACERTAIN)//Acho que desvio não causará conflito
                    if (instruction.ToUpper().Contains(str))
                        return false;
            }

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
                    if (i + 1 < mipsInput.Length)
                    {
                        List<string> splitedCommandNext = Helper.RemoveExtraSpaces(mipsInput[i + 1]).Split(DELIMITERS).ToList();
                        
                        splitedCommandNext.RemoveAt(0);
                        if(MipsIsWrite(mipsInput[i + 1]))
                            splitedCommandNext.RemoveAt(0);

                        if (splitedCommandNext.Any(x => x.ToUpper().Contains(splitedCommand[1].ToUpper())))
                        { //Read After Write - Se a proxima linha conter leitura e a linha anterior for escrita = conflito
                            var conflict = new ConflictData(i);
                            conflict.AddConflictWith(i + 1);
                            conflictDatas.Add(conflict);
                        }
                    }
                    else if (i + 2 < mipsInput.Length)
                    {
                        List<string> splitedCommandNext = Helper.RemoveExtraSpaces(mipsInput[i + 2]).Split(DELIMITERS).ToList();

                        splitedCommandNext.RemoveAt(0);
                        if(MipsIsWrite(mipsInput[i + 2]))
                            splitedCommandNext.RemoveAt(0);

                        if (splitedCommandNext.Any(x => x.ToUpper().Contains(splitedCommand[1].ToUpper())))
                        { //Read After Write - Se a proxima linha conter leitura e a linha anterior for escrita = conflito
                            var conflict = new ConflictData(i);
                            conflict.AddConflictWith(i + 2);
                            conflictDatas.Add(conflict);
                        }
                    }
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