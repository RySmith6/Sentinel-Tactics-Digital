using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieRolling : MonoBehaviour {

	// Use this for initialization
	private Rigidbody rb;

	void Start ()
	{
		rb = GetComponent<Rigidbody>();
		rb.AddTorque (new Vector3 (Random.value, Random.value, Random.value), ForceMode.Impulse);
		Physics.gravity = (new Vector3 (0.0f, 0.0f, 9.8f));
	}

	void FixedUpdate ()
	{
		if (Input.GetMouseButtonDown (0)) {
			rb.AddForce (new Vector3 (Random.value - 0.5f, Random.value - 0.5f, -0.5f)*20, ForceMode.Impulse);
			Roll ();
		}

	}


	void Roll()
	{
		rb.AddTorque (new Vector3 (Random.value, Random.value, Random.value)*10, ForceMode.Impulse);
	}

	int GetDiceCount()
	{
		if (Vector3.Dot (transform.forward, Vector3.up) > 1)
			return 1;
		if (Vector3.Dot (-transform.forward, Vector3.up) > 1)
			return 2;
		if (Vector3.Dot (transform.up, Vector3.up) > 1)
			return 3;
		if (Vector3.Dot (-transform.up, Vector3.up) > 1)
			return 4;
		if (Vector3.Dot (transform.right, Vector3.up) > 1)
			return 5;
		if (Vector3.Dot (-transform.right, Vector3.up) > 1)
			return 6;
		return 1;
	}

}
