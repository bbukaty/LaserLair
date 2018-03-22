using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character: MonoBehaviour {

	public bool canGrab;
	public bool canJump; 
	public AudioClip walkingClip;

	private Animator characterAnimator;
	private bool isGrabbing;
    private LevelManager levelManager;
	private CharacterManager characterManager;
	private CubeObject cubeObject;
    private AudioSource audioSource;

	void Awake() {
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
		Debug.Assert(levelManager != null, "Warning: Level Manager object not found in scene!");
		characterManager = GameObject.Find("CharacterManager").GetComponent<CharacterManager>();
		Debug.Assert(characterManager != null, "Warning: Spawner object not found in scene!");
		cubeObject = GetComponent<CubeObject>();
		Debug.Assert(cubeObject != null, "Warning: Character initialized with no CubeObject attached!");
		isGrabbing = false;
		audioSource = GetComponent<AudioSource>();
		characterAnimator = GetComponentInChildren<Animator>();
	}

	private void walkSound() {
		audioSource.clip = walkingClip;
		audioSource.volume = Random.Range(0.8f, 1f);
		audioSource.pitch = Random.Range(0.8f, 1.1f);
		audioSource.Play();
	}
    // Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Left")) {
			walkSound();
			applyInput(new Vector3Int(-1, 0, 0));
		} else if (Input.GetButtonDown ("Right")) {
			walkSound();
			applyInput(new Vector3Int(1, 0, 0));
		} else if (Input.GetButtonDown ("Up")) {
			walkSound();
			applyInput(new Vector3Int(0, 0, 1));
		} else if (Input.GetButtonDown ("Down")) {
			walkSound();
			applyInput(new Vector3Int(0, 0, -1));
		} else if (Input.GetButtonDown("Grab") && canGrab) {
			tryGrab();
		}
		if (!cubeObject.isMoving) {
			characterAnimator.SetBool("isMovingForward", false);
			characterAnimator.SetBool("isMovingBack", false);
		}
	}

	private void tryGrab() {
		if (cubeObject.modelIsMoving()) {
			return;
		} else if (isGrabbing) {
			characterAnimator.SetBool("isGrabbing", false);
			isGrabbing = false;
		} else if (levelManager.getCubeObjIn(cubeObject.levelPos + cubeObject.orientation) != null) {
			// toggle grab animation
			isGrabbing = true;
		}
		characterAnimator.SetBool("isGrabbing", isGrabbing);
	}

    private void applyInput(Vector3Int direction) {
		if (cubeObject.modelIsMoving()) {
			return;
		}
		CubeObject facingBlock = levelManager.getCubeObjIn(cubeObject.levelPos + cubeObject.orientation);
		if (direction == cubeObject.orientation) {
			characterAnimator.SetBool("isMovingForward", true);
			if (isGrabbing) {
				levelManager.move(cubeObject.levelPos, direction);
				isGrabbing = levelManager.getCubeObjIn(cubeObject.levelPos + cubeObject.orientation) != null;
				// TODO: keep better track of grabbing, this breaks down if the block you were grabbing falls and is replaced
			} else if (facingBlock != null) {
				tryJump(direction);
			} else {
				levelManager.move(cubeObject.levelPos, direction);
			}
		} else if (direction == cubeObject.orientation * -1) {
			characterAnimator.SetBool("isMovingBack", true);
			//can't jump up backwards
			if (isGrabbing) {
				levelManager.move(facingBlock.levelPos, direction);
				isGrabbing = levelManager.getCubeObjIn(cubeObject.levelPos + cubeObject.orientation) != null;
				// TODO: keep better track of grabbing, this breaks down if the block you were grabbing falls and is replaced
			} else {
				levelManager.move(cubeObject.levelPos, direction);
			}
		} else { // movement axis doesn't align with current orientation
			// drop grabbed block
			isGrabbing = false;
			cubeObject.updateOrientation(direction);
			//TODO: rotate block on head (?)
		}
		characterAnimator.SetBool("isGrabbing", isGrabbing);
		
	}
	
	private void tryJump(Vector3Int movement) {
		if (!canJump) {
			return;
		}
		Vector3Int newPos = cubeObject.levelPos + movement + Vector3Int.up;
		if (levelManager.isInBounds(newPos) && levelManager.getCubeObjIn(newPos) == null) {
			levelManager.move(cubeObject.levelPos, movement + Vector3Int.up);
		}
	}

	void OnDestroy() {
		characterManager.onCharacterDeath();
	}
}
