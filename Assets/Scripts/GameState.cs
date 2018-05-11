using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

public class GameState: MonoBehaviour {

    private GameObject[] players;
    public Text GameOverText;
    private int activePlayer = 0;
    private SerialInputManager manager;

	// Use this for initialization
	void Start () {
        this.players = new GameObject[2];
        this.players[0] = GameObject.Find("CannonP1");
        this.players[1] = GameObject.Find("CannonP2");
        
        this.GameOverText.text = "";

        this.players[0].SendMessage("SetStatus", true);
        this.players[1].SendMessage("SetStatus", false);

        //this.manager = SerialInputManager.Construct("InputMan","/dev/cu.usbmodem1421");
        //this.players[0].GetComponent<Cannon>().inputManager = this.manager;
        //this.players[1].GetComponent<Cannon>().inputManager = this.manager;
	}

	
	// Update is called once per frame
	void Update () {
        CheckGameOverCondition(this.players[0]);
        CheckGameOverCondition(this.players[1]);

        SerialInputManager.WriteMessage(SendMessageType.Reset, 12);
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
