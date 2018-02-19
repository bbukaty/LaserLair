using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour {

	//prefab references
	public Transform blockRobotPrefab;
	public Transform normalBlockPrefab;

	private CubeObject[,,] level;
	private Transform currPlayer;

	private class CubeObject {
		private int orientation;

	}

	private class BlockRobot : CubeObject {
		private bool alive;
		private int[] position;
		private Transform modelPosition;

		public BlockRobot(int x, int y, int z) {
			position = new int[3] {x, y, z};
			modelPosition = Instantiate (blockRobotPrefab, new Vector3(x, y, z), Quaternion.identity).GetComponent<Transform>();
		}

		public bool isAlive() {
			return alive;
		}

		public void 
	}

	private class NormalBlock : CubeObject {
		/*
		public NormalBlock(float x, float y, float z) {
			Instantiate(normalBlockPrefab, new Vector3(x, y, z), Quaternion.identity);
		}
		*/
	}

	void Start () {
		getLevelFromScene ();

		string[,,] levelSpec = new string[4, 5, 5] {
			{{"bn","le","bn","bn","bn"},{"bn","n","n","n","n"},{"bn","n","n","n","n"},{"bn","n","n","n","n"},{"bn","n","n","n","n"}},
			{{"bn","bn","bn","bn","bn"},{"bn","n","n","n","n"},{"bn","n","n","n","n"},{"bn","n","n","n","n"},{"bn","n","n","n","n"}},
			{{"bn","bn","bn","bn","bn"},{"bn","n","n","n","n"},{"bn","n","n","n","n"},{"bn","n","n","n","n"},{"bn","n","n","n","n"}},
			{{"bn","bn","bn","bn","bn"},{"bn","n","n","n","n"},{"bn","n","n","n","n"},{"bn","n","n","n","n"},{"bn","n","n","n","n"}}
		};

		//initializeLevel(levelSpec);
		initializePlayer();

	}

	void getLevelFromScene() {
		// Gets the furthest cube in each direction to create a level array of the right size
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
		level = new CubeObject[maxPositions [0] + 1, maxPositions [1] + 1, maxPositions [2] + 1];
		// now populate the level array
		foreach (Transform block in transform) {
			int[] gridPosition = new int[3];
			for (int i = 0; i < 3; i++) {
				gridPosition[i] = (int)block.position [i];
			}
			level [gridPosition [0], gridPosition [1], gridPosition [2]] = new CubeObject ();
		}
		// debug printing
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

	
	void initializeLevel(string[,,] levelSpec) {
		// not for-each for now, we'll probably want indices for stuff
		for (int z = 0; z < levelSpec.GetLength(0); z++) {
			for (int x = 0; x < levelSpec.GetLength (1); x++) {
				for (int y = 0; y < levelSpec.GetLength (2); y++) {
					if (levelSpec[z, x, y] == "bn") {
						//flip the z coordinate so in the world it looks like it does in the array
						//level[z,x,y] = new NormalBlock(x, y, -1*z + levelSpec.GetLength(0) - 1);
					}
				}
			}
		}
	}

	void initializePlayer() {
		currPlayer = new BlockRobot (new Vector3 (1, 1, 0));
			
	}
		
	void Update() {
		playerIsMoving = playerModel.GetComponentInChildren<Rigidbody> ().velocity.magnitude != 0;
		if (!playerIsMoving) {
			if (Input.GetButtonDown ("left")) {
				
			}
			if (Input.GetButtonDown ("right")) {
				if (player.position.x + 1 < level.GetLength(1)) {
					player.Translate(new Vector3(1, 0, 0));
					Debug.Log("moving right");
				}
			}
		}

	}
}

