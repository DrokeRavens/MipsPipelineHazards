using System;
using System.Collections.Generic;
using Anthem.Controller;
namespace Anthem
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string[]> testes = new List<string[]>() {
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
                var conflict = MipsConflictController.MipsConflictLines(testes[i]);

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
