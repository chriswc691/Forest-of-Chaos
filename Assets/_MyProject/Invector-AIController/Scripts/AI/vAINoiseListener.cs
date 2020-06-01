using System.Collections.Generic;
using UnityEngine;
using System;

namespace Invector.vCharacterController.AI
{
    /// <summary>
    /// Is a <seealso cref="vIAIComponent"/> use with the vNoiseObject to hear a noise, you can also create custom noises 
    /// </summary>
    [DisallowMultipleComponent]
    [vClassHeader("AI Noise Listener")]
    public class vAINoiseListener : vMonoBehaviour, vIAIComponent
    {
        [vHelpBox("The noise has a radius effect and the noise volume decreases depending on the distance, 'Listener Power'  will applify the distance of the noise to listener"), Range(0f, 10f)]
        public float listenerPower = 1;

        public bool debugMode;

        public List<string> ignoreNoiseType;

        public Type ComponentType
        {
            get
            {
                return typeof(vAINoiseListener);
            }
        }

        protected virtual bool IsInListenerPower(vNoise noise)
        {
            return NoiseVolume(noise) > 0;
        }

        protected virtual List<vNoise> NoisesCanBeListened()
        {
            var noisesCanBeListened = noises.FindAll(n => !ignoreNoiseType.Contains(n.noiseType) && IsInListenerPower(n));

            return noisesCanBeListened;
        }

        protected virtual List<vNoise> SortByDistance()
        {
            var noisesCanBeListened = NoisesCanBeListened();
            if (noisesCanBeListened.Count > 1)
                noisesCanBeListened.Sort(delegate (vNoise noiseA, vNoise noiseB)
                {
                    return Vector3.Distance(transform.position, noiseA.position).CompareTo
                                ((Vector3.Distance(transform.position, noiseB.position)));
                });
            if (noisesCanBeListened.Count > 0)
                lastListenedNoise = noisesCanBeListened[0];
            return noisesCanBeListened;
        }

        protected virtual List<vNoise> SortNoisesTypeByDistance(string noiseType)
        {
            var noisesCanBeListened = NoisesCanBeListened();
            var noisesFromType = noisesCanBeListened.FindAll(n => n.noiseType.Equals(noiseType));
            if (noisesFromType.Count > 1)
                noisesFromType.Sort(delegate (vNoise noiseA, vNoise noiseB)
                {
                    return Vector3.Distance(transform.position, noiseA.position).CompareTo
                                ((Vector3.Distance(transform.position, noiseB.position)));
                });
            if (noisesFromType.Count > 0)
                lastListenedNoise = noisesFromType[0];
            return noisesFromType;
        }

        protected virtual List<vNoise> SortNoisesTypesByDistance(List<string> noiseTypes)
        {
            var noisesCanBeListened = NoisesCanBeListened();
            var noisesFromType = noisesCanBeListened.FindAll(n =>noiseTypes.Contains(n.noiseType));
            if (noisesFromType.Count > 1)
                noisesFromType.Sort(delegate (vNoise noiseA, vNoise noiseB)
                {
                    return Vector3.Distance(transform.position, noiseA.position).CompareTo
                                ((Vector3.Distance(transform.position, noiseB.position)));
                });
            if (noisesFromType.Count > 0)
                lastListenedNoise = noisesFromType[0];
            return noisesFromType;
        }

        protected virtual List<vNoise> noises { get { return vAINoiseManager.Instance.noises; } }

        /// <summary>
        /// Target Noise is automatically when use Any Get noise Methods"/>
        /// </summary>
        public vNoise lastListenedNoise { get; protected set; }

        /// <summary>
        /// Get noise volume relative to <seealso cref="listenerPower"/>
        /// </summary>
        /// <param name="noise">noise to check</param>
        /// <returns></returns>
        public float NoiseVolume(vNoise noise)
        {
            var progress = 0f;
            if (listenerPower > 0)
            {
                var minDistance = noise.minDistance * listenerPower;
                var maxDistance = noise.maxDistance * listenerPower;
                var relativeDistance = Vector3.Distance(noise.position, transform.position) - minDistance;
                progress = 1f - (relativeDistance / (minDistance == maxDistance ? maxDistance : minDistance > maxDistance ? minDistance - maxDistance : maxDistance - minDistance));
            }
            return noise.volume * progress;
        }

        /// <summary>
        /// Check if is listening any noise
        /// </summary>    
        /// <returns>Return true if is listening any noise</returns>
        public virtual bool IsListeningNoise()
        {
            return SortByDistance().Count > 0;
        }

        /// <summary>
        /// Check if is listening  specific noises
        /// </summary>      
        /// <param name="noiseTypes">types of noises to check</param>
        /// <returns>Return true if is listening any noise in the list of types</returns>
        public virtual bool IsListeningSpecificNoises(List<string> noiseTypes)
        {
            return SortNoisesTypesByDistance(noiseTypes).Count > 0;
        }

        /// <summary>
        /// Get near noise if is listening any noise"/>
        /// </summary>      
        /// <returns>Ner Noise</returns>
        public virtual vNoise GetNearNoise()
        {
            var noisesByDistance = SortByDistance();
            if (noisesByDistance.Count > 0)
                return noises[0];
            else return null;
        }

        /// <summary>
        /// Get noise by type if is listening a specific noise"/>
        /// </summary>       
        /// <param name="noiseType">type of noise to get</param>
        /// <returns>Near Noise </returns>
        public virtual vNoise GetNearNoiseByType(string noiseType)
        {
            var noisesByType = SortNoisesTypeByDistance(noiseType);
            if (noisesByType.Count > 0)
                return noises[0];
            else return null;
        }
        /// <summary>
        /// Get near noise by types if is listening a specific noise"/>
        /// </summary>      
        /// <param name="noiseTypes">types of noises to get</param>
        /// <returns>Near noise</returns>
        public virtual vNoise GetNearNoiseByTypes(List<string> noiseTypes)
        {
            var noisesByType = SortNoisesTypesByDistance(noiseTypes);
            if (noisesByType.Count > 0)
                return noises[0];
            else return null;
        }

        /// <summary>
        /// Get noises by type if is listening  specific noises"/>
        /// </summary>       
        /// <param name="noiseTypes">types of noises to get</param>
        /// <param name="sortByDistance">sort list by noises distance</param>
        /// <returns>List of Noises that can be Listener</returns>
        public virtual List<vNoise> GetNoiseByTypes(List<string> noiseTypes)
        {
            var noisesByType = SortNoisesTypesByDistance(noiseTypes);
            if (noisesByType.Count > 0)
            {
                return noises;
            }
               
            else return null;
        }
    }
}