using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRobot: MonoBehaviour {

	private LevelManager levelManager;
	private Vector3Int levelPos;
	private Vector3Int orientation;
	public Transform corpsePrefab;

	public void initLevelPos(Vector3Int startPos) {
		levelPos = startPos;
	}

    void Start() {
		// find the level manager object in the scene to get level data from
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Left")) {
			movePlayer(new Vector3Int(-1, 0, 0));
		}
		if (Input.GetButtonDown ("Right")) {
			movePlayer(new Vector3Int(1, 0, 0));
		}
		if (Input.GetButtonDown ("Up")) {
			movePlayer(new Vector3Int(0, 0, 1));
		}
		if (Input.GetButtonDown ("Down")) {
			movePlayer(new Vector3Int(0, 0, -1));
		}
	}

	bool modelIsMoving() {
		Rigidbody body = transform.GetComponentInChildren<Rigidbody>();
		return body.angularVelocity.magnitude == 0 && body.velocity.magnitude != 0;
	}

	void moveModel(Vector3Int movement) {
		transform.Translate(movement);
	}

	bool applyInput(Vector3Int movement) {
		if (modelIsMoving()) {
			return false;
		}
		// if movement direction doesn't equal orientation
		if (movement != levelPos) {
			return rotatePlayer(movement);
		}
		return movePlayer(movement);
	}

	bool movePlayer(Vector3Int movement) {
		Debug.Log("moving: " + movement.ToString());
		Vector3Int newPos = levelPos + movement;
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
			Transform corpse = Instantiate(corpsePrefab, levelPos, Quaternion.identity, levelManager.transform);
			levelManager.addBlock(levelPos, corpse.GetComponent<Block>());
			Destroy(gameObject);
		}
	}

	private bool rotatePlayer(Vector3Int movement) {

		return true;
	}
	
}
