using System.Collections.Generic;

namespace LineUpGame
{
    public class MoveHistory
    {
        private Stack<int> undoStack = new();
        private Stack<int> redoStack = new();

        public void RecordMove(int column)
        {
            undoStack.Push(column);
            redoStack.Clear();
        }

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;

        public int Undo()
        {
            int move = undoStack.Pop();
            redoStack.Push(move);
            return move;
        }

        public int Redo()
        {
            int move = redoStack.Pop();
            undoStack.Push(move);
            return move;
        }
    }
}
