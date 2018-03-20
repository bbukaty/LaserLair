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
	public FollowCamera playerCam;

	private Transform currPlayer;
	private Transform levelManager;
	private RectTransform panelPos;
	

	void Awake() {
		levelManager = GameObject.Find("LevelManager").transform;
		Debug.Assert(levelManager != null, "Warning: Level Manager script not found in scene!");
		Debug.Assert(robotPrefabs.Length == buttonSprites.Length, "Warning: Spawner prefabs and sprites lists don't match!");
		currPlayer = null;
		panelPos = buttonPanel.GetComponent<RectTransform>();
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


	void Update() {
		if (currPlayer == null && panelPos.anchoredPosition != Vector2.zero) {
			panelPos.anchoredPosition = Vector2.zero;
		} 
	}

	public void spawn(Transform robotPrefab) {
		// Reminder: if the previous player died or was deactivated, its gameObject was destroyed and replaced.
		if (currPlayer == null) {
			panelPos.anchoredPosition += Vector2.down * 180f;
			currPlayer = Instantiate(robotPrefab, spawnLoc, Quaternion.identity, levelManager);
			levelManager.GetComponent<LevelManager>().addBlock(currPlayer.GetComponent<CubeObject>());
			playerCam.follow(currPlayer);
		}
	}

}
