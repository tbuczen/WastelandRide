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
			Debug.Log("doing ragdoll");

			foreach (var col in allColliders)
			{
				col.enabled = isRagdoll;
			}

			mainCollider.enabled = !isRagdoll;
			rb.useGravity = !isRagdoll;
			anim.enabled = !isRagdoll;
		}


		private void OnTriggerEnter(Collider other)
		{
			throw new System.NotImplementedException();
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
			// create a hit and see how close the actual hit point is.
			
			Debug.DrawRay(transform.position, col.transform.position, Color.green,2);
			Debug.Log("Car hit");
			
			RaycastHit hit;
			if (!Physics.Linecast(transform.position, col.transform.position, out hit)) return;
			Debug.Log("Linecast hit");
			// are we less than 1 frame from hitting the person?
				
			Debug.Log(colRigidbody.velocity.magnitude);
			Debug.Log(Vector3.Distance(transform.position, hit.point));

			if(colRigidbody.velocity.magnitude * Time.deltaTime / Vector3.Distance(transform.position, hit.point)<1){
				//We are about to hit...
				DoRagdoll(true);
				transform.position += new Vector3(0f, 0.05f, 0f);
//                    Rigidbody bone = FallDown ();
//                    bone.AddForce((colRigidbody.velocity.normalized * fallForce) + new Vector3(0f, fallForce, 0f), ForceMode.Impulse);
                    
			}
		}
        
		Rigidbody FallDown()
		{
			anim.enabled = false;		
//            GameObject rag = (GameObject)Instantiate(ragdoll, transform.position, transform.rotation);
//            transform.position = new Vector3(0, -300, 0);
//            Rigidbody bone = rag.GetComponentInChildren<Rigidbody>();
//            m_Camera.FocusOn(bone);
			//timeToGetUp = Time.time + timeToStayDown;		
//            return bone;
			return null;
		}
	}
}
