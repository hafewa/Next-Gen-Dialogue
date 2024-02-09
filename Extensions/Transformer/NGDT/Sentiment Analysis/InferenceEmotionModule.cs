using UnityEngine;
using Kurisu.NGDS.Transformer;
using Kurisu.NGDS;
namespace Kurisu.NGDT.Transformer.SA
{
    [AkiInfo("Module: Inference Emotion Module is used to inference emotion labelId based on container's content at runtime.")]
    [ModuleOf(typeof(Piece))]
    [ModuleOf(typeof(Option))]
    [AkiGroup("AIGC/Transformer")]
    public class InferenceEmotionModule : Module
    {
        [Tooltip("Sentiment analysis inference engine")]
        public SharedTObject<SentimentAnalysisEngine> saEngine;
        [ForceShared]
        public SharedInt labelId;
        public SharedFloat probability;
        protected sealed override Status OnUpdate()
        {
            var content = Tree.Builder.GetNode() as IContent;
            labelId.Value = saEngine.Value.Classify(content.Content, out float probabilityValue);
            probability.Value = probabilityValue;
            return Status.Success;
        }
    }
}
