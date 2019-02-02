using UnityEngine;

namespace CharacterPackage.Scripts
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Collider))]
	public class HumanoidRagdollBehaviour : MonoBehaviour {
		private Collider mainCollider;
		private Collider[] allColliders;
		private Animator anim;
		private Rigidbody rb;

		private void Awake()
		{
			mainCollider = GetComponent<Collider>();
			allColliders = GetComponentsInChildren<Collider>(true);
			anim = GetComponent<Animator>();
			rb = GetComponent<Rigidbody>();
			DoRagdoll(false);
		}

		public void DoRagdoll(bool isRagdoll)
		{
			if(isRagdoll)
			Debug.Log("doing ragdoll");

			foreach (var col in allColliders)
			{
				col.enabled = isRagdoll;
			}

			mainCollider.enabled = !isRagdoll;
			rb.useGravity = !isRagdoll;
			rb.isKinematic = isRagdoll;
			anim.enabled = !isRagdoll;
		}

		void OnCollisionEnter (Collision collision) {
			// are is the collider heading in the direction of the person 0.5 is arbitrary, it may require more or less

			var col = collision.collider;
			if (!col.CompareTag("Vehicle"))
			{
				return;
			}
			
			var colRigidbody = col.GetComponent<Rigidbody>();
			Debug.Log(Vector3.Dot(colRigidbody.velocity, transform.position));
			
			if (Vector3.Dot(colRigidbody.velocity, transform.position) < 1000) return;

			var force = colRigidbody.velocity.magnitude;
			Vector3 dir = collision.contacts[0].point;
			rb.AddForce(dir.normalized*force);
			DoRagdoll(true);
		}
	}
}
