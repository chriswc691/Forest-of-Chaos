using System.Collections;
using UnityEngine;
namespace Invector.vCharacterController.AI
{
    /// <summary>
    /// Noise Object is used to shoot "sounds" that can be detected by the Controller a through the <seealso cref="vAINoiseListener"/>
    /// </summary> 
    [System.Serializable]
    public class vNoise
    {
        [System.Serializable]
        public class vAINoiseEvent : UnityEngine.Events.UnityEvent<vNoise> { }
        public vNoise(string noiseType,Vector3 position,float volume,float minDistance,float maxDistance,float duration)
        {
            this.noiseType = noiseType;
            this.position = position;
            this.volume = volume;
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
            this.duration = duration;
            AddDuration(duration);           
        }

        public bool isPlaying;// { get; protected set; }
        public vAINoiseEvent onFinishNoise = new vAINoiseEvent();

        public float duration;
        /// <summary>
        /// Add more time to noise finish
        /// </summary>
        /// <param name="duration"></param>
        public void AddDuration(float duration)
        {
            noiseFinishTime = Time.time + duration;
        }
        
        /// <summary>
        /// Remove duraction of the noise
        /// </summary>
        public void CancelNoise()
        {
            noiseFinishTime = 0;
        }

        /// <summary>
        /// Coroutine to control when the noise will finish
        /// </summary>
        /// <param name="onFinishNoiseCallBack">noise finish callback</param>
        /// <returns></returns>
        public IEnumerator Play()
        { 
            if (!isPlaying)
            {
                AddDuration(duration);
                isPlaying = true;
                while (noiseFinishTime > Time.time) yield return null;
                isPlaying = false;
                if (onFinishNoise != null) onFinishNoise.Invoke(this);
               
            }          
        }
        
        /// <summary>
        /// The center of the noise
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// The type of noise. Can be used by <seealso cref="vAINoiseListener"/> to make different decisions
        /// </summary>
        public string noiseType;

        /// <summary>
        /// maximum volume that noise produces
        /// </summary>
        public float volume;
       
        /// <summary>
        /// Minimun distance that the noise can be heard
        /// </summary>
        public float minDistance;

        /// <summary>
        /// maximum distance that the noise can be heard
        /// </summary>
        public float maxDistance;
        
        float noiseFinishTime;
    }
}