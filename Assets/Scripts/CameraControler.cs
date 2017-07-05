using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour {

	public float minDistance;
	public float maxDistance;
	public float turnSpeed;
	public float zoomSpeed;
	public Transform target;
	public float currentDistance;
	float distance = 5f;
	float elevation = 2.5f;
	float verticalModifier = -0.25f;

	void Start () {
		
	}
	
	void Update () {
		Vector3 temp;
		if (Input.GetMouseButton (1)) {
			transform.RotateAround (target.position, Vector3.up, turnSpeed * Input.GetAxis("Mouse X")*Time.deltaTime);
			transform.Translate (Vector3.up * Input.GetAxis ("Mouse Y") * Time.deltaTime * turnSpeed * verticalModifier);
			temp = transform.position;
			elevation = temp.y;
			elevation = Mathf.Clamp (elevation, 1, distance - 1);
		}

		distance = distance - Input.GetAxis ("Mouse ScrollWheel") * zoomSpeed*Time.deltaTime;
		distance = Mathf.Clamp (distance, minDistance, maxDistance);

		//Follow target
		currentDistance = Vector3.Distance(transform.position, target.position);


		temp = transform.position;
		temp.y = elevation;
		transform.position = temp;

		Vector3 direction = (transform.position - target.position).normalized;
		transform.position = target.position + direction * distance;

		//Keep looking at target
		transform.LookAt (target);
	}
}
