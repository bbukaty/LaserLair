using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot: CubeObject {

	public Transform corpsePrefab;

    protected LevelManager levelManager;
	protected bool isGrabbing;

    bool modelIsMoving() {
		Rigidbody body = transform.GetComponentInChildren<Rigidbody>();
		return body.angularVelocity.magnitude == 0 && body.velocity.magnitude != 0;
	}

	void Awake() {
        // find the level manager object in the scene to get level data from
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
		Debug.Assert(levelManager != null, "Warning: Level Manager script not found in scene!");
		isGrabbing = false;
        initOrientation();
		initLevelPos();
	}

    // Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Left")) {
			applyInput(new Vector3Int(-1, 0, 0));
		} else if (Input.GetButtonDown ("Right")) {
			applyInput(new Vector3Int(1, 0, 0));
		} else if (Input.GetButtonDown ("Up")) {
			applyInput(new Vector3Int(0, 0, 1));
		} else if (Input.GetButtonDown ("Down")) {
			applyInput(new Vector3Int(0, 0, -1));
		} else if (Input.GetButtonDown("Grab") && !modelIsMoving()) {
			tryGrab();
		}
	}

    private void applyInput(Vector3Int movement) {
		if (modelIsMoving()) {
			return;
		}
		if (movement == orientation) {
			movePlayer(movement);
		} else if (movement == orientation * -1) {
			bool moved = movePlayer(movement); 
			if (isGrabbing && moved) {
				// pull grabbed block backwards, *2 because we just moved 1 more away from the block to pull
				isGrabbing = levelManager.tryPush(levelPos + orientation*2, movement);
				// set isGrabbing to whether could push, so that if you try to pull heavy you stop grabbing
			}
		} else { // movement axis doesn't align with current orientation
			// drop grabbed block
			isGrabbing = false;
			rotateModelTo(movement);
		}
	}

	private bool movePlayer(Vector3Int movement) {
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
		} else {
			// TODO: display "can't move" animation
			return false;
		}
	}

    protected virtual void getMoveConsequences() {
        return;
    }

	protected virtual void tryGrab() {
		return;
	}
}
