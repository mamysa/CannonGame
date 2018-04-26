using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System;

public class GameState: MonoBehaviour {

    private GameObject[] players;
    public Text GameOverText;
    private int activePlayer = 0;


    // todo remove me later!
    private SerialPort serial;

	// Use this for initialization
	void Start () {
        this.players = new GameObject[2];
        this.players[0] = GameObject.Find("CannonP1");
        this.players[1] = GameObject.Find("CannonP2");

        this.GameOverText.text = "";

        this.players[0].SendMessage("SetStatus", true);
        this.players[1].SendMessage("SetStatus", false);

        // on osx - ls /dev/cu* and choose usb modem
        this.serial = new SerialPort("/dev/cu.usbmodem1421");
        this.serial.RtsEnable = true;
        this.serial.BaudRate = 9600;
        this.serial.ReadTimeout = 1;
        this.serial.Open();
        Debug.Assert(this.serial.IsOpen);
	}

    void HandleInput() {
        byte[] write = new byte[1] { 12 };
        this.serial.Write(write, 0, 1);
        try {
            int d = this.serial.ReadByte();
            Debug.Log("Received from arduino " + d);
        }
        catch (TimeoutException ex) {
            Debug.Log("Didn't receive anything from Arduino!");
        }
    }
	
	// Update is called once per frame
	void Update () {
        StartCoroutine("HandleInput");
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
