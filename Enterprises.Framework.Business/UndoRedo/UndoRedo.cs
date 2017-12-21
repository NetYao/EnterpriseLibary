

using System.Diagnostics;

namespace Enterprises.Framework.UndoRedo
{
    [DebuggerDisplay("{Value}")]
    public class UndoRedo<TValue> : IUndoRedoMember
    {
        private TValue tValue;

        public UndoRedo()
        {
            this.tValue = default(TValue);
        }

        public UndoRedo(TValue defaultValue)
        {
            this.tValue = defaultValue;
        }

        public TValue Value
        {
            get
            {
                return this.tValue;
            }
            set
            {
                UndoRedoManager.AssertCommand();
                if (!UndoRedoManager.CurrentCommand.ContainsKey(this))
                {
                    var change = new Change<TValue>();
                    change.OldState = this.tValue;
                    UndoRedoManager.CurrentCommand[this] = change;
                }
                this.tValue = value;
            }
        }

        #region IUndoRedoMember Members

        void IUndoRedoMember.OnCommit(object change)
        {
            Debug.Assert(change != null);
            ((Change<TValue>)change).NewState = this.tValue;
        }

        void IUndoRedoMember.OnUndo(object change)
        {
            Debug.Assert(change != null);
            this.tValue = ((Change<TValue>)change).OldState;
        }

        void IUndoRedoMember.OnRedo(object change)
        {
            Debug.Assert(change != null);
            this.tValue = ((Change<TValue>)change).NewState;
        }
        
        #endregion
    }
}