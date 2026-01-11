using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public abstract class CarAudioBase : MonoBehaviour
    {
        public enum EngineAudioOptions
        {
            Simple,
            FourChannel
        }

        // Shared serialized audio settings
        public EngineAudioOptions engineSoundStyle = EngineAudioOptions.FourChannel;
        public AudioClip lowAccelClip;
        public AudioClip lowDecelClip;
        public AudioClip highAccelClip;
        public AudioClip highDecelClip;
        public float pitchMultiplier = 1f;
        public float lowPitchMin = 1f;
        public float lowPitchMax = 6f;
        public float highPitchMultiplier = 0.25f;
        public float maxRolloffDistance = 500;
        public float dopplerLevel = 1;
        public bool useDoppler = true;

        // Optional target used for rolloff distance; assign a camera or listener in the Inspector.
        [SerializeField] private Transform ListenerTarget;

        // Runtime audio sources
        protected AudioSource m_LowAccel;
        protected AudioSource m_LowDecel;
        protected AudioSource m_HighAccel;
        protected AudioSource m_HighDecel;
        protected bool m_StartedSound;

        // Children must provide access to controller values
        protected abstract float GetRevs();
        protected abstract float GetAccelInput();

        private Transform ResolveRolloffTarget()
        {
            if (ListenerTarget != null) return ListenerTarget;
            var cam = Camera.main;
            if (cam != null) return cam.transform;
            var listener = FindObjectOfType<AudioListener>();
            return listener != null ? listener.transform : null;
        }

        // Public for children to call from Update()
        protected void UpdateAudio()
        {
            var target = ResolveRolloffTarget();
            // If no listener/camera is found, assume "near" so audio still starts.
            float camDistSq = (target != null)
                ? (target.position - transform.position).sqrMagnitude
                : 0f;

            if (m_StartedSound && camDistSq > maxRolloffDistance * maxRolloffDistance)
            {
                StopAudio();
            }

            if (!m_StartedSound && camDistSq < maxRolloffDistance * maxRolloffDistance)
            {
                StartAudio();
            }

            if (!m_StartedSound) return;

            float pitch = ULerp(lowPitchMin, lowPitchMax, GetRevs());
            pitch = Mathf.Min(lowPitchMax, pitch);

            if (engineSoundStyle == EngineAudioOptions.Simple)
            {
                m_HighAccel.pitch = pitch * pitchMultiplier * highPitchMultiplier;
                m_HighAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
                m_HighAccel.volume = 1;
            }
            else
            {
                m_LowAccel.pitch = pitch * pitchMultiplier;
                m_LowDecel.pitch = pitch * pitchMultiplier;
                m_HighAccel.pitch = pitch * highPitchMultiplier * pitchMultiplier;
                m_HighDecel.pitch = pitch * highPitchMultiplier * pitchMultiplier;

                float accFade = Mathf.Abs(GetAccelInput());
                float decFade = 1 - accFade;

                float highFade = Mathf.InverseLerp(0.2f, 0.8f, GetRevs());
                float lowFade = 1 - highFade;

                highFade = 1 - ((1 - highFade) * (1 - highFade));
                lowFade = 1 - ((1 - lowFade) * (1 - lowFade));
                accFade = 1 - ((1 - accFade) * (1 - accFade));
                decFade = 1 - ((1 - decFade) * (1 - decFade));

                m_LowAccel.volume = lowFade * accFade;
                m_LowDecel.volume = lowFade * decFade;
                m_HighAccel.volume = highFade * accFade;
                m_HighDecel.volume = highFade * decFade;

                float dop = useDoppler ? dopplerLevel : 0;
                m_HighAccel.dopplerLevel = dop;
                m_LowAccel.dopplerLevel = dop;
                m_HighDecel.dopplerLevel = dop;
                m_LowDecel.dopplerLevel = dop;
            }
        }

        protected void StartAudio()
        {
            m_HighAccel = SetUpEngineAudioSource(highAccelClip);

            if (engineSoundStyle == EngineAudioOptions.FourChannel)
            {
                m_LowAccel = SetUpEngineAudioSource(lowAccelClip);
                m_LowDecel = SetUpEngineAudioSource(lowDecelClip);
                m_HighDecel = SetUpEngineAudioSource(highDecelClip);
            }

            m_StartedSound = true;
        }

        protected void StopAudio()
        {
            foreach (var source in GetComponents<AudioSource>())
            {
                Destroy(source);
            }
            m_StartedSound = false;
        }

        private AudioSource SetUpEngineAudioSource(AudioClip clip)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = 0;
            source.loop = true;
            source.time = clip != null ? Random.Range(0f, clip.length) : 0f;
            source.Play();
            source.minDistance = 5;
            source.maxDistance = maxRolloffDistance;
            source.dopplerLevel = 0;
            return source;
        }

        // unclamped versions of Lerp
        protected static float ULerp(float from, float to, float value)
        {
            return (1.0f - value) * from + value * to;
        }
    }
}
