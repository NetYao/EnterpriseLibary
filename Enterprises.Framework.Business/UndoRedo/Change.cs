

using System;

namespace Enterprises.Framework.UndoRedo
{
    internal class Change<TState>
    {
        public TState OldState;
        public TState NewState;
    }
}