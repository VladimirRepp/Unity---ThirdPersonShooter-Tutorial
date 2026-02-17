using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(AudioSource), typeof(SurfaceDetector))]
public class PlayerFootstepSounds : MonoBehaviour
{
    [System.Serializable]
    public class MovementSettings
    {
        public float walkStepInterval = 0.5f;
        public float sprintStepInterval = 0.3f;
        public float crouchStepInterval = 0.7f;
        public float velocityThreshold = 0.1f;
        public float landSoundVelocityThreshold = 3f;
    }

    [Header("Movement Settings")]
    public MovementSettings movementSettings = new();

    [Header("Timing Settings")]
    [SerializeField] private float stepTimer = 0f;
    [SerializeField] private bool wasGroundedLastFrame = true;
    [SerializeField] private float currentStepInterval;

    private StarterAssetsInputs _input;

    private CharacterController characterController;
    private AudioSource audioSource;
    private SurfaceDetector surfaceDetector;

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (surfaceDetector == null)
            surfaceDetector = GetComponent<SurfaceDetector>();

        audioSource.spatialBlend = 1f; // 3D звук
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 20f;

        _input = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        if (!characterController.isGrounded ||
            characterController.velocity.magnitude < movementSettings.velocityThreshold)
        {
            stepTimer = 0f;
            return;
        }

        HandleFootsteps();
        HandleLandingSound();

        wasGroundedLastFrame = characterController.isGrounded;
    }

    private void HandleFootsteps()
    {
        // Определяем интервал шагов в зависимости от состояния
        if (_input.sprint && characterController.velocity.magnitude > 3f)
        {
            currentStepInterval = movementSettings.sprintStepInterval;
        }
        else if (_input.crouch)
        {
            currentStepInterval = movementSettings.crouchStepInterval;
        }
        else
        {
            currentStepInterval = movementSettings.walkStepInterval;
        }

        // Таймер шагов
        stepTimer += Time.deltaTime;

        if (stepTimer >= currentStepInterval)
        {
            PlayFootstepSound();
            stepTimer = 0f;
        }
    }

    private void HandleLandingSound()
    {
        if (characterController.isGrounded && !wasGroundedLastFrame)
        {
            // Рассчитываем скорость падения (примерно)
            float fallVelocity = Mathf.Abs(characterController.velocity.y);

            if (fallVelocity > movementSettings.landSoundVelocityThreshold)
            {
                PlayLandSound();
            }
        }
    }

    private void PlayFootstepSound()
    {
        var (surfaceTag, physicMaterial) = surfaceDetector.GetCurrentSurfaceInfo();
        var surfaceSet = AudioManager.Instance.GetSurfaceSoundSet(surfaceTag, physicMaterial);

        AudioClip[] clipsToUse;

        // Выбираем набор звуков в зависимости от действия
        if (_input.sprint && characterController.velocity.magnitude > 3f)
        {
            clipsToUse = surfaceSet.sprintSounds.Length > 0 ?
                         surfaceSet.sprintSounds : surfaceSet.footstepSounds;
        }
        else
        {
            clipsToUse = surfaceSet.footstepSounds;
        }

        AudioClip clip = AudioManager.Instance.GetRandomClip(clipsToUse);

        if (clip != null)
        {
            audioSource.pitch = AudioManager.Instance.GetRandomizedPitch();
            audioSource.volume = AudioManager.Instance.footstepVolume * surfaceSet.volumeMultiplier;
            audioSource.PlayOneShot(clip);
        }
    }

    private void PlayLandSound()
    {
        var (surfaceTag, physicMaterial) = surfaceDetector.GetCurrentSurfaceInfo();
        var surfaceSet = AudioManager.Instance.GetSurfaceSoundSet(surfaceTag, physicMaterial);

        AudioClip clip = AudioManager.Instance.GetRandomClip(surfaceSet.landSounds);

        if (clip != null)
        {
            audioSource.pitch = AudioManager.Instance.GetRandomizedPitch();
            audioSource.volume = AudioManager.Instance.landVolume * surfaceSet.volumeMultiplier;
            audioSource.PlayOneShot(clip);
        }
    }
}

