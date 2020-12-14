using System;
using System.Collections.Generic;
using Anthem.Controller;
namespace Anthem
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Log.WriteLog(Log.Status.Warn, "Digite sair a qualquer momento para sair.");
            goto label1;
            while (true) {
                Console.WriteLine("Digite o nome do arquivo a ser convertido: ");
                var line = Console.ReadLine();

                if (line.Equals("sair", StringComparison.OrdinalIgnoreCase))
                    break;

                var fileController = new FileController(line);

                if(!fileController.CheckFile()){
                    Console.WriteLine("Arquivo não existe!");
                    continue;
                }
                    

                var fileData = fileController.ReadLines();

                var conflict = MipsConflictController.MipsConflictLines_RAW(fileData);
                Log.WriteLog(Log.Status.Warn, "Realizando procedimento de bolha.");
                var result = MipsConflictController.MipsResolveBolha(fileData, conflict);

                Log.WriteLog(Log.Status.Warn, $"Arquivo salvo como: {fileController.GetFilePathNotExt() + "_bolha.txt"}");
                fileController.WriteLines("_bolha.txt", result);

                Log.WriteLog(Log.Status.Warn, "Realizando procedimento de bolha/adiantamento.");
                result = MipsConflictController.MipsResolveBolhaAdiantamento(fileData, conflict);

                 Log.WriteLog(Log.Status.Warn, $"Arquivo salvo como: {fileController.GetFilePathNotExt() + "_bolhaadiantamento.txt"}");
                fileController.WriteLines("_bolhaadiantamento.txt", result);

                Log.WriteLog(Log.Status.Warn, "Realizando procedimento de bolha/adiantamento.");
                result = MipsConflictController.MipsResolveReordenacaoNovo(result);

                 Log.WriteLog(Log.Status.Warn, $"Arquivo salvo como: {fileController.GetFilePathNotExt() + "_reordenacao.txt"}");
                fileController.WriteLines("_reordenacao.txt", result);

                Environment.Exit(0);
            }
            Environment.Exit(0);
            label1:
            List<string[]> testes = new List<string[]>() {

                new string[]{ //Teste0
                    "lw $t0, 1200($t1)     ",
                    "add $t0, $s2, $t0     ",
                    "sw $t0, 1200($t1)     ",
                    "lw     $t0, 4($t7)    ",
                    "mult   $t0, $t0, $t0  ",
                    "lw     $t1, 4($t7)    ",
                    "ori    $t2, $zero, 3  ",
                    "mult   $t1, $t1, $t2  ",
                    "add    $t2, $t0, $t1  ",
                    "sw     $t2, 0($t7)    ",
                    "lw     $t0, 0($t7)    ",
                    "srl    $t0, $t0, 1    ",
                    "addi   $t1, $t7, 28   ",
                    "sll    $t0, $t0, 2    ",
                    "add    $t1, $t1, $t0  ",
                    "lw     $t1, 0($t1)    ",
                    "addi   $t1, $t1, 1    ",
                    "lw     $t0, 0($t7)    ",
                    "sll    $t0, $t0, 2    ",
                    "addi   $t2, $t7, 28   ",
                    "add    $t2, $t2, $t0  ",
                    "sw     $t1, 0($t2)    ",
                    "lw     $t0, 0($t7)    ",
                    "addi   $t0, $t0, 1    ",
                    "sll    $t0, $t0, 2    ",
                    "addi   $t1, $t7, 28   ",
                    "add    $t1, $t1, $t0  ",
                    "addi   $t2, $zero, -1 ",
                    "sw     $t2, 0($t1)    "
                },
                new string[] { //Teste1
                    "lw $t0, 1200($t1)",
                    "add $t0, $s2, $t0",
                    "sw $t0, 1200($t1)"
                },
                new string[] { //Teste2
                    "add $s5, $s0, $s0",
                    "addi $s2, $s0, 400",
                    "lw $s1, 1000($s2)",
                    "add $s5, $s5, $s1",
                    "addi $s2, $s2, 4",
                    "blez $s2, Soma"
                },
                new string [] { //Teste3
                    "add $s5, $s6, $s7",
                    "lw $s6, 1200($s7)",
                    "sub $s7, $s6, $s0",
                    "addi $s7, $s7, 1",
                    "sw $s6, 100($s7)"
                },
                new string[] { //Teste4
                    "lw $t0, 1200($t1)",
                    "beq $t1, $s2, 200",
                    "sw $t0, 1500($t1)"
                }

            };

            //BOLHA - OK
            //BOLHA + ADIANTAMENTO - OK
            //REORDENAÇÃO - Falha.

            for(int i =0; i < testes.Count; i ++){
                Console.WriteLine($"------------ TESTE {i + 1} ------------");
                var conflict = MipsConflictController.MipsConflictLines_RAW(testes[i]);

                var result = MipsConflictController.MipsResolveBolha(testes[i], conflict);
                Console.WriteLine("Bolha:");
                foreach(var line in result){
                    Console.WriteLine(line);
                }

                result = MipsConflictController.MipsResolveBolhaAdiantamento(testes[i], conflict);
                Console.WriteLine("----");
                Console.WriteLine("---");
                Console.WriteLine("Bolha com Adiantamento");
                foreach(var line in result){
                    Console.WriteLine(line);
                }
                Console.WriteLine("----");
                Console.WriteLine("---");
                Console.WriteLine("Usando adiantamento para reordenação");

                result = MipsConflictController.MipsResolveReordenacaoNovo(result);
                foreach(var line in result){
                    Console.WriteLine(line);
                }

            }

            
                
        }
    }
}
