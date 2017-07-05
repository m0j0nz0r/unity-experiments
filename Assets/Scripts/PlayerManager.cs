using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace testGame{
	public class PlayerManager : Photon.PunBehaviour {

		#region Character Stats

		public int BOD;
		public int AGI;
		public int REA;
		public int STR;
		public int CHA;
		public int INT;
		public int LOG;
		public int WIL;
		public int EDG;
		public int MAG;
		public int RES;
		public int DEP;
		public int AugmentedBOD {
			get {
				return BOD;
			}
		}
		public int AugmentedAGI {
			get {
				return AGI;
			}
		}
		public int AugmentedREA {
			get {
				return REA;
			}
		}
		public int AugmentedSTR {
			get {
				return STR;
			}
		}
		public int AugmentedCHA {
			get {
				return CHA;
			}
		}
		public int AugmentedINT {
			get {
				return INT;
			}
		}
		public int AugmentedLOG {
			get {
				return LOG;
			}
		}
		public int AugmentedWIL {
			get {
				return WIL;
			}
		}
		public int AugmentedEDG {
			get {
				return EDG;
			}
		}
		public int AugmentedMAG {
			get {
				return MAG;
			}
		}
		public int AugmentedRES {
			get {
				return RES;
			}
		}
		public int AugmentedDEP {
			get {
				return DEP;
			}
		}

		public int Initiative{
			get{
			return AugmentedINT + AugmentedREA + sumRoll (1); 
			}
		}
		public int AstralInitiative {
			get {
			return AugmentedINT * 2 + sumRoll(2);
			}
		}
		public int MatrixARInitiative {
			get { 
			return AugmentedINT + AugmentedREA + sumRoll (1); 
			}
		}
		public int MatrixVRInitiative;
		public int MentalLimit {
			get {
			return Mathf.CeilToInt ((AugmentedLOG * 2 + AugmentedINT + AugmentedWIL) / 3);
			}
		}
		public int PhysicalLimit {
			get {
			return Mathf.CeilToInt ((AugmentedSTR * 2 + AugmentedBOD + AugmentedREA) / 3);
			}
		}
		public int SocialLimit {
			get {
			return Mathf.CeilToInt ((AugmentedCHA * 2 + AugmentedWIL + Essence) / 3);
			}
		}
		public float Essence;
		public int ConditionMonitorPhysical{
			get{
			return Mathf.CeilToInt (AugmentedBOD / 2) + 8;
			}
		}
		public int ConditionMonitorStun {
			get {
				//int bonuses = 0;
			return Mathf.CeilToInt (AugmentedWIL / 2) + 8;
			}
		}
		public int ConditionMonitorOverflow{
			get{ 
				//int bonuses = 0;
			return AugmentedBOD;
			}
		}
		public int physicalWounds = 0;
		public int stunWounds = 0;
		public int Armor{
			get{
				return 13;
			}
		}
		private int sumRoll(int d){
			int retval = 0;
			while (d > 0) {
				retval += Mathf.RoundToInt (Random.value * 6 + 1);
				d--;
			}
			return retval;
		}
		private int successRoll(int d){
			int retval = 0;
			while (d > 0) {
				retval += (sumRoll (1) >= 5) ? 1 : 0;
				d--;
			}
			return retval;
		}

				
		#endregion

		void Start(){
			loadCharacter ();
		}
		void Update(){
			getInput ();
		}
		void getInput(){
			if (Input.GetMouseButton (0)) {
				if (MouseManager.Instance != null) {
					GameObject obj = MouseManager.Instance.selectedObject;
					if (obj != null && obj.GetComponent<Targetable> () != null) {
						//cakeslice.Outline outline;
						if (target != null) {
							setTarget (false);
						}
						target = obj;
						setTarget (true);
					}
				} else {
					MouseManager.Instance = FindObjectOfType<MouseManager> ();
				}
			}
		}
		void setTarget(bool set){
			cakeslice.Outline outline = target.GetComponent<cakeslice.Outline> ();
			if (outline != null) {
				outline.color = set ? 1 : 0;
				outline.enabled = set;
			}
		}
		void updateHud(){
			
		}

		void loadCharacter(){
			BOD = 7;
			AGI = 6;
			REA = 5;
			STR = 5;
			WIL = 3;
			LOG = 2;
			INT = 3;
			CHA = 2;
			EDG = 1;
			Essence = 0.88f;
		}

		#region interactions
		public GameObject target;

		public int takeDamage(int rawDamage, int AP, bool physical){
			int modifiedArmor = Mathf.Max(Armor + AP, 0);

			physical = physical && (rawDamage > modifiedArmor);

			int retval = Mathf.Max(rawDamage - successRoll (modifiedArmor + AugmentedBOD), 0);

			if (physical) {
				physicalWounds += retval;
			} else {
				int maxStun = ConditionMonitorStun;
				stunWounds += retval;
				if (stunWounds > maxStun) {
					physicalWounds += stunWounds - maxStun;
					stunWounds = maxStun;
				}
			}

			return retval;
		}
		#endregion
		public void testButtonClicked(){
			takeDamage (8, -5, true);
		}
	}
}
