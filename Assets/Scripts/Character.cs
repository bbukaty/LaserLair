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
		CubeObject facingBlock = levelManager.getCubeObjIn(levelPos + orientation);
		if (movement == orientation) {
			if (isGrabbing) {
				tryMove(movement);
			} else if (facingBlock != null) {
				tryJump(movement);
			} else {
				tryMove(movement);
			}
		} else if (movement == orientation * -1) {
			//can't jump up backwards
			if (isGrabbing) {
				Debug.Log("pulling block backwards");
				facingBlock.tryMove(movement);
			} else {
				tryMove(movement);
			}
		} else { // movement axis doesn't align with current orientation
			// drop grabbed block
			isGrabbing = false;
			updateOrientation(movement);
		}
	}
	
	protected virtual void tryJump(Vector3Int movement) {
		Vector3Int newPos = levelPos + movement + Vector3Int.up;
		if (levelManager.getCubeObjIn(newPos) == null) {
			updatePos(movement + Vector3Int.up);
			getMoveConsequences();
		}
	}
}
