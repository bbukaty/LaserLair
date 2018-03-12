using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character: CubeObject {

	public Transform corpsePrefab;
	public bool canGrab;

	protected bool isGrabbing;

	protected override void initStateVars() {
		isGrabbing = false;
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
		} else if (levelManager.getCubeObjIn(levelPos + orientation) != null) {
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
				CubeObject grabbedBlock = levelManager.getCubeObjIn(levelPos + orientation);
				if (tryMove(movement, justChecking: true) && grabbedBlock.tryMove(movement)) {
					tryMove(movement);
				}
			} else {
				if (levelManager.getCubeObjIn(levelPos + movement) == null) {
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
	
	protected virtual void tryJump(Vector3Int movement) {
		if (levelManager.getCubeObjIn(levelPos + movement + Vector3Int.up) == null) {
			levelPos += movement + Vector3Int.up;
			moveModel(movement + Vector3Int.up);
			getMoveConsequences();
		}
	}
}
