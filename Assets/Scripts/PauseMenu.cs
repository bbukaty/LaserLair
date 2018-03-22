using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

	public static bool GameIsPaused = false;

	public GameObject pauseMenuUI;

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if (GameIsPaused)
			{
				Resume();
			} else
			{
				Pause();
			}
		}
	}

	public void Resume ()
	{
		pauseMenuUI.SetActive(false);
		Time.timeScale = 1f;
		GameIsPaused = false;
	}

	public void Pause ()
	{
		pauseMenuUI.SetActive(true);
		Time.timeScale = 0f;
		GameIsPaused = true;	
	}

	public void LoadMenu() 
	{
		Time.timeScale = 1f;
		Application.LoadLevel("Level Select");
	}

	public void RestartLevel()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	public void QuitGame() 
	{
		Application.Quit();
	}

}
