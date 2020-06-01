using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector.vCharacterController.AI
{
    public class vAINoiseManager : MonoBehaviour
    {
        static vAINoiseManager _instance;
        public static vAINoiseManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<vAINoiseManager>();
                if (_instance == null)
                {
                    var noiseManager = new GameObject("AI Noise Manager");
                    _instance = noiseManager.AddComponent<vAINoiseManager>();
                    _instance.noises = new List<vNoise>();
                }
                return _instance;
            }
        }
        /// <summary>
        /// List of all noises that is listening
        /// </summary>
        public  List<vNoise> noises;// { get; protected set; }
        
        public void AddNoise(vNoise noise)
        {
            if (noises==null) noises = new List<vNoise>();
            
            if (noises.Contains(noise))
            {               
                noises[noises.IndexOf(noise)].AddDuration(noise.duration);
            }
            else
            {              
                noise.onFinishNoise.AddListener(RemoveNoise);
                noises.Add(noise);               
            }
            if (!noise.isPlaying) StartCoroutine(noise.Play());
        }

        public void RemoveNoise(vNoise noise)
        {
            if (noises == null) noises = new List<vNoise>();
            if (noises.Contains(noise))
            {              
                noises.Remove(noise);
            }
        }
    }
}