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
            if (!HasDriver())
            {
                return;
            }
            currentSpeed = rb.velocity.magnitude * 3.8f;
            pitch = currentSpeed / topSpeed;
            //TODO :: Dont base on pure velocity magnitude - take in count maxMotorTorque ? 
            audioEngineRunning.pitch = 1 + pitch;
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
            steeringWheel.transform.rotation =  Quaternion.Euler( rotation.eulerAngles.x, rotation.eulerAngles.y , -steering );;

            
            foreach (var axleInfo in axleInfos) {
                if (axleInfo.steering) {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }

                if (isIgnited && axleInfo.motor)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                }

                ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                ApplyLocalPositionToVisuals(axleInfo.rightWheel);
            }
        }
    }
}