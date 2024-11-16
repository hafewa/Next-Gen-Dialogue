using System;
using Ceres.Node;
using Kurisu.NGDS;
using UnityEngine;
namespace Kurisu.NGDT
{
    [Serializable]
    public abstract class Module : NodeBehavior
    {
        protected override void OnRun() { }
    }
    [Serializable]
    public abstract class BehaviorModule : Module
    {
        [SerializeReference]
        private NodeBehavior child;
        public NodeBehavior Child
        {
            get => child;
#if UNITY_EDITOR
            set => child = value;
#endif
        }

        protected sealed override void OnRun()
        {
            child?.Run(GameObject, Tree);
        }

        public override void Awake()
        {
            child?.Awake();
        }

        public override void Start()
        {
            child?.Start();
        }
        public sealed override CeresNode GetChildAt(int index)
        {
            return child;
        }
        public sealed override int GetChildrenCount()
        {
            return child == null ? 0 : 1;
        }
        public sealed override void ClearChildren()
        {
            child = null;
        }
        public sealed override void AddChild(CeresNode nodeBehavior)
        {
            child = nodeBehavior as NodeBehavior;
        }
        public sealed override void SetChildren(CeresNode[] inChildren)
        {
            child = inChildren[0] as NodeBehavior;
        }
        public sealed override CeresNode[] GetChildren()
        {
            if (child == null) return base.GetChildren();
            return new CeresNode[1] { child };
        }
    }
    [Serializable]
    public abstract class CustomModule : Module
    {
        protected sealed override void OnRun()
        {
        }
        protected sealed override Status OnUpdate()
        {
            Tree.Builder.GetNode().AddModule(GetModule());
            return Status.Success;
        }
        protected abstract IDialogueModule GetModule();
    }
}
