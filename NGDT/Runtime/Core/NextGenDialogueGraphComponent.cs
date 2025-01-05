using System;
using Ceres.Graph;
using UnityEngine;
using UObject = UnityEngine.Object;
namespace Kurisu.NGDT
{
    [DisallowMultipleComponent]
    public class NextGenDialogueGraphComponent : MonoBehaviour, IDialogueGraphContainer
    {
        [NonSerialized]
        private DialogueGraph _dialogueGraph;
        
        [HideInInspector, SerializeField]
        private DialogueGraphData graphData;
        
        public UObject Object => gameObject;
        
        [SerializeField, Tooltip("Create dialogue graph from external dialogue asset at runtime")]
        private NextGenDialogueGraphAsset externalDialogueGraphAsset;
        
        /// <summary>
        /// Overwrite external dialogueTreeAsset to use external data, and leave null to use embedded data.
        /// </summary>
        /// <value></value>
        public NextGenDialogueGraphAsset ExternalData 
        { 
            get => externalDialogueGraphAsset;
            set
            {
                externalDialogueGraphAsset = value;
                if (_dialogueGraph == null) return;
                
                _dialogueGraph.Dispose();
                _dialogueGraph = null;
                (_dialogueGraph = GetDialogueGraph()).Compile();
            } 
        }

        private void Awake()
        {
            (_dialogueGraph = GetDialogueGraph()).Compile();
        }

        private void OnDestroy()
        {
            _dialogueGraph?.Dispose();
        }

        public CeresGraph GetGraph()
        {
            return GetDialogueGraph();
        }

        /// <summary>
        /// Get or create dialogue graph instance from this component
        /// </summary>
        /// <returns></returns>
        public DialogueGraph GetDialogueGraphInstance()
        {
            return _dialogueGraph ?? GetDialogueGraph();
        }

        /// <summary>
        /// Play dialogue
        /// </summary>
        public void PlayDialogue()
        {
            GetDialogueGraphInstance().PlayDialogue(gameObject);
        }

        /// <summary>
        /// Create new dialogue graph from this component
        /// </summary>
        /// <returns></returns>
        public DialogueGraph GetDialogueGraph()
        {
            if (externalDialogueGraphAsset)
            {
                return externalDialogueGraphAsset.GetDialogueGraph();
            }
            return new DialogueGraph(graphData.CloneT<DialogueGraphData>());
        }

        public void SetGraphData(CeresGraphData graph)
        {
            graphData = (DialogueGraphData)graph;
        }
    }
}
