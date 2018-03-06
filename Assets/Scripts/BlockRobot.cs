using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRobot: MonoBehaviour {

	private LevelManager levelManager;
	private Vector3Int levelPos;
	public Vector3Int orientation;
	public Transform corpsePrefab;

	public void initLevelPos(Vector3Int startPos) {
		levelPos = startPos;
	}

    void Start() {
		// find the level manager object in the scene to get level data from
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
		orientation = new Vector3Int();
        for (int i = 0; i < 3; i++) {
            Debug.Assert(Mathf.Abs((float)(int)transform.right[i] - transform.right[i]) < 0.001, "Warning: Level contains improperly oriented cube!");
            orientation[i] = (int)transform.right[i];
        }
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Left")) {
			applyInput(new Vector3Int(-1, 0, 0));
		}
		if (Input.GetButtonDown ("Right")) {
			applyInput(new Vector3Int(1, 0, 0));
		}
		if (Input.GetButtonDown ("Up")) {
			applyInput(new Vector3Int(0, 0, 1));
		}
		if (Input.GetButtonDown ("Down")) {
			applyInput(new Vector3Int(0, 0, -1));
		}
	}

	bool modelIsMoving() {
		Rigidbody body = transform.GetComponentInChildren<Rigidbody>();
		return body.angularVelocity.magnitude == 0 && body.velocity.magnitude != 0;
	}

	void moveModel(Vector3Int movement) {
		transform.Translate(movement, Space.World);
	}

	bool applyInput(Vector3Int movement) {
		if (modelIsMoving()) {
			return false;
		}
		// if movement axis doesn't align with current orientation
		if (movement != orientation && movement != orientation * -1) {
			Debug.Log("rotating");
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
		transform.right = movement;
		orientation = movement;
		return true;
	}

}
