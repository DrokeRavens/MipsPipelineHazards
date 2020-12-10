using System.Collections.Generic;

namespace Anthem{
    public class ConflictData {
        public int conflictLine {get; private set;}
        public List<int> conflictWith {get; private set;} = new List<int>();
        public ConflictData(int line) {
            conflictLine = line;
        }
        public void AddConflictWith(int line){
            conflictWith.Add(line);
        }
    }
}