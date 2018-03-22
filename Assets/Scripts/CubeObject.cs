using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeObject: MonoBehaviour {

	public Vector3Int orientation;
    public Vector3Int levelPos;
    public bool isMovable;

    protected LevelManager levelManager;

    private bool isMoving;
    private float timeStartedMoving;
	private Vector3 startPosition;
	private Vector3 endPosition;

    private bool isRotating;
    private float timeStartedRotating;
    private Vector3 startRotation;
	private Vector3 endRotation;

    void Awake() {
        // find the level manager object in the scene to get level data from
        Debug.Assert(transform.parent != null, "Warning: Level Manager script not found in scene!");
		levelManager = transform.parent.GetComponent<LevelManager>();
        initOrientation();
		initLevelPos();
        isMoving = false;
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
        levelManager.movingBlocks += 1;
        levelPos += movement;
        // transform.Translate(movement, Space.World);
        isMoving = true;
        timeStartedMoving = Time.time;
        startPosition = transform.position;
        endPosition = transform.position + movement;
    }

    public void updateOrientation(Vector3Int direction) {
		// transform.forward = direction;
        isRotating = true;
		orientation = direction;
        timeStartedRotating = Time.time;
        startRotation = transform.forward;
        endRotation = direction;
	}

    public void die() {
		levelManager.animateExplosion(levelPos);
		DestroyImmediate(gameObject);
	}

    void Update() {
        if (isMoving) {
			float timeSinceStarted = Time.time - timeStartedMoving;
            float percentageComplete = timeSinceStarted / 0.08f;
 
            transform.position = Vector3.Lerp(startPosition, endPosition, percentageComplete);
 
            if (percentageComplete >= 1.0f) {
                isMoving = false;
                levelManager.movingBlocks -= 1;
            }
        }
        if (isRotating) {
            float timeSinceStarted = Time.time - timeStartedRotating;
            float percentageComplete = timeSinceStarted / 0.1f;
 
            transform.forward = Vector3.Lerp(startRotation, endRotation, percentageComplete);
 
            if (percentageComplete >= 1.0f) {
                isRotating = false;
            }
        }
    }
}