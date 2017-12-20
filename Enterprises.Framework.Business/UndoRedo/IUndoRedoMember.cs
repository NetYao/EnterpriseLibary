// Siarhei Arkhipenka (c) 2006-2007. email: sbs-arhipenko@yandex.ru

namespace Enterprises.Framework.UndoRedo
{
    public interface IUndoRedoMember
    {
        void OnCommit(object change);

        void OnUndo(object change);

        void OnRedo(object change);
    }
}