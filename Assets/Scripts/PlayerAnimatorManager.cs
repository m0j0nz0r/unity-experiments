using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace testGame{
	[RequireComponent(typeof(Animator))]
	public class PlayerAnimatorManager : MonoBehaviour {
		#region Public Variables

		public float vel;
		#endregion

		#region Private Variables

		private Animator animator;
		private Vector3 newPosition;
		private int forwardAnimationHash;
		private int turnAnimationHash;
		private NavMeshAgent agent;
		//private Vector2 smoothDeltaPosition;
		//private Vector3 velocity;
		private Quaternion lastRotation;
		#endregion


		#region MonoBerhaviour Messages

		void Start () {
			animator = GetComponent<Animator> ();
			newPosition = transform.position;
			forwardAnimationHash = Animator.StringToHash ("Speed");
			turnAnimationHash = Animator.StringToHash ("Direction");
			agent = GetComponent<NavMeshAgent> ();
			agent.updatePosition = false;
			//smoothDeltaPosition = Vector2.zero;
			//velocity = Vector3.zero;
			lastRotation = transform.rotation;
		}

		void Update(){
			getInput ();
			doMovement ();
		}

		#endregion

		void getInput(){
			if (Input.GetMouseButton (0)) {
				RaycastHit hit;
				Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (mouseRay, out hit) && hit.transform.tag == "Ground") {
					newPosition = hit.point;
				}
			}
		}
		void doMovement(){
			agent.destination = newPosition;

			animator.SetFloat (forwardAnimationHash, agent.velocity.magnitude);

			Quaternion deltaRotation = transform.rotation * Quaternion.Inverse (lastRotation);

			Vector3 eulerRotation = new Vector3 (Mathf.DeltaAngle (0, deltaRotation.eulerAngles.x), Mathf.DeltaAngle (0, deltaRotation.eulerAngles.y), Mathf.DeltaAngle (0, deltaRotation.eulerAngles.z));

			vel = Mathf.Round(eulerRotation.y*Time.deltaTime*100);

			animator.SetFloat (turnAnimationHash, vel);

			lastRotation = transform.rotation;
			transform.position = agent.nextPosition;
		}
	}
}
