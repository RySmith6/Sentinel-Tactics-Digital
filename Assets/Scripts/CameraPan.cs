using UnityEngine;
using System.Collections;

public class CameraPan : MonoBehaviour {
	Camera thisCamera;
	public float smooth = 0.5f;
	private Vector3 pointyUp;
	private bool flatTop=true;
	// Use this for initialization
	void Start () {
		thisCamera = this.GetComponent<Camera> ();
		pointyUp = new Vector3 (-Mathf.Sin (0.523599f), Mathf.Cos (0.523599f)); //(Mathf.Sqrt (3) / 2.0f));

	}
	
	// Update is called once per frame
	void Update () {
		if (!ManageInputs.showMenu) {
			float moveHorizontal = Input.GetAxis ("Horizontal");
			float moveVertical = Input.GetAxis ("Vertical");
			Vector3 tempVector = this.transform.position;
			if (((moveHorizontal > 0.1f || moveHorizontal < -0.1f) || (moveVertical > 0.1f || moveVertical < -0.1f))) {
				tempVector.x += -20 * moveHorizontal;
				tempVector.y += 20 * moveVertical;
				if (tempVector.x > 15)
					tempVector.x = 15;
				if (tempVector.x < -15)
					tempVector.x = -15;
				if (tempVector.y > 15)
					tempVector.y = 15;
				if (tempVector.y < -15)
					tempVector.y = -15;
				this.transform.position = Vector3.Lerp (this.transform.position, tempVector, smooth * Time.deltaTime);
			}
			else if(Input.GetKeyDown(KeyCode.Q))
			{
				flatTop = !flatTop;

			}
			HandleRotation();
		}
	}

	void HandleRotation()
	{
		Vector3 relativePlayerPosition = this.transform.position;
		relativePlayerPosition.z = -relativePlayerPosition.z;
		relativePlayerPosition.y = 0;
		Quaternion lookAtRotation;
		if (flatTop)
			lookAtRotation = Quaternion.LookRotation (Vector3.forward, Vector3.up);
		else
			lookAtRotation = Quaternion.LookRotation (Vector3.forward, pointyUp);
		this.transform.rotation = Quaternion.Lerp (this.transform.rotation, lookAtRotation, smooth * Time.deltaTime);
	}
}
