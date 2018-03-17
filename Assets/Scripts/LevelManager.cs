using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour {

	public GameObject explosionAnimation;
	private CubeObject[,,] level;

	void Start() {
		// on Start, all the blocks' position/orientation data will be initialized
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
		level[pos.x, pos.y, pos.z] = blockToAdd;
	}

	private void moveInArray(Vector3Int pos1, Vector3Int pos2) {
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

	public void move(Vector3Int pos, Vector3Int direction) {
		List<Vector3Int> movedBlocks = new List<Vector3Int>();
		tryPush(pos, direction, movedBlocks);

		movedBlocks.ForEach(getMoveConsequences);
		// for each block without anything underneath it, fall its whole stack
		// otherwise iterate upwards and fall stuff down to get rid of holes from explosions

		//check if scientist on goalblock
	}

    private bool tryPush(Vector3Int pos, Vector3Int direction, List<Vector3Int> movedBlocks) {
		CubeObject blockToMove = getCubeObjIn(pos);
		Debug.Assert(blockToMove != null, "Warning: trying to push a null block!");
		Vector3Int adjacentPos = pos + direction;
		if (blockToMove.isMovable && isInBounds(adjacentPos)) {
			CubeObject adjacentBlock = getCubeObjIn(adjacentPos);
			if (adjacentBlock == null || tryPush(adjacentPos, direction, movedBlocks)) {
				// can push, update position in level array
				moveInArray(pos, adjacentPos);
				// move block in world
				blockToMove.updatePos(direction);
				movedBlocks.Add(blockToMove.levelPos); 
				// try to move blocks on top of the pushed block
				tryPush(pos + Vector3Int.up, direction, movedBlocks);
				return true;
			}
		}
        return false;
    }

	private void getMoveConsequences(Vector3Int pos) {
		Debug.Assert(isInBounds(pos), "Warning: movedBlocks contains an out of bounds position!");
		CubeObject block = getCubeObjIn(pos);

		// if block was already destroyed somehow, none of these will be true
		if (block is ExplodeBlock) {
			if (isInLaser(pos)) {
				explodeOutwards(pos);
			}
		} else if (block is BlockRobot) {
			if (isInLaser(pos)) {
				((BlockRobot)block).spawnCorpse();
				block.die();
			}
		} else if (block is Scientist) {
			if (isInLaser(pos)) {
				block.die();
			}
		}
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
		CubeObject blockToExplode = getCubeObjIn(pos);
		Debug.Assert(blockToExplode != null, "Warning: attempting to explode an empty block somehow.");
		DestroyImmediate(blockToExplode.gameObject); // immediate prevents other explosions from re-exploding this block
		animateExplosion(pos);
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
			animateExplosion(pos);
		} else if (occupant is ExplodeBlock) {
			explodeOutwards(pos);
		} else if (occupant is BlockRobot || occupant is Scientist || occupant is CrackedBlock || occupant is NormalBlock) {
			occupant.die();
		}
	}

	public void animateExplosion(Vector3Int pos) {
		GameObject explosion = Instantiate(explosionAnimation, pos, Quaternion.identity);
	 	Destroy(explosion, .5f);
	}
}

