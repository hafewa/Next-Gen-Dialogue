using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
namespace Kurisu.NGDT.Editor
{
    public class ModuleNode : DialogueTreeNode
    {
        protected sealed override void AddDescription() { }
        protected override void AddParent()
        {
        }
        protected override bool OnValidate(Stack<IDialogueNode> stack) => true;

        protected override void OnCommit(Stack<IDialogueNode> stack) { }

        protected override void OnClearStyle() { }
        public sealed override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            var remainTargets = evt.menu.MenuItems().FindAll(e =>
            {
                return e switch
                {
                    NGDTDropdownMenuAction a => false,
                    DropdownMenuAction a => a.name == "Create Node" || a.name == "Delete",
                    _ => false,
                };
            });
            //Remove needless default actions .
            evt.menu.MenuItems().Clear();
            remainTargets.ForEach(evt.menu.MenuItems().Add);
        }
        protected override void OnGeometryChanged(GeometryChangedEvent evt)
        {
            bool isAttached = GetFirstAncestorOfType<ContainerNode>() != null;
            if (settingButton != null && settingsContainer != null && settingsContainer.parent != null)
            {
                var settingsButtonLayout = settingButton.ChangeCoordinatesTo(settingsContainer.parent, settingButton.layout);
                settingsContainer.style.top = settingsButtonLayout.yMax - (isAttached ? 70f : 20f);
                settingsContainer.style.left = settingsButtonLayout.xMin - layout.width + (isAttached ? 10f : 20f);
            }
        }
    }
    public class EditorModuleNode : ModuleNode
    {

    }
    public class BehaviorModuleNode : ModuleNode
    {
        private readonly Port childPort;
        public Port Child => childPort;
        private IDialogueNode cache;
        public BehaviorModuleNode() : base()
        {
            childPort = CreateChildPort();
            outputContainer.Add(childPort);
        }
        protected override bool OnValidate(Stack<IDialogueNode> stack)
        {
            if (!childPort.connected)
            {
                return true;
            }
            stack.Push(PortHelper.FindChildNode(childPort));
            return true;
        }

        protected override void OnCommit(Stack<IDialogueNode> stack)
        {
            if (!childPort.connected)
            {
                (NodeBehavior as BehaviorModule).Child = null;
                cache = null;
                return;
            }
            var child = PortHelper.FindChildNode(childPort);
            (NodeBehavior as BehaviorModule).Child = child.ReplaceBehavior();
            stack.Push(child);
            cache = child;
        }

        protected override void OnClearStyle()
        {
            cache?.ClearStyle();
            if (childPort.connected)
            {
                PortHelper.FindChildNode(childPort).ClearStyle();
            }
        }
    }
}