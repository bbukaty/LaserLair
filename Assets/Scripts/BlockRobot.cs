using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRobot: MonoBehaviour {

	private LevelManager levelManager;
	public int[] levelPos;

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

	void moveModel(int[] movement) {
		transform.Translate(new Vector3 (movement[0], movement[1], movement[2]));
	}

	bool movePlayer(int[] movement) {
		if (modelIsMoving()) {
			return false;
		}
		Debug.Log("trying to move: " + movement[0].ToString() + movement[1].ToString() + movement[2].ToString());
		int[] newPos = new int[3] {levelPos[0]+movement[0], levelPos[1]+movement[1], levelPos[2]+movement[2]};
		if (!levelManager.isInBounds(newPos)) {
			return false;
		}
		Block occupant = levelManager.getBlockIn(newPos);
		//bool canPushOccupant = true; TODO: get this from occupant
		if (occupant == null || levelManager.tryPush(newPos, movement)) {
			/* maybe check if there's solid ground there first
			int[] belowNewPos = (int[])newPos.Clone();
			if (levelManager.getBlockIn(belowNewPos) != null) {
				// move
			}
			*/
			levelPos = newPos;
			moveModel(movement);
			getMoveConsequences();
			return true;
		} 
		return false;
	}

	private void getMoveConsequences() {
		if (levelManager.isInLaser(levelPos)) {
			// kill robot and leave block
		}
	}
}
