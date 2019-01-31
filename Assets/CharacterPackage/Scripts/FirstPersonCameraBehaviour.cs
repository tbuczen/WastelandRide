//using UnityEngine;
//using System.Collections;
//
//public class FirstPersonCameraBehaviour : MonoBehaviour {
//
//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//        //Camera mycam = GetComponent<Camera>();
//       // transform.LookAt(mycam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mycam.nearClipPlane)), Vector3.up);
//
//        Camera mycam = GetComponent<Camera>();
//
//        float sensitivity = 0.05f;
//        Vector3 vp = mycam.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mycam.nearClipPlane));
//        vp.x -= 0.5f;
//        vp.y -= 0.5f;
//        vp.x *= sensitivity;
//        vp.y *= sensitivity;
//        vp.x += 0.5f;
//        vp.y += 0.5f;
//        Vector3 sp = mycam.ViewportToScreenPoint(vp);
//
//        Vector3 v = mycam.ScreenToWorldPoint(sp);
//        transform.LookAt(v, Vector3.up);
//    }
//}
