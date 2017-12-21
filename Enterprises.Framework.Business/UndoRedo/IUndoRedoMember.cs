

namespace Enterprises.Framework.UndoRedo
{
    public interface IUndoRedoMember
    {
        void OnCommit(object change);

        void OnUndo(object change);

        void OnRedo(object change);
    }
}