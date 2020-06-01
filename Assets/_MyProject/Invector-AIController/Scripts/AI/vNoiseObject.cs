using UnityEngine;
namespace Invector.vCharacterController.AI
{
    [DisallowMultipleComponent]
    [vClassHeader("Noise Object", "Call the method 'TriggerNoise' or use the option 'TriggerOnStart' to instantly trigger your noise", openClose = false)]
    public class vNoiseObject : vMonoBehaviour
    {
        public string noiseType = "noise";
        public float minDistance = 1, maxDistance = 4;
        public float volume = 1;
        [Range(0.1f, 10f)]
        public float duration = 0.1f;
        public bool triggerOnStart;
        public bool looping;
        public vNoise.vAINoiseEvent onTriggerNoise;
        public vNoise.vAINoiseEvent onFinishNoise;

        vNoise noise;
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, minDistance);
            if (maxDistance < minDistance) minDistance = maxDistance;

            Gizmos.DrawWireSphere(transform.position, maxDistance);
        }
        private void Start()
        {
            if (triggerOnStart) TriggerNoise();
        }

        public void TriggerNoise()
        {
            if (noise == null)
            {
                noise = new vNoise(noiseType, transform.position, volume, minDistance, maxDistance, duration);
                noise.onFinishNoise.AddListener(OnFinishNoise);

            }
            noise.position = transform.position;
            onTriggerNoise.Invoke(noise);
            vAINoiseManager.Instance.AddNoise(noise);
        }

        void OnFinishNoise(vNoise noise)
        {
            onFinishNoise.Invoke(noise); if (looping) Invoke("TriggerNoise", 0.1f);
        }
    }
}
