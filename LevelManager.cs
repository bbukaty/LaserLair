using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour {

	//prefab references
	public Transform blockRobotPrefab;

	private Block[,,] level;
	private Robot currPlayer;

	private class Block {
		private int orientation;
	}

	private class Robot {
		protected int[] position;
		protected Transform modelTransform;

		public Robot(Transform prefab, int[] pos) {
			this.position = pos;
			this.modelTransform = Instantiate (prefab, new Vector3(pos[0], pos[1], pos[2]), Quaternion.identity).GetComponent<Transform>();
		}

		public bool isMoving() {
			return this.modelTransform.GetComponentInChildren<Rigidbody> ().velocity.magnitude != 0;
		}

		public bool tryMove(int[] movement) {
			if (!this.isMoving ()) {
				Debug.Log("trying to move: " + movement[0].ToString() + movement[1].ToString() + movement[2].ToString());
			}
			return false;
		}
	}

	void Start () {
		getLevelFromScene ();
		currPlayer = new Robot (blockRobotPrefab, new int[] {1,1,0});

	}

	void Update() {

		if (Input.GetButtonDown ("left")) {
			currPlayer.tryMove (new int[3]{ -1, 0, 0 });
		}
		if (Input.GetButtonDown ("right")) {
			currPlayer.tryMove (new int[3]{ 1, 0, 0 });
		}
	}

	void getLevelFromScene() {
		// Gets cube positions straight from the editor to build a data representation of the level.
		// First get the furthest cube in each direction to create a level array of the right size
		int[] maxPositions = new int[3]{ 0, 0, 0 };
		// for each child block of level manager
		foreach (Transform block in transform) {
			for (int i = 0; i < 3; i++) {
				int ithDimension = (int)block.position [i];
				Debug.Assert ((float)ithDimension == block.position [i], "Error: Level contains misaligned cube!");
				Debug.Assert (ithDimension >= 0, "Error: Cube coordinates must be positive!");
				if (ithDimension > maxPositions[i]) {
					maxPositions [i] = ithDimension;
				}
			}
		}
		level = new Block[maxPositions [0] + 1, maxPositions [1] + 1, maxPositions [2] + 1];
		// now populate the level array
		foreach (Transform block in transform) {
			int[] gridPosition = new int[3];
			for (int i = 0; i < 3; i++) {
				gridPosition[i] = (int)block.position [i];
			}
			// TODO: support for different block types
			level [gridPosition [0], gridPosition [1], gridPosition [2]] = new Block ();
		}
		// debug logs
		for (int x = 0; x < level.GetLength (0); x++) {
			for (int y = 0; y < level.GetLength (1); y++) {
				for (int z = 0; z < level.GetLength (2); z++) {
					if (level [x, y, z] != null) {
						Debug.Log ("Cube at " + x.ToString () + ", " + y.ToString () + ", " + z.ToString ());
					}
				}
			}
		}
	}
}

