using System;
using Anthem.Controller;
namespace Anthem
{
    class Program
    {
        static void Main(string[] args)
        {
            var simulateConflict = new string[] { //Bolha - Pass
                "lw $t0, 1200($t1)",
                "add $t0, $s2, $t0",
                "sw $t0, 1200($t1)"
            };
            simulateConflict = new string[] { //Bolha - Pass
                "add $s5, $s0, $s0",
                "addi $s2, $s0, 400",
                "lw $s1, 1000($s2)",
                "add $s5, $s5, $s1",
                "addi $s2, $s2, 4",
                "blez $s2, Soma"
            };

            simulateConflict = new string [] { //Bolha adiantamento - Pass
                "add $s5, $s6, $s7",
                "lw $s6, 1200($s7)",
                "sub $s7, $s6, $s0",
                "addi $s7, $s7, 1",
                "sw $s6, 100($s7)"
            };
            var conflict = MipsConflictController.MipsConflictLines(simulateConflict);
            //var result = MipsConflictController.MipsResolveBolha(simulateConflict, conflict);
            var result = MipsConflictController.MipsResolveBolhaAdiantamento(simulateConflict, conflict);

            foreach(var line in result)
                Console.WriteLine(line);
        }
    }
}
