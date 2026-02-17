using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class SurfaceSoundSet
    {
        public string surfaceTag = "Ground";
        public AudioClip[] footstepSounds;
        public AudioClip[] jumpSounds;
        public AudioClip[] landSounds;
        public AudioClip[] sprintSounds;
        [Range(0f, 1f)] public float volumeMultiplier = 1f;
        public PhysicsMaterial PhysicsMaterial; // Альтернатива тегу
    }

    [Header("Settings")]
    public SurfaceSoundSet defaultSurface;
    public List<SurfaceSoundSet> surfaceSoundSets = new List<SurfaceSoundSet>();

    [Header("Volumes Settings")]
    [Range(0f, 1f)] public float footstepVolume = 0.5f;
    [Range(0f, 1f)] public float jumpVolume = 0.7f;
    [Range(0f, 1f)] public float landVolume = 0.6f;

    [Header("Random Settings")]
    [Range(0f, 0.5f)] public float pitchRandomness = 0.1f;

    private Dictionary<string, SurfaceSoundSet> surfaceDictionary;
    private Dictionary<PhysicsMaterial, SurfaceSoundSet> PhysicsMaterialDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        surfaceDictionary = new Dictionary<string, SurfaceSoundSet>();
        PhysicsMaterialDictionary = new Dictionary<PhysicsMaterial, SurfaceSoundSet>();

        foreach (var set in surfaceSoundSets)
        {
            if (!string.IsNullOrEmpty(set.surfaceTag))
            {
                surfaceDictionary[set.surfaceTag] = set;
            }

            if (set.PhysicsMaterial != null)
            {
                PhysicsMaterialDictionary[set.PhysicsMaterial] = set;
            }
        }
    }

    public SurfaceSoundSet GetSurfaceSoundSet(string surfaceTag, PhysicsMaterial PhysicsMaterial = null)
    {
        // Приоритет: PhysicsMaterial -> Tag -> Default
        if (PhysicsMaterial != null && PhysicsMaterialDictionary.ContainsKey(PhysicsMaterial))
        {
            return PhysicsMaterialDictionary[PhysicsMaterial];
        }

        if (!string.IsNullOrEmpty(surfaceTag) && surfaceDictionary.ContainsKey(surfaceTag))
        {
            return surfaceDictionary[surfaceTag];
        }

        return defaultSurface;
    }

    public AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }

    public float GetRandomizedPitch()
    {
        return 1f + UnityEngine.Random.Range(-pitchRandomness, pitchRandomness);
    }
}
