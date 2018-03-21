using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour {

	public Transform buttonPrefab;
	public Transform buttonPanel;
	public Transform[] robotPrefabs;
	public Sprite[] buttonSprites;
	public Vector3Int spawnLoc;
	public FollowCamera charCam;

	private Transform currChar;
	private bool charIsScientist;
	private LevelManager levelManager;
	// for panel movement
	private bool panelIsMoving;
	private float timeStartedLerping;
	private Vector2 startPosition;
	private Vector2 endPosition;
	

	

	void Awake() {
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
		Debug.Assert(levelManager != null, "Warning: Level Manager script not found in scene!");
		Debug.Assert(robotPrefabs.Length == buttonSprites.Length, "Warning: Spawner prefabs and sprites lists don't match!");
		currChar = null;
		charIsScientist = false;
	}

	void Start() {
		// create a UI spawn button for each robot prefab
		for (int i = 0; i < robotPrefabs.Length; i++) {
			Transform buttonTransform = Instantiate(buttonPrefab, buttonPanel);
			Transform robotToSpawn = robotPrefabs[i];
			buttonTransform.GetComponent<Button>().onClick.AddListener(delegate{spawn(robotToSpawn);});
			buttonTransform.GetComponent<Image>().sprite = buttonSprites[i];
		}
	}

	public void spawn(Transform robotPrefab) {
		if (currChar == null) { // make sure you can't press the button multiple times
			currChar = Instantiate(robotPrefab, spawnLoc, Quaternion.identity, levelManager.transform);
			CubeObject charCubeObj = currChar.GetComponent<CubeObject>();
			// we'll need this bool to check later because the actual object will have been destroyed
			charIsScientist = charCubeObj is Scientist;
			levelManager.addBlock(charCubeObj);
			charCam.follow(currChar);
			moveSpawnPanel(true);
		}
	}

	public void onCharacterDeath() {
		if (charIsScientist) {
			//display game over + restart
			Debug.Log("Game Over");
		} else {
			// reset spawn button panel into view
			moveSpawnPanel(false);
		}
	}

	private void moveSpawnPanel(bool hiding) {
		panelIsMoving = true;
		timeStartedLerping = Time.time;
		startPosition = hiding ? Vector2.zero : Vector2.down * 180f;
		endPosition = hiding ? Vector2.down * 180f : Vector2.zero;
	}

	void Update() {
		if (panelIsMoving) {
			float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / 0.4f;
 
            buttonPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp (startPosition, endPosition, percentageComplete);
 
            if(percentageComplete >= 1.0f) {
                panelIsMoving = false;
            }
		}
	}
}
