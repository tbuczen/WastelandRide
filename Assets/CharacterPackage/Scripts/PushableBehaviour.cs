using UnityEngine;

namespace CharacterPackage.Scripts
{
    public class PushableBehaviour : MonoBehaviour {

        public float pushStrength = 9.0f;

        private UserController uc;

        void OnControllerColliderHit(ControllerColliderHit hit) {
            Rigidbody rb = hit.collider.attachedRigidbody;
            Debug.Log(hit.moveDirection.y);
            if (rb == null || rb.isKinematic || hit.moveDirection.y < -0.1f) {
                return;
            }
            //dont move the rigidbody if the character is on top of it
            /*if (m_CollisionFlags == CollisionFlags.Below)
        {
            return;
        }*/

            pushStrength = uc.GetSpeed();

            Vector3 direction = new Vector3 (hit.moveDirection.x, 0, hit.moveDirection.z);

            rb.velocity = direction * pushStrength;
        }


        void Start() {
            uc = gameObject.GetComponent<UserController>();
        }
    }
}
