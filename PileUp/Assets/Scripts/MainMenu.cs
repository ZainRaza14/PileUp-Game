using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour 
{
	public Text scoreText;

	public void Start()
	{
		scoreText.text = PlayerPrefs.GetInt ("score").ToString();
	}

	public void ToGame()
	{
		SceneManager.LoadScene ("Game");
	}



}
