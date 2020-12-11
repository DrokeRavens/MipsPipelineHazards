using System;
using Anthem.Controller;
namespace Anthem
{
    class Program
    {
        static void Main(string[] args)
        {
            var simulateConflict = new string[] {
                "lw $t0, 1200($t1)",
                "add $t0, $s2, $t0",
                "sw $t0, 1200($t1)"
            };
            var conflict = MipsConflictController.MipsConflictLines(simulateConflict);
            var result = MipsConflictController.MipsResolveBolha(simulateConflict, conflict);

            foreach(var line in result)
                Console.WriteLine(line);
        }
    }
}
