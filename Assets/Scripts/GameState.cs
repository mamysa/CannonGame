using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState: MonoBehaviour {

    private GameObject[] players;
    public Text GameOverText;
    private int activePlayer = 0;

	// Use this for initialization
	void Start () {
        this.players = new GameObject[2];
        this.players[0] = GameObject.Find("CannonP1");
        this.players[1] = GameObject.Find("CannonP2");

        this.GameOverText.text = "";

        this.players[0].SendMessage("SetStatus", true);
        this.players[1].SendMessage("SetStatus", false);
	}
	
	// Update is called once per frame
	void Update () {
        CheckGameOverCondition(this.players[0]);
        CheckGameOverCondition(this.players[1]);
	}

    private IEnumerator OnProjectileFired(string name) {
        yield return new WaitForSeconds(1.0f);
        int nextPlayer = (this.activePlayer + 1) % this.players.Length;
        this.players[this.activePlayer].SendMessage("SetStatus", false);
        this.players[nextPlayer].SendMessage("SetStatus", true);
        this.activePlayer = nextPlayer;
    }

    private void CheckGameOverCondition(GameObject obj) {
        Cannon cannon = obj.GetComponent<Cannon>();
        if (cannon.Health < 0) {
            this.GameOverText.text = "Game Over";            
            this.players[0].SendMessage("SetStatus", false);
            this.players[1].SendMessage("SetStatus", false);
        }
    }
}
