using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace VehiclePackage
{
    [System.Serializable]
    public class Seat
    {
        public Transform placePosition;
        private GameObject player;
        public bool isDriverSeat;

        public void SetPlayer(GameObject obj)
        {
            if(obj.CompareTag("Player"))
            player = obj;
        }
        
        public void RemovePlayer()
        {
            player = null;
        }

        public GameObject GetPlayer()
        {
            return player;
        }
        
        public bool HasPlayer()
        {
            return player != null;
        }
    }

    public enum VehicleSoundType{
        start,running,end
    }
    
    [System.Serializable]
    public class VehicleSounds
    {
        public AudioClip engineStart;
        public AudioClip engineRunning;
        public AudioClip engineEnd;
        
    }

//    [RequireComponent(typeof(Rigidbody))]
    public class VehicleController : MonoBehaviour
    {
        protected AudioSource audioSrc;
        public List<Seat> seats;
        public VehicleSounds sounds;
        public float maxMotorTorque;
        public float maxSteeringAngle;
        [SerializeField] public GameObject leftHandIKSteeringWheel;
        [SerializeField] public GameObject rightHandIKSteeringWheel;
        [SerializeField] public GameObject steeringWheel;
        [SerializeField] public GameObject exit;
        [SerializeField] public Transform centerOfMass;
        
        public float topSpeed = 100; // km per hour
        protected float currentSpeed = 0;
        protected float pitch = 0;

        protected bool isIgnited;
        
        protected AudioSource audioEngineStart;
        protected AudioSource audioEngineRunning;
        protected AudioSource audioEngineEnd;
        
        public void Start(){
            // add the necessary AudioSources:
            audioEngineStart = AddAudio(sounds.engineStart, false, false, 0.2f);
            audioEngineRunning = AddAudio(sounds.engineRunning, true, false, 0.2f);
            audioEngineEnd = AddAudio(sounds.engineEnd, false, false, 0.8f);
        }

        protected bool HasDriver()
        {
            foreach (var seat in seats)
            {
                if (seat.isDriverSeat && seat.HasPlayer())
                {
                    return true;
                }
            }
            return false;
        }

        public void RemovePlayer(GameObject player)
        {
            if (!player.CompareTag("Player")) return;
            foreach (var seat in seats)
            {
                if (seat.GetPlayer() == player)
                {
                    if (seat.isDriverSeat)
                    {
                        TurnOffEngine();
                    }

                    seat.RemovePlayer();
                }
            }
        }

        public Vector3 getExitPosition()
        {
            return exit.transform.position;
        }

        /**
         * 
         */
        [CanBeNull]
        public Seat SetPlayer(GameObject player)
        {
            if (!player.CompareTag("Player")) return null;
            
            foreach (var seat in seats)
            {
                if (seat.HasPlayer()) continue;
                //We play start engine sound only if player seats in driver position
                if (seat.isDriverSeat)
                {
                    StartEngine();
                }
                seat.SetPlayer(player);
                return seat;
            }

            return null;
        }

        public bool IsDriver(GameObject obj)
        {
            if (!obj.CompareTag("Player")) return false;
            foreach (var seat in seats)
            {
                if (seat.isDriverSeat && seat.GetPlayer() == obj)
                {
                    return true;
                }
            }

            return false;
        }

        private void StartEngine()
        {
            audioEngineStart.Play();
            audioEngineRunning.PlayDelayed(2.366f);
            StartCoroutine( SetIsIgnited(2.366f) );


        }

        private IEnumerator SetIsIgnited(float wait)
        {
            yield return new WaitForSeconds(wait);
            isIgnited = true;
        }

        private void TurnOffEngine()
        {
            audioEngineEnd.Play();
            audioEngineRunning.Stop();
        }

        private AudioSource AddAudio (AudioClip clip, bool loop, bool playAwake, float vol) {
            var newAudio = gameObject.AddComponent<AudioSource>();
            newAudio.clip = clip;
            newAudio.loop = loop;
            newAudio.playOnAwake = playAwake;
            newAudio.volume = vol;
            return newAudio;
        }
        
    }
    

}