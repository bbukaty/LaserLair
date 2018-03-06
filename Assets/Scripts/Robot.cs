using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot: CubeObject {

    protected LevelManager levelManager;
    protected Vector3Int levelPos;
	public Transform corpsePrefab;

    public void setLevelPos(Vector3Int pos) {
		levelPos = pos;
	}

    bool modelIsMoving() {
		Rigidbody body = transform.GetComponentInChildren<Rigidbody>();
		return body.angularVelocity.magnitude == 0 && body.velocity.magnitude != 0;
	}

    void Start() {
        // find the level manager object in the scene to get level data from
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        initOrientation();
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

    private void applyInput(Vector3Int movement) {
		if (modelIsMoving()) {
			return;
		}
		// if movement axis doesn't align with current orientation
		if (movement != orientation && movement != orientation * -1) {
			rotateModelTo(movement);
		} else {
			movePlayer(movement);
		}
	}

	private void movePlayer(Vector3Int movement) {
		Debug.Log("moving: " + movement.ToString());
		Vector3Int newPos = levelPos + movement;
		if (!levelManager.isInBounds(newPos)) {
			return;
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
		} else {
			// TODO: display "can't move" animation
			return;
		}
	}

    protected virtual void getMoveConsequences() {
        return;
    }

}
