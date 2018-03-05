using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRobot: MonoBehaviour {

	private LevelManager levelManager;
	private intTrio levelPos;
	private intTrio orientation;
	public Transform corpsePrefab;

	public void initLevelPos(intTrio startPos) {
		levelPos = startPos;
	}

    void Start() {
		// find the level manager object in the scene to get level data from
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Left")) {
			movePlayer(new intTrio(-1, 0, 0));
		}
		if (Input.GetButtonDown ("Right")) {
			movePlayer(new intTrio(1, 0, 0));
		}
		if (Input.GetButtonDown ("Up")) {
			movePlayer(new intTrio(0, 0, 1));
		}
		if (Input.GetButtonDown ("Down")) {
			movePlayer(new intTrio(0, 0, -1));
		}
	}

	bool modelIsMoving() {
		Rigidbody body = transform.GetComponentInChildren<Rigidbody>();
		return body.angularVelocity.magnitude == 0 && body.velocity.magnitude != 0;
	}

	void moveModel(intTrio movement) {
		transform.Translate(new Vector3 (movement.x, movement.y, movement.z));
	}

	bool applyInput(intTrio movement) {
		if (modelIsMoving()) {
			return false;
		}
		// if movement direction doesn't equal orientation
		if (movement != levelPos) {
			return rotatePlayer(movement);
		}
		return movePlayer(movement);
	}

	bool movePlayer(intTrio movement) {
		Debug.Log("moving: " + movement.ToString());
		intTrio newPos = levelPos + movement;
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
			Debug.Log("Robot died in laser");
			Transform corpse = Instantiate(corpsePrefab, new Vector3(levelPos[0], levelPos[1], levelPos[2]), Quaternion.identity, levelManager.transform);
			levelManager.addBlock(levelPos, corpse.GetComponent<Block>());
			Destroy(gameObject);
		}
	}

	private bool rotatePlayer(intTrio movement) {

		return true;
	}
}
