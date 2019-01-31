using UnityEngine;

namespace CharacterPackage.Scripts
{
	
	[RequireComponent(typeof(Animator))] 

	public class FeetIKHandler : MonoBehaviour
	{
		private Animator anim;

		Vector3 leftFootPosition;
		Vector3 rightFootPosition;

		Quaternion leftFootRotation;
		Quaternion rightFootRotation;

		float leftFootWeight;
		float rightFootWeight;

		private Transform leftFoot;
		private Transform rightFoot;

		private static readonly int LeftFoot = Animator.StringToHash("LeftFoot");
		private static readonly int RightFoot = Animator.StringToHash("RightFoot");

//		private static readonly float maxDistance = 0.2f;
		public float maxDistance = 0.2f;
		
		//add offset so feet wont sink into terrain
		public float FeetYOffset = 0.2f;
		private static readonly int InCar = Animator.StringToHash("inCar");

		// Use this for initialization
		void Start ()
		{
			anim = GetComponent<Animator>();
			//set feet directly from code as we expect humanoid
			leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
			rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);

			leftFootRotation = leftFoot.rotation;
			rightFootRotation = rightFoot.rotation;
		}
	
		// Update is called once per frame
		void Update ()
		{

			if (anim.GetBool(InCar))
			{
				return;
			}

			RaycastHit leftHit;
			RaycastHit rightHit;

			// ?
			Vector3 lPos = leftFoot.TransformPoint(Vector3.zero);
			Vector3 rPos = rightFoot.TransformPoint(Vector3.zero);

			if (Physics.Raycast(lPos, Vector3.down, out leftHit, maxDistance))
			{
				//get position of foot when it hits the ground
				leftFootPosition = leftHit.point;
				//???? got from net
				leftFootRotation = Quaternion.FromToRotation(transform.up, leftHit.normal) * transform.rotation;
			}
			
			if (Physics.Raycast(rPos, Vector3.down, out rightHit, maxDistance))
			{
				//get position of foot when it hits the ground
				rightFootPosition = rightHit.point;
				//???? got from net
				rightFootRotation = Quaternion.FromToRotation(transform.up, rightHit.normal) * transform.rotation;
			}
		}

		private void OnAnimatorIK()
		{
			leftFootWeight = anim.GetFloat(LeftFoot);
			rightFootWeight = anim.GetFloat(RightFoot);
			
			//feet
			anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot,leftFootWeight);
			anim.SetIKPositionWeight(AvatarIKGoal.RightFoot,rightFootWeight);
			
			anim.SetIKPosition(AvatarIKGoal.LeftFoot,leftFootPosition + new Vector3(0,FeetYOffset,0));
			anim.SetIKPosition(AvatarIKGoal.RightFoot,rightFootPosition + new Vector3(0,FeetYOffset,0));
			
			anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot,leftFootWeight);
			anim.SetIKRotationWeight(AvatarIKGoal.RightFoot,rightFootWeight);
			
			anim.SetIKRotation(AvatarIKGoal.LeftFoot,leftFootRotation);
			anim.SetIKRotation(AvatarIKGoal.RightFoot,rightFootRotation);
			//knees
//			anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee,IKWeight);
//			anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee,IKWeight);
//			
//			anim.SetIKHintPosition(AvatarIKHint.LeftKnee,hintLeft.position);
//			anim.SetIKHintPosition(AvatarIKHint.RightKnee,hintRight.position);
		}
	}
}
