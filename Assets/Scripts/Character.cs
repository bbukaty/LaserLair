using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character: MonoBehaviour {

	public bool canGrab;
	public bool canJump; 

	private bool isGrabbing;
    private LevelManager levelManager;
	private CubeObject cubeObject;

	void Awake() {
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
		Debug.Assert(levelManager != null, "Warning: Level Manager script not found in scene!");
		cubeObject = GetComponent<CubeObject>();
		Debug.Assert(cubeObject != null, "Warning: Character initialized with no CubeObject attached!");
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
		} else if (Input.GetButtonDown("Grab") && canGrab) {
			tryGrab();
		}
	}

	private void tryGrab() {
		if (cubeObject.modelIsMoving()) {
			return;
		} else if (isGrabbing) {
			isGrabbing = false;
		} else if (levelManager.getCubeObjIn(cubeObject.levelPos + cubeObject.orientation) != null) {
			// toggle grab animation
			isGrabbing = true;
		}
	}

    private void applyInput(Vector3Int movement) {
		if (cubeObject.modelIsMoving()) {
			return;
		}
		Debug.Log("moving: " + movement.ToString());
		CubeObject facingBlock = levelManager.getCubeObjIn(cubeObject.levelPos + cubeObject.orientation);
		if (movement == cubeObject.orientation) {
			if (isGrabbing) {
				cubeObject.push(movement);
			} else if (facingBlock != null) {
				tryJump(movement);
			} else {
				cubeObject.push(movement);
			}
		} else if (movement == cubeObject.orientation * -1) {
			//can't jump up backwards
			if (isGrabbing) {
				Debug.Log("pulling block backwards");
				facingBlock.push(movement);
			} else {
				cubeObject.push(movement);
			}
		} else { // movement axis doesn't align with current orientation
			// drop grabbed block
			isGrabbing = false;
			cubeObject.updateOrientation(movement);
		}
	}
	
	private void tryJump(Vector3Int movement) {
		if (!canJump) {
			return;
		}
		Vector3Int newPos = cubeObject.levelPos + movement + Vector3Int.up;
		if (levelManager.isInBounds(newPos) && levelManager.getCubeObjIn(newPos) == null) {
			cubeObject.updatePos(movement + Vector3Int.up);
		}
	}
}
