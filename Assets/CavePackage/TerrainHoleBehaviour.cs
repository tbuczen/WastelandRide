using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHoleBehaviour : MonoBehaviour {

    [SerializeField]
    Terrain terrain;
    
	void Start () {}
	void Update () {}

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Player")
        {
            Physics.IgnoreCollision(c, terrain.GetComponent<Collider>(), true);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag == "Player")
        {
            Physics.IgnoreCollision(c, terrain.GetComponent<Collider>(), false);
        }
    }
}
