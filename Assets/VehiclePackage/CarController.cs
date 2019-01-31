using System.Collections.Generic;
using UnityEngine;

namespace VehiclePackage
{
    [System.Serializable]
    public class AxleInfo {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
    }

    public class CarController : VehicleController {
        public List<AxleInfo> axleInfos; 
        private Rigidbody rb; 
        
        //center of mass
        public void Awake()
        {
            if(centerOfMass == null)
                return;
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = centerOfMass.localPosition;

        }
        // finds the corresponding visual wheel
        // correctly applies the transform
        private static void ApplyLocalPositionToVisuals(WheelCollider wheelCollider)
        {
            if (wheelCollider.transform.childCount == 0) {
                return;
            }

            var visualWheel = wheelCollider.transform.GetChild(0);

            Vector3 position;
            Quaternion rotation;
            wheelCollider.GetWorldPose(out position, out rotation);

            var transform1 = visualWheel.transform;
            transform1.position = position;
            transform1.rotation = rotation;
        }

        private void Update()
        {
            currentSpeed = rb.velocity.magnitude * 3.6f;
            Debug.Log(currentSpeed);
            pitch = currentSpeed / topSpeed;
 
            audioEngineRunning.pitch = pitch;
        }

        public void FixedUpdate()
        {
            if (!HasDriver())
            {
                return;
            }
            
            float motor = maxMotorTorque * Input.GetAxis("Vertical");
            float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

            var rotation = steeringWheel.transform.rotation;
            rotation = Quaternion.Euler( rotation.eulerAngles.x, rotation.eulerAngles.y , steering );
            steeringWheel.transform.rotation = rotation;

            foreach (var axleInfo in axleInfos) {
                if (axleInfo.steering) {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }
                if (axleInfo.motor) {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                }
                ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                ApplyLocalPositionToVisuals(axleInfo.rightWheel);
            }
        }
    }
}