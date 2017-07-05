using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour {

	public GameObject selectedObject;
	public static MouseManager Instance;
	Color oldColor;
	// Use this for initialization
	void Start () {
		Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition);
		RaycastHit hitInfo;

		if (Physics.Raycast( ray, out hitInfo)){
			GameObject hitObject = hitInfo.transform.root.gameObject;

			SelectObject(hitObject);
		}
		else{
			ClearSelection();
		}
	}
	void SelectObject(GameObject obj){
		if (selectedObject != null){
			if (obj == selectedObject)
				return;

			ClearSelection ();
		}
		selectedObject = obj;

		HighlightObject (selectedObject, true);
	}
	void ClearSelection(){
		if (selectedObject != null) {
			HighlightObject (selectedObject, false);
			selectedObject = null;
		}
	}
	void HighlightObject(GameObject obj, bool set) {
		cakeslice.Outline outline = obj.GetComponent<cakeslice.Outline> ();
		if (outline && outline.color == 0)
			outline.enabled = set;
	}
}
