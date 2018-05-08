using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

public class SerialInputManager : MonoBehaviour {
    
    private string port;
    private SerialPort serial;


    private List<byte> recvBuf = new List<byte>();

    public static SerialInputManager Construct(string objname, string port){ 
        GameObject obj = new GameObject(objname);
        SerialInputManager im = obj.AddComponent<SerialInputManager>();
        im.port = port;
        return im;
    }

    // on osx - ls /dev/cu* and choose usb modem
	void Start () {
        this.serial = new SerialPort(this.port);
        this.serial.RtsEnable = true;
        this.serial.BaudRate = 57600; 
        this.serial.ReadTimeout = 1;
        this.serial.Open();
        Debug.Assert(this.serial.IsOpen);
	}
	
	// Update is called once per frame
	void Update () {
        try {
            while (this.serial.BytesToRead > 0) {
                int chr = this.serial.ReadByte();
                byte by = Convert.ToByte(chr);
                if (by == '\n') {
                    string str = System.Text.Encoding.Default.GetString(this.recvBuf.ToArray());
                    Debug.Log(str);
                    this.recvBuf.Clear();
                    return;
                }
                this.recvBuf.Add(by);
            }
        }
        catch (TimeoutException ex) {
            Debug.Log("Didn't receive anything from Arduino!");
        }
	}

    //  Setup hapkit parameters, whathever they might be!
    public void writeConfigurationPacket(int param1, int param2) {
        string str = String.Format("P1 {0} {1}\n", param1, param2);
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(str);
        this.serial.Write(msg, 0, msg.Length);
    }
}
