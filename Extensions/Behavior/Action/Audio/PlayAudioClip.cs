using System;
using Ceres;
using Ceres.Annotations;
using UnityEngine;
namespace Kurisu.NGDT.Behavior
{
    [Serializable]
    [NodeInfo("Action: Play audioClip on target audioSource")]
    [CeresLabel("Audio: PlayAudioClip")]
    [NodeGroup("Audio")]
    public class PlayAudioClip : Action
    {
        public SharedUObject<AudioClip> audioClip;
        
        public SharedUObject<AudioSource> audioSource;
        
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
