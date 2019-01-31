using JetBrains.Annotations;
using UnityEngine;
using VehiclePackage;

namespace CharacterPackage.Scripts
{
	
	[RequireComponent(typeof(Animator))] 
	[RequireComponent(typeof(UserController))] 

	public class HandsIKHandler : MonoBehaviour
	{
		private Animator anim;
		private UserController uc;

		Vector3 leftHandPosition;
		Vector3 rightHandPosition;

		Quaternion leftHandRotation;
		Quaternion rightHandRotation;

		float leftHandWeight;
		float rightHandWeight;

		private Transform leftHand;
		private Transform rightHand;
		
		private static readonly int InCar = Animator.StringToHash("inCar");

		// Use this for initialization
		void Start()
		{
			anim = GetComponent<Animator>();
			uc = GetComponent<UserController>();

			//set feet directly from code as we expect humanoid
			leftHand = anim.GetBoneTransform(HumanBodyBones.LeftHand);
			rightHand = anim.GetBoneTransform(HumanBodyBones.RightHand);

			leftHandRotation = leftHand.rotation;
			rightHandRotation = rightHand.rotation;
		}

		// Update is called once per frame
		void Update ()
		{

			//Proceed only when user is in vehicle
			if (!anim.GetBool(InCar) && !uc.IsInVehicle())
			{
				return;
			}

			var leftIKTransfrom = uc.GetVehicleController().leftHandIKSteeringWheel.transform;
			var rightIKTransfrom = uc.GetVehicleController().rightHandIKSteeringWheel.transform;

			leftHandPosition = leftIKTransfrom.position;
			rightHandPosition = rightIKTransfrom.position;
			//
			leftHandRotation =  leftIKTransfrom.rotation;
			rightHandRotation =  rightIKTransfrom.rotation;
		
		}

		private void OnAnimatorIK()
		{
			var weight = 1f;
			
			//Position
			anim.SetIKPositionWeight(AvatarIKGoal.LeftHand,weight);
			anim.SetIKPositionWeight(AvatarIKGoal.RightHand,weight);
			
			anim.SetIKPosition(AvatarIKGoal.LeftHand,leftHandPosition);
			anim.SetIKPosition(AvatarIKGoal.RightHand,rightHandPosition);
			
			//Rotation
			anim.SetIKRotationWeight(AvatarIKGoal.LeftHand,weight);
			anim.SetIKRotationWeight(AvatarIKGoal.RightHand,weight);

			anim.SetIKRotation(AvatarIKGoal.LeftHand,leftHandRotation);
			anim.SetIKRotation(AvatarIKGoal.RightHand,rightHandRotation);

		}
	}
}
