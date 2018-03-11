using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scientist: Character {

	protected override void die() {
		Destroy(gameObject);
	}

}
