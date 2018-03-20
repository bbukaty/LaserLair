using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour {

	public GameObject explosionAnimation;
	private CubeObject[,,] level;
	private List<LaserBlock> laserBlocks;

	void Awake() {
		laserBlocks = new List<LaserBlock>();
	}

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
			// also populate the laserBlock list
			if (block is LaserBlock) {
				laserBlocks.Add((LaserBlock)block);
			}
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

	///<summary>
	///Moves the cubeObject at pos in direction, pushing blocks in the way and updating the level with the consequences of the move.
	///</summary>
	public void move(Vector3Int pos, Vector3Int direction) {
		List<Vector3Int> updatedBlocks = new List<Vector3Int>();
		tryPush(pos, direction, updatedBlocks);
		bool stillFalling = true;
		while (stillFalling) {
			getMoveConsequences(updatedBlocks); // this might add more blocks to the list
			stillFalling = iterateFalling(updatedBlocks);
		}
	}

    private bool tryPush(Vector3Int pos, Vector3Int direction, List<Vector3Int> updatedBlocks) {
		CubeObject blockToMove = getCubeObjIn(pos);
		Debug.Assert(blockToMove != null, "Warning: trying to push a null block!");
		Vector3Int adjacentPos = pos + direction;
		if (blockToMove.isMovable && isInBounds(adjacentPos)) {
			CubeObject adjacentBlock = getCubeObjIn(adjacentPos);
			if (adjacentBlock == null || tryPush(adjacentPos, direction, updatedBlocks)) {
				// can push, update position in level array
				moveInArray(pos, adjacentPos);
				// move block in world
				blockToMove.updatePos(direction);
				updatedBlocks.Add(blockToMove.levelPos); 
				// try to move blocks on top of the pushed block
				if (getCubeObjIn(pos+Vector3Int.up) != null) {
					tryPush(pos + Vector3Int.up, direction, updatedBlocks);
				}
				return true;
			}
		}
        return false;
    }

	private void moveInArray(Vector3Int pos1, Vector3Int pos2) {
		Debug.Assert(getCubeObjIn(pos2) == null, "Warning: overwriting existing block in levelManager!");
		level[pos2.x, pos2.y, pos2.z] = level[pos1.x, pos1.y, pos1.z];
		level[pos1.x, pos1.y, pos1.z] = null;
	}

	private void getMoveConsequences(List<Vector3Int> updatedBlocks) {
		// kill blocks in lasers, explosions will add blocks to updatedBlocks list
		foreach (LaserBlock laserBlock in laserBlocks) {
			CubeObject laserTarget = getLaserTarget(laserBlock.levelPos, laserBlock.orientation);
			if (laserTarget is ExplodeBlock) {
				explodeOutwards(laserTarget.levelPos, updatedBlocks);
			} else if (laserTarget is BlockRobot) {
				((BlockRobot)laserTarget).spawnCorpse();
				laserTarget.die();
			} else if (laserTarget is Scientist) {
				laserTarget.die();
			}
		}
		// prune dead blocks from updatedBlocks, check if won
		for (int i = 0; i < updatedBlocks.Count; i++) {
			Vector3Int blockPos = updatedBlocks[i];
			CubeObject block = getCubeObjIn(blockPos);
			if (block == null) {
				updatedBlocks.RemoveAt(i);
				i--;
			} else if (block is Scientist && getCubeObjIn(blockPos + Vector3Int.down) is GoalBlock) {
				win();
			}
			
		}
	}

	private bool iterateFalling(List<Vector3Int> blockPositions) {
		for (int i = 0; i < blockPositions.Count; i++) {
		}
		bool blocksFell = false;
		for (int i = 0; i < blockPositions.Count; i++) {
			CubeObject blockToFall = getCubeObjIn(blockPositions[i]);
			Debug.Assert(blockToFall != null, "Warning: tryFall called on null block!");
			Vector3Int below = blockPositions[i] + Vector3Int.down;
			if (!isInBounds(below)) {
				blocksFell = true;
				fallOut(blockToFall);
				blockPositions.RemoveAt(i);
				i--;
			} else if (getCubeObjIn(below) == null) {
				blocksFell = true;
				// update position in level array and updatedBlocks
				moveInArray(blockToFall.levelPos, below);
				blockPositions[i] = below;
				// move block in world
				blockToFall.updatePos(Vector3Int.down);
			}
		}
		return blocksFell;

	}

	private void fallOut(CubeObject blockToFall) {
		Rigidbody body = blockToFall.GetComponent<Rigidbody>();
		body.useGravity = true;
		body.isKinematic = false;
		Destroy(blockToFall.gameObject, 1);
		DestroyImmediate(blockToFall);
	}

	private CubeObject getLaserTarget(Vector3Int laserPos, Vector3Int direction) {
		Vector3Int adjacentPos = laserPos + direction;
		if (!isInBounds(adjacentPos)) {
			return null;
		}
		CubeObject adjacentBlock = getCubeObjIn(adjacentPos);
		if (adjacentBlock == null || adjacentBlock is GlassBlock) {
			// there's no block in the way, keep searching in this direction
			return getLaserTarget(adjacentPos, direction);
		} else {
			return adjacentBlock;
		}
	}

	public void explodeOutwards(Vector3Int pos, List<Vector3Int> updatedBlocks) {
		CubeObject blockToExplode = getCubeObjIn(pos);
		Debug.Assert(blockToExplode != null, "Warning: attempting to explode an empty block somehow.");
		DestroyImmediate(blockToExplode.gameObject); // immediate prevents other explosions from re-exploding this block
		animateExplosion(pos);
		for (int sign = -1; sign <= 1; sign += 2) {
			for (int i = 0; i < 3; i++) {
				Vector3Int searchOrientation = new Vector3Int(0, 0, 0);
				searchOrientation[i] = sign;
				explodeInDirection(searchOrientation, pos, updatedBlocks);
			}
		}
	}

	private void explodeInDirection(Vector3Int direction, Vector3Int pos, List<Vector3Int> updatedBlocks) {
		Vector3Int adjacentPos = pos + direction;
		if (!isInBounds(adjacentPos)) {
			return;
		}
		explodePos(adjacentPos, updatedBlocks);
		explodeInDirection(direction, adjacentPos, updatedBlocks);
	}

	private void explodePos(Vector3Int pos, List<Vector3Int> updatedBlocks) {
		CubeObject occupant = getCubeObjIn(pos);
		if (occupant == null) {
			animateExplosion(pos);
		} else if (occupant is ExplodeBlock) {
			explodeOutwards(pos, updatedBlocks);
		} else if (occupant is BlockRobot || occupant is Scientist || occupant is CrackedBlock || occupant is NormalBlock) {
			occupant.die();
		}
	}

	public void animateExplosion(Vector3Int pos) {
		GameObject explosion = Instantiate(explosionAnimation, pos, Quaternion.identity);
	 	Destroy(explosion, .5f);
	}
}

