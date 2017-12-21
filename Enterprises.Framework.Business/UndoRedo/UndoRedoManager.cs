

using System;
using System.Collections.Generic;

namespace Enterprises.Framework.UndoRedo
{
    /// <summary>
    /// 提交撤销管理
    /// </summary>
    public static class UndoRedoManager
    {
        private static readonly List<Command> history = new List<Command>();
        private static int currentPosition = -1;
        private static Command currentCommand;
        private static int maxHistorySize;

        public static event EventHandler<CommandDoneEventArgs> CommandDone;

        /// <summary>
        /// Returns true if history has command that can be undone
        /// </summary>
        public static bool CanUndo
        {
            get
            {
                return currentPosition >= 0;
            }
        }

        /// <summary>
        /// Returns true if history has command that can be redone
        /// </summary>
        public static bool CanRedo
        {
            get
            {
                return currentPosition < history.Count - 1;
            }
        }

        public static bool IsCommandStarted
        {
            get
            {
                return currentCommand != null;
            }
        }

        /// <summary>Gets an enumeration of commands captions that can be undone.</summary>
        /// <remarks>The first command in the enumeration will be undone first</remarks>
        public static IEnumerable<string> UndoCommands
        {
            get
            {
                for (int i = currentPosition; i >= 0; i--)
                {
                    yield return history[i].Caption;
                }
            }
        }

        /// <summary>Gets an enumeration of commands captions that can be redone.</summary>
        /// <remarks>The first command in the enumeration will be redone first</remarks>
        public static IEnumerable<string> RedoCommands
        {
            get
            {
                for (int i = currentPosition + 1; i < history.Count; i++)
                {
                    yield return history[i].Caption;
                }
            }
        }

        /// <summary>
        /// Gets/sets max commands stored in history. 
        /// Zero value (default) sets unlimited history size.
        /// </summary>
        public static int MaxHistorySize
        {
            get
            {
                return maxHistorySize;
            }
            set
            {
                if (IsCommandStarted)
                {
                    throw new InvalidOperationException("Max size may not be set while command is run.");
                }
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Value may not be less than 0");
                }
                maxHistorySize = value;
                TruncateHistory();
            }
        }

        internal static Command CurrentCommand
        {
            get
            {
                return currentCommand;
            }
        }

        /// <summary>
        /// Undo last command from history list
        /// </summary>
        public static void Undo()
        {
            AssertNoCommand();
            if (CanUndo)
            {
                Command command = history[currentPosition--];
                foreach (IUndoRedoMember member in command.Keys)
                {
                    member.OnUndo(command[member]);
                }
                OnCommandDone(CommandDoneType.Undo);
            }
        }

        /// <summary>
        /// Repeats command that was undone before
        /// </summary>
        public static void Redo()
        {
            AssertNoCommand();
            if (CanRedo)
            {
                Command command = history[++currentPosition];
                foreach (IUndoRedoMember member in command.Keys)
                {
                    member.OnRedo(command[member]);
                }
                OnCommandDone(CommandDoneType.Redo);
            }
        }

        /// <summary>
        /// Start command. Any data changes must be done within a command.
        /// </summary>
        /// <param name="commandCaption"></param>
        /// <returns></returns>
        public static IDisposable Start(string commandCaption)
        {
            AssertNoCommand();
            currentCommand = new Command(commandCaption);
            return currentCommand;
        }

        /// <summary>
        /// Commits current command and saves changes into history
        /// </summary>
        public static void Commit()
        {
            AssertCommand();
            foreach (IUndoRedoMember member in currentCommand.Keys)
            {
                member.OnCommit(currentCommand[member]);
            }

            // add command to history (all redo records will be removed)
            int count = history.Count - currentPosition - 1;
            history.RemoveRange(currentPosition + 1, count);

            history.Add(currentCommand);
            currentPosition++;
            TruncateHistory();

            currentCommand = null;
            OnCommandDone(CommandDoneType.Commit);
        }

        /// <summary>
        /// Rollback current command. It does not saves any changes done in current command.
        /// </summary>
        public static void Cancel()
        {
            AssertCommand();
            foreach (IUndoRedoMember member in currentCommand.Keys)
            {
                member.OnUndo(currentCommand[member]);
            }
            currentCommand = null;
        }

        /// <summary>
        /// Clears all history. It does not affect current data but history only. 
        /// It is usefull after any data initialization if you want forbid user to undo this initialization.
        /// </summary>
        public static void FlushHistory()
        {
            currentCommand = null;
            currentPosition = -1;
            history.Clear();
        }

        /// <summary>Checks there is no command started</summary>
        internal static void AssertNoCommand()
        {
            if (currentCommand != null)
            {
                throw new InvalidOperationException(
                    "Previous command is not completed. Use UndoRedoManager.Commit() to complete current command.");
            }
        }

        /// <summary>Checks that command had been started</summary>
        internal static void AssertCommand()
        {
            if (currentCommand == null)
            {
                throw new InvalidOperationException("Command is not started. Use method UndoRedoManager.Start().");
            }
        }

        private static void OnCommandDone(CommandDoneType type)
        {
            if (CommandDone != null)
            {
                CommandDone(null, new CommandDoneEventArgs(type));
            }
        }

        private static void TruncateHistory()
        {
            if (maxHistorySize > 0)
            {
                if (history.Count > maxHistorySize)
                {
                    int count = history.Count - maxHistorySize;
                    history.RemoveRange(0, count);
                    currentPosition -= count;
                }
            }
        }
    }
}