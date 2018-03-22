using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeObject: MonoBehaviour {

	public Vector3Int orientation;
    public Vector3Int levelPos;
    public bool isMovable;

    protected LevelManager levelManager;

    void Awake() {
        // find the level manager object in the scene to get level data from
        Debug.Assert(transform.parent != null, "Warning: Level Manager script not found in scene!");
		levelManager = transform.parent.GetComponent<LevelManager>();
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
        levelPos += movement;
        transform.Translate(movement, Space.World);
    }

    public void updateOrientation(Vector3Int direction) {
		transform.forward = direction;
		orientation = direction;
	}

    public void die() {
		levelManager.animateExplosion(levelPos);
		DestroyImmediate(gameObject);
	}
}