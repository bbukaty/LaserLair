using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRobot: MonoBehaviour {

	private LevelManager levelManager;
	public int[] levelPosition;
	
	void Start() {
		// find the level manager object in the scene to get level data from
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Left")) {
			movePlayer(new int[3] {-1, 0, 0});
		}
		if (Input.GetButtonDown ("Right")) {
			movePlayer(new int[3] {1, 0, 0});
		}
		if (Input.GetButtonDown ("Up")) {
			movePlayer(new int[3] {0, 0, 1});
		}
		if (Input.GetButtonDown ("Down")) {
			movePlayer(new int[3] {0, 0, -1});
		}
	}

	bool modelIsMoving() {
		return transform.GetComponentInChildren<Rigidbody>().velocity.magnitude != 0;
	}

	bool movePlayer(int[] movement) {
		if (modelIsMoving()) {
			return false;
		}
		Debug.Log("trying to move: " + movement[0].ToString() + movement[1].ToString() + movement[2].ToString());
		int[] newPos = new int[3];
		for (int i = 0; i < 3; i++) {
			newPos[i] = levelPosition[i] + movement[i];
		}
		if (!levelManager.isInBounds(newPos)) {
			return false;
		}
		bool canMove = false;
		Transform occupant = levelManager.getBlockIn(newPos);
		//bool canPushOccupant = true; TODO: get this from occupant
		if (occupant == null || levelManager.tryPush(newPos, movement)) {
			/* maybe check if there's solid ground there first
			int[] belowNewPos = (int[])newPos.Clone();
			belowNewPos[1] -= 1;
			if (levelManager.isInBounds(belowNewPos) && levelManager.getBlockIn(belowNewPos) is Transform) {
				levelPosition = newPos;
				transform.Translate (new Vector3 (x, y, z));
			}
			*/
			canMove = true;
		} 
		/* else if (levelManager.tryPush()){
			occupant.Translate(new Vector3(x, y, z));
			//TODO: this doesn't change where the blocks are in the data, just visually.
			levelPosition = newPos;
			transform.Translate(new Vector3 (x, y, z));
		}
		*/
		if (canMove) {
			levelPosition = newPos;
			transform.Translate(new Vector3 (movement[0], movement[1], movement[2]));
		}
		return false;
	}
}
