using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : Singleton<GameStateManager>{

	// Use this for initialization
	void Start () {
        SceneManager.LoadScene("myscene");
    }
	
	// Update is called once per frame
	void Update () {
		Debug.Log("Update Gamestatemanager");
	}

    public void doSoemthing(){
        Debug.Log("woo");
    }
}
