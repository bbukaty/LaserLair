using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour {
	[System.Serializable]
	public class Level{
		public string LevelText;
		public int UnLocked;
		public bool isInteractable;
	
	}

	public GameObject levelButton;
	public Transform Spacer;
	public List<Level> LevelList;

	void Start () {
		DeleteAll();
		FillList();
	}

	void FillList()
	{
		foreach (var level in LevelList)
		{
			GameObject newButton = Instantiate(levelButton) as GameObject;
			LevelButtonNew button = newButton.GetComponent<LevelButtonNew>();
			button.LevelText.text = level.LevelText;

			// If the the level is unlocked then load it
			if (PlayerPrefs.GetInt("level" + button.LevelText.text) == 1){
				level.UnLocked = 1;
				level.isInteractable = true;
			}

			button.unlocked = level.UnLocked;
			button.GetComponent<Button>().interactable = level.isInteractable;
			button.GetComponent<Button>().onClick.AddListener(() => loadLevels("level" + button.LevelText.text));
			newButton.transform.SetParent(Spacer, false);
		}
		SaveAll();
	}

	void SaveAll()
	{
		/* 
		if(PlayerPrefs.HasKey("level1"))
		{
			return;
		} 
		else
		*/
		{
			GameObject[] allButtons = GameObject.FindGameObjectsWithTag("LevelButton");
			foreach (GameObject buttons in allButtons)
			{
				LevelButtonNew button = buttons.GetComponent<LevelButtonNew>();
				PlayerPrefs.SetInt("level" + button.LevelText.text, button.unlocked);
			}
		}
	}

	void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}

	void loadLevels(string val)
	{
		Application.LoadLevel(val);
	}
}
