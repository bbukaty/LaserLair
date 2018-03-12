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
			// if (grabbingBlock != null && !isGrabbing) {
			// 	tryJump(movement);
			// } else {
			// 	tryMove(movement);
			// }
			if (isGrabbing) {
				tryMove(movement);
			} else if (facingBlock != null) {
				Debug.Log("block in front, not grabbing - tryJump");
			} else {
				tryMove(movement);
			}
		} else if (movement == orientation * -1) {
			// if (isGrabbing) {
			// 	grabbingBlock.tryMove(movement);
			// } else {
			// 	CubeObject blockInFront = levelManager.getCubeObjIn(levelPos + movement);
			// 	if (blockInFront != null) {
			// 		tryJump(movement);
			// 	} else {
			// 		tryMove(movement);
			// 	}
			// }
			if (isGrabbing) {
				Debug.Log("pulling block backwards");
				facingBlock.tryMove(movement);
			} else {
				//can't jump up if not facing it
				tryMove(movement);
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
