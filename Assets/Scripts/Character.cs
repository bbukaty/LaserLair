using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character: CubeObject {

	public Transform corpsePrefab;
	public bool canGrab;

    protected LevelManager levelManager;
	protected bool isGrabbing;

    bool modelIsMoving() {
		Rigidbody body = transform.GetComponent<Rigidbody>();
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
		} else if (Input.GetButtonDown("Grab") && canGrab && !modelIsMoving()) {
			toggleGrab();
		}
	}

	private void toggleGrab() {
		if (isGrabbing) {
			isGrabbing = false;
		} else if (levelManager.getBlockIn(levelPos + orientation) != null) {
			// toggle grab animation
			isGrabbing = true;
		}
	}

    private void applyInput(Vector3Int movement) {
		if (modelIsMoving()) {
			return;
		}
		Debug.Log("moving: " + movement.ToString());
		if (movement == orientation || movement == orientation * -1) {
			if (isGrabbing) {
				Vector3Int grabBlockPos = levelPos + orientation;
				if (tryMove(movement, justChecking: true) && levelManager.tryPush(grabBlockPos, movement)) {
					tryMove(movement);
				}
			} else {
				Debug.Log("levelPos:" + levelPos.ToString());
				Debug.Log("blockIn pos + mov: " + levelManager.getBlockIn(levelPos + movement));
				if (levelManager.getBlockIn(levelPos + movement) == null) {
					tryMove(movement);
				} else {
					tryJump(movement);
				}
			}
		} else { // movement axis doesn't align with current orientation
			// drop grabbed block
			isGrabbing = false;
			rotateModelTo(movement);
		}
	}

	private bool tryMove(Vector3Int movement, bool justChecking = false) {
		Vector3Int newPos = levelPos + movement;
		if (!levelManager.isInBounds(newPos)) {
			return false;
		}
		Block occupant = levelManager.getBlockIn(newPos);
		if (occupant == null || levelManager.tryPush(newPos, movement, justChecking)) {
			if (!justChecking) {
				levelPos = newPos;
				moveModel(movement);
				getMoveConsequences();
			}
			return true;
		} else {
			// TODO: display "can't move" animation
			return false;
		}
	}

    protected virtual void getMoveConsequences() {
        if (levelManager.isInLaser(levelPos)) {
			die();
		}
		tryFall();
    }
	
	protected virtual void tryJump(Vector3Int movement) {
		if (levelManager.getBlockIn(levelPos + movement + Vector3Int.up) == null) {
			levelPos += movement + Vector3Int.up;
			moveModel(movement + Vector3Int.up);
			getMoveConsequences();
		}
	}

	protected virtual void die() {
		return;
	}

	private void tryFall() {
		Vector3Int below = levelPos + Vector3Int.down;
		if (!levelManager.isInBounds(below)) {
			// fall out of level, remove character component while falling so they can't move
			Destroy(this);
			Destroy(gameObject, 2);
			Rigidbody body = GetComponent<Rigidbody>();
			body.useGravity = true;
			body.isKinematic = false;
		} else if (levelManager.getBlockIn(below) == null) {
			levelPos = below;
			moveModel(Vector3Int.down);
			getMoveConsequences();
		}
	}
}
