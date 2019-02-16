namespace VehiclePackage
{
		
	using UnityEngine;
	using System.Collections;
	
	public class DamageableBehaviour : MonoBehaviour 
	{
		public float maxMoveDelta = 1.0f; // maximum distance one vertice moves per explosion (in meters)
		public float maxCollisionStrength = 50.0f;
		[Range(0f,1.0f)]
		public float yForceDamp = 0.1f; // 0.0 - 1.0
		public float demolutionRange = 0.5f;
		public float impactDirManipulator = 0.0f;
		public MeshFilter[] optionalMeshList;
		
		private MeshFilter[] meshFilters;
		private float sqrDemRange;
	
		public void Awake()
		{
			//if list > 0 get list if not find mesh filter component
			meshFilters = optionalMeshList.Length>0 ? optionalMeshList : GetComponentsInChildren<MeshFilter>();
			Debug.Log(optionalMeshList.Length>0);
			Debug.Log(meshFilters);
			sqrDemRange = demolutionRange*demolutionRange;
		}
		
		public void OnCollisionEnter( Collision collision ) 
		{
			var col = collision.collider;
			Debug.Log(col.tag);
			if (col.CompareTag("Enemy")) return;
			if (col.CompareTag("Player")) return;
	
			Vector3 colRelVel = collision.relativeVelocity;
			colRelVel.y *= yForceDamp;
			Vector3 colPointToMe = transform.position - collision.contacts[0].point;
	
//			Debug.Log("Hit");
			// Dot = angle to collision point, frontal = highest damage, strip = lowest damage
			float colStrength = colRelVel.magnitude * Vector3.Dot(collision.contacts[0].normal, colPointToMe.normalized);
	
//			Debug.Log("Hit Strength:" + colStrength);
			
			OnMeshForce( collision.contacts[0].point, Mathf.Clamp01(colStrength/maxCollisionStrength) );
		}
		
//		 if called by SendMessage(), we only have 1 param
		public void OnMeshForce( Vector4 originPosAndForce )
		{
			OnMeshForce( (Vector3)originPosAndForce, originPosAndForce.w );
		}

		private void OnMeshForce( Vector3 originPos, float force )
		{
			// force should be between 0.0 and 1.0
			force = Mathf.Clamp01(force);
				
			foreach (var t in meshFilters)
			{
				Vector3 [] verts = t.mesh.vertices;
				for (int i=0;i<verts.Length;++i)
				{
					Vector3 scaledVert = Vector3.Scale( verts[i], transform.localScale );						
					Vector3 vertWorldPos = t.transform.position + (t.transform.rotation * scaledVert);
					Vector3 originToMeDir = vertWorldPos - originPos;
					Vector3 flatVertToCenterDir = transform.position - vertWorldPos;
					flatVertToCenterDir.y = 0.0f;
					
					// 0.5 - 1 => 45° to 0°  / current vertice is nearer to exploPos than center of bounds
					if (!(originToMeDir.sqrMagnitude < sqrDemRange)) continue;
					
					float dist = Mathf.Clamp01(originToMeDir.sqrMagnitude/sqrDemRange);
					float moveDelta = force * (1.0f-dist) * maxMoveDelta;
					Vector3 moveDir = Vector3.Slerp(originToMeDir, flatVertToCenterDir, impactDirManipulator).normalized * moveDelta;
					verts[i] += Quaternion.Inverse(transform.rotation)*moveDir;
				}
		   
				t.mesh.vertices = verts;
				t.mesh.RecalculateBounds();
			}
		} 
	}

}