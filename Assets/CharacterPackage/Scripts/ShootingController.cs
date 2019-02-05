using UnityEngine;

namespace CharacterPackage.Scripts
{
	public class ShootingController : MonoBehaviour {
	
		// Update is called once per frame
		private void Update () {
			if (Input.GetMouseButtonDown(0))
			{

				Camera activeCam = null;
				foreach (var cam in Camera.allCameras)
				{
					if (cam.isActiveAndEnabled)
					{
						activeCam = cam;
					}
				}
				var ray = activeCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
				var hits = Physics.RaycastAll(ray);
				foreach (var hit in hits)
				{
					var targetRagdoll = hit.collider.GetComponent<HumanoidRagdollBehaviour>();
					if (targetRagdoll != null)
					{
					
						var targetRb = targetRagdoll.GetRb();
						targetRb.AddForce(ray.direction*5/10);
						targetRagdoll.DoRagdoll(true);
					}
				}
			}
		}
	}
}
