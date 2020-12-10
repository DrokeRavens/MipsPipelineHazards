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
            int t = 0;
        }
    }
}
