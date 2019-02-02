using System.Collections;
using System.Collections.Generic;
using CharacterPackage.Scripts;
using UnityEngine;

public class ShootingController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));

			var hits = Physics.RaycastAll(ray);
			foreach (var hit in hits)
			{
				
				var targetRagdoll = hit.collider.GetComponent<HumanoidRagdollBehaviour>();
				if (targetRagdoll != null)
				{
					targetRagdoll.DoRagdoll(true);
				}
			}
		}
	}
}
