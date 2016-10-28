using UnityEngine;
using System.Collections;

[RequireComponent (typeof(SphereCollider))][RequireComponent (typeof(Rigidbody))]
public class Waypoint : MonoBehaviour
{

	void Start ()
	{
		gameObject.GetComponent<SphereCollider> ().isTrigger = true;
		gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		gameObject.name = "Waypoint";
	}




	void OnTriggerEnter (Collider collider)
	{
		if (collider.gameObject.layer == 9)
			Destroy (gameObject);
	}
}
