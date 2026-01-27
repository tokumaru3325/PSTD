using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffectGroup", menuName = "Audio/SoundEffectGroup")]
public class SoundEffectGroup : ScriptableObject
{
    public SoundId id;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.5f, 2f)]
    public float pitchMin = 1f;

    [Range(0.5f, 2f)]
    public float pitchMax = 1f;

    public List<AudioClip> clips;

    [Min(0f)]
    public float cooldown = 0.3f;
}
