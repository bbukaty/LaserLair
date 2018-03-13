using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour {

	private CubeObject[,,] level;

	void Start() {
		getLevelFromScene();
	}

	///<summary>
	///Scans through the children of the levelManager in the editor and populates the level array with them. 
	///</summary>
	private void getLevelFromScene() {
		// First get the furthest cube in each direction to create a level array of the right size
		Vector3Int levelBounds = new Vector3Int(0,0,0);
		// for each child CubeObject of level manager
		foreach (Transform blockTransform in transform) {
			Debug.Assert(blockTransform.GetComponent<CubeObject>() != null, "Warning: CubeObject in level is missing CubeObject script!");
			CubeObject block = blockTransform.GetComponent<CubeObject>();
			for (int i = 0; i < 3; i++) {
				if (block.levelPos[i] > levelBounds[i]) {
					levelBounds[i] = block.levelPos[i];
				}
			}
		}

		levelBounds = levelBounds + new Vector3Int(1,1,1); // account for zero-indexing
		level = new CubeObject[levelBounds.x, levelBounds.y, levelBounds.z];
		
		// now populate the level array
		foreach (Transform blockTransform in transform) {
			addBlock(blockTransform.GetComponent<CubeObject>());
		}
		// printLevel();
	}

	public void printLevel() {
		for (int x = 0; x < level.GetLength(0); x++) {
			for (int y = 0; y < level.GetLength(1); y++) {
				for (int z = 0; z < level.GetLength(2); z++) {
					if (level[x, y, z] != null) {
						CubeObject block = level[x, y, z].GetComponent<CubeObject>();
						Debug.Log("Cube at " + block.levelPos.ToString());
						Debug.Log("Orientation: " + block.orientation.ToString());
					}
				}
			}
		}
	}

	public void win() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
	
	///<summary>
	///Returns whether pos is a valid index into the level array. 
	///</summary>
	public bool isInBounds(Vector3Int pos) {
		for (int i = 0; i < 3; i++) {
			if (pos[i] < 0 || pos[i] > level.GetUpperBound(i)) {
				return false;
			}
		}
		return true;
	}

	///<summary>
	///Returns the item at pos in the level array or null if pos is out of bounds.
	///</summary>
	public CubeObject getCubeObjIn(Vector3Int pos) {
		return (isInBounds(pos)) ? level[pos[0], pos[1], pos[2]] : null;
	}

	public void addBlock(CubeObject blockToAdd) {
		Vector3Int pos = blockToAdd.levelPos;
		Debug.Assert(level[pos.x, pos.y, pos.z] == null, "Warning: Adding block into an occupied position in the level!");
		level[pos.x, pos.y, pos.z] = blockToAdd;
	}

	public void moveBlock(Vector3Int pos1, Vector3Int pos2) {
		Debug.Assert(getCubeObjIn(pos2) == null, "Warning: overwriting existing block in levelManager!");
		level[pos2.x, pos2.y, pos2.z] = level[pos1.x, pos1.y, pos1.z];
		level[pos1.x, pos1.y, pos1.z] = null;
	}

	///<summary>
	///Returns whether or not a laser passes through position pos.
	///</summary>
	public bool isInLaser(Vector3Int pos) {
		bool result = false;
		for (int sign = -1; sign <= 1; sign += 2) {
			for (int i = 0; i < 3; i++) {
				Vector3Int searchOrientation = new Vector3Int(0, 0, 0);
				searchOrientation[i] = sign;
				if (isLaserInDirection(searchOrientation, pos)) {
					result = true;
				}
			}
		}
		return result;
	}

	private bool isLaserInDirection(Vector3Int direction, Vector3Int pos) {
		Vector3Int adjacentPos = pos + direction;
		if (!isInBounds(adjacentPos)) {
			return false;
		}
		CubeObject adjacentBlock = getCubeObjIn(adjacentPos);
		if (adjacentBlock == null || adjacentBlock is GlassBlock) {
			// there's no block in the way, keep searching in this direction
			return isLaserInDirection(direction, adjacentPos);
		} else if (adjacentBlock is LaserBlock && adjacentBlock.orientation == direction * -1) {
			// there's a laser pointing towards pos
			return true;
		} else {
			// there's a non-laser block in the way of any potential beams in this direction
			return false;
		}
	}

	public void explodeOutwards(Vector3Int pos) {
		for (int sign = -1; sign <= 1; sign += 2) {
			for (int i = 0; i < 3; i++) {
				Vector3Int searchOrientation = new Vector3Int(0, 0, 0);
				searchOrientation[i] = sign;
				explodeInDirection(searchOrientation, pos);
			}
		}
	}

	private void explodeInDirection(Vector3Int direction, Vector3Int pos) {
		Vector3Int adjacentPos = pos + direction;
		if (!isInBounds(adjacentPos)) {
			return;
		}
		explodePos(adjacentPos);
		explodeInDirection(direction, adjacentPos);
	}

	private void explodePos(Vector3Int pos) {
		CubeObject occupant = getCubeObjIn(pos);
		if (occupant == null) {
			explodeAnimation(pos);
		} else {
			if (occupant.diesToExplosion) {
				explodeAnimation(pos);
				occupant.die();
			}
		}
	}

	private void explodeAnimation(Vector3Int pos) {
		return;
	}
}

