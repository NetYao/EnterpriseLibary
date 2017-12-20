using System;

namespace Enterprises.Framework.UndoRedo
{
    public class CommandDoneEventArgs : EventArgs
    {
        public readonly CommandDoneType CommandDoneType;

        public CommandDoneEventArgs(CommandDoneType type)
        {
            this.CommandDoneType = type;
        }
    }
}