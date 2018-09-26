using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTrigger : MonoBehaviour {

	public List<GameObject> textBoxes;
	
	void OnTriggerEnter(Collider other) {
		foreach (GameObject textBox in textBoxes) {
			textBox.SetActive(!textBox.activeSelf);
			Destroy(gameObject);
		}
	}
}
