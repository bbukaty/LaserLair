using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeObject: MonoBehaviour {

	public Vector3Int orientation;
    public Vector3Int levelPos;
    public Transform corpsePrefab;
    public bool isMovable;
    public bool diesToExplosion;
    public bool diesToLaser;

    protected LevelManager levelManager;

    void Awake() {
        // find the level manager object in the scene to get level data from
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
		Debug.Assert(levelManager != null, "Warning: Level Manager script not found in scene!");
        initOrientation();
		initLevelPos();
	}

    private void initLevelPos() {
        levelPos = Vector3Int.RoundToInt(transform.position);
        Debug.Assert((Vector3)levelPos == transform.position, "Warning: Improperly aligned object has been initialized!");
    }

    private void initOrientation() {
        orientation = new Vector3Int();
        for (int i = 0; i < 3; i++) {
            Debug.Assert(Mathf.Abs((float)(int)transform.forward[i] - transform.forward[i]) < 0.001, "Warning: Level contains improperly oriented cube!");
            orientation[i] = (int)transform.forward[i];
        }
    }

    public bool modelIsMoving() {
		Rigidbody body = transform.GetComponent<Rigidbody>();
		return body.angularVelocity.magnitude == 0 && body.velocity.magnitude != 0;
	}

    public void updatePos(Vector3Int movement) {
        levelManager.moveBlock(levelPos, levelPos + movement);
        levelPos += movement;
        transform.Translate(movement, Space.World);
        getMoveConsequences();
    }

    public void updateOrientation(Vector3Int direction) {
		transform.forward = direction;
		orientation = direction;
	}

    ///<summary>
	///Try to push this block in the movement direction, propogates push through other blocks.
	///Returns whether push was successful. 
	///</summary>
    public bool push(Vector3Int movement, bool justChecking = false) {
        bool canMove = false;
        if (isMovable) {
            Vector3Int adjacentPos = levelPos + movement;
            if (levelManager.isInBounds(adjacentPos)) {
                CubeObject adjacentBlock = levelManager.getCubeObjIn(adjacentPos);
                if (adjacentBlock == null) {
                    canMove = true;
                } else {
                    canMove = adjacentBlock.push(movement, justChecking);
                }
            }
        }
        if (canMove && !justChecking) {
            // update position in level manager and in internal levelPos var
            updatePos(movement);
		}
        return canMove;
    }

    public virtual void getMoveConsequences() {
		if (diesToLaser && levelManager.isInLaser(levelPos)) {
			die();
		}
		tryFall();
    }

    public virtual void die() {
        Destroy(gameObject);
    }

    protected void tryFall() {
		Vector3Int below = levelPos + Vector3Int.down;
		if (!levelManager.isInBounds(below)) {
            //TODO: trigger this for all cubeObjects above this immediately
			// fall out of level, remove character component while falling so they can't move
			Rigidbody body = GetComponent<Rigidbody>();
			body.useGravity = true;
			body.isKinematic = false;
			Destroy(gameObject, 2);
			Destroy(this);
		} else if (levelManager.getCubeObjIn(below) == null) {
			updatePos(Vector3Int.down);
		}
	}
}