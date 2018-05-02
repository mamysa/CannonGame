using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

public class SerialInputManager : MonoBehaviour {
    
    private string port;
    private SerialPort serial;
    
    public static SerialInputManager Construct(string objname, string port){ 
        GameObject obj = new GameObject(objname);
        SerialInputManager im = obj.AddComponent<SerialInputManager>();
        im.port = port;
        return im;
    }

    // on osx - ls /dev/cu* and choose usb modem
	void Start () {
        this.serial = new SerialPort("/dev/cu.usbmodem1421");
        this.serial.RtsEnable = true;
        this.serial.BaudRate = 9600;
        this.serial.ReadTimeout = 1;
        this.serial.Open();
        Debug.Assert(this.serial.IsOpen);
	}
	
	// Update is called once per frame
	void Update () {
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
}
