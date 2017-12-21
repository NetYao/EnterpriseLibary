

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Enterprises.Framework.UndoRedo;

namespace Enterprises.Framework.UndoRedo
{
    internal class Command : Dictionary<IUndoRedoMember, object>, IDisposable
    {
        public readonly string Caption;

        public Command(string caption)
        {
            this.Caption = caption;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (UndoRedoManager.CurrentCommand != null)
            {
                if (UndoRedoManager.CurrentCommand == this)
                {
                    UndoRedoManager.Cancel();
                }
                else
                {
                    Debug.Fail("There was another command within disposed command");
                }
            }
        }
        
        #endregion
    }
}