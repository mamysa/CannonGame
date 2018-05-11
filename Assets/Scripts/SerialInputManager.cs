using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;


// all message types that we can send 
public enum SendMessageType {
    Param1, 
    Param2,
    Reset, 
};

// all message types that we can receive
public enum RecvMessageType {
    Prm, // Current angle and force
    Ack, 
    Nack, 
};

public class SerialInputManager : Singleton<SerialInputManager> {
    
    private string port;
    private SerialPort serial;
    private Queue<byte[]> msgQueue = new Queue<byte[]>();

    private List<byte> recvBuf = new List<byte>();

    // on osx - ls /dev/cu* and choose usb modem
	void Start () {
        #if false
        this.serial = new SerialPort(this.port);
        this.serial.RtsEnable = true;
        this.serial.BaudRate = 57600; 
        this.serial.ReadTimeout = 1;
        this.serial.Open();
        Debug.Assert(this.serial.IsOpen);
        #endif
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Updating input manager");
        #if false
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
        #endif
	}

    public void AddToWriteQueue(byte[] message) {

    }

    //  Setup hapkit parameters, whathever they might be!
    public void writeConfigurationPacket(int param1, int param2) {
        string str = String.Format("P1 {0} {1}\n", param1, param2);
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(str);
        this.serial.Write(msg, 0, msg.Length);
    }

    public static void WriteMessage(SendMessageType type, params object[] param) {
        string str = null;
        if (type == SendMessageType.Reset)  { str = String.Format("{0}\n", type.ToString()); }
        if (type == SendMessageType.Param1) { str = String.Format("{0} {1}\n", type.ToString(), param[0]); }
        if (type == SendMessageType.Param2) { str = String.Format("{0} {1} {2}\n", type.ToString(), param[0], param[1]); }
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(str);
        Instance.AddToWriteQueue(msg);
    }
}
