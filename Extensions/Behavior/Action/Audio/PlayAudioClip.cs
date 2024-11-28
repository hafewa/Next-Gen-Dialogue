using Ceres;
using Ceres.Annotations;
using UnityEngine;
namespace Kurisu.NGDT.Behavior
{
    [NodeInfo("Action: Play audioClip on target audioSource")]
    [NodeLabel("Audio: PlayAudioClip")]
    [NodeGroup("Audio")]
    public class PlayAudioClip : Action
    {
        public SharedTObject<AudioClip> audioClip;
        public SharedTObject<AudioSource> audioSource;
        protected override Status OnUpdate()
        {
            if (audioSource.Value)
            {
                audioSource.Value.clip = audioClip.Value;
                audioSource.Value.Play();
            }
            return Status.Success;
        }
    }
}
