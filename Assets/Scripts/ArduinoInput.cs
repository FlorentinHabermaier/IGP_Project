using UnityEngine;
using System.IO.Ports;

public class ArduinoInput : MonoBehaviour
{
    public string portName = "COM3";
    public int baudRate = 9600;

    private SerialPort serialPort;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 50;

        try
        {
            serialPort.Open();
            Debug.Log("Arduino verbunden auf " + portName);
        }
        catch
        {
            Debug.LogWarning("Arduino konnte nicht verbunden werden.");
        }
    }

    void Update()
    {
        if (serialPort == null || !serialPort.IsOpen) return;

        try
        {
            string input = serialPort.ReadLine().Trim();

            Vector2 movement = Vector2.zero;

            if (input == "UP") movement = Vector2.up;
            else if (input == "DOWN") movement = Vector2.down;
            else if (input == "LEFT") movement = Vector2.left;
            else if (input == "RIGHT") movement = Vector2.right;

            playerMovement.SetMovementInput(movement);
        }
        catch
        {
            // nix
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}