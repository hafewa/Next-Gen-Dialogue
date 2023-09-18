using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
namespace Kurisu.NGDT.Editor
{
    public interface IDialogueTreeView
    {
        GraphView GraphView { get; }
        void SelectGroup(IDialogueNode node);
        void UnSelectGroup();
        IDialogueNode DuplicateNode(IDialogueNode node);
        event Action<SharedVariable> OnPropertyNameChange;
        IList<SharedVariable> ExposedProperties { get; }
        bool IsRestoring { get; }
        Action<IDialogueNode> OnSelectAction { get; }
        void AddExposedProperty(SharedVariable variable);
        void BakeDialogue();
    }
}
