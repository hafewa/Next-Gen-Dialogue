using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
namespace Kurisu.NGDT.Editor
{
    public class DecoratorNode : DialogueTreeNode
    {
        private readonly Port childPort;

        public Port Child => childPort;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Add(new NGDTDropdownMenuAction("Change Behavior", (a) =>
            {
                var provider = ScriptableObject.CreateInstance<DecoratorSearchWindowProvider>();
                provider.Init(this, NextGenDialogueSetting.GetMask());
                SearchWindow.Open(new SearchWindowContext(a.eventInfo.localMousePosition), provider);
            }));
            base.BuildContextualMenu(evt);
        }

        public DecoratorNode()
        {
            AddToClassList("DecoratorNode");
            childPort = CreateChildPort();
            outputContainer.Add(childPort);
        }

        protected override bool OnValidate(Stack<IDialogueNode> stack)
        {
            if (!childPort.connected)
            {
                return false;
            }
            stack.Push(childPort.connections.First().input.node as DialogueTreeNode);
            return true;
        }

        protected override void OnCommit(Stack<IDialogueNode> stack)
        {
            if (!childPort.connected)
            {
                (NodeBehavior as Decorator).Child = null;
                return;
            }
            var child = PortHelper.FindChildNode(childPort);
            (NodeBehavior as Decorator).Child = child.ReplaceBehavior();
            stack.Push(child);
        }

        protected override void OnClearStyle()
        {
            if (!childPort.connected) return;
            var child = PortHelper.FindChildNode(childPort);
            child.ClearStyle();
        }

    }
}