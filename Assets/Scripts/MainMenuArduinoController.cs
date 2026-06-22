using UnityEngine;
using System.IO.Ports;

public class MainMenuArduinoController : MonoBehaviour
{
    public string portName = "COM3";
    public int baudRate = 9600;
    public MainMenu mainMenu;

    private SerialPort serialPort;
    private float lastMoveTime;
    private float moveCooldown = 0.25f;

    void Start()
    {
        if (mainMenu == null)
            mainMenu = FindFirstObjectByType<MainMenu>();

        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 10;
            serialPort.Open();
            serialPort.DiscardInBuffer();

            Debug.Log("Arduino Main Menu verbunden");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Arduino Fehler: " + e.Message);
        }
    }

    void Update()
    {
        if (serialPort == null || !serialPort.IsOpen || mainMenu == null)
            return;

        try
        {
            string input = serialPort.ReadLine().Trim().ToUpper();
            Debug.Log("Arduino sagt: " + input);

            if (Time.time - lastMoveTime < moveCooldown)
                return;

            if (input == "DOWN")
            {
                mainMenu.SelectNext();
                lastMoveTime = Time.time;
            }
            else if (input == "UP")
            {
                mainMenu.SelectPrevious();
                lastMoveTime = Time.time;
            }
            else if (input == "BUTTON" || input == "SELECT" || input == "PRESS" || input == "CLICK" || input == "BTN")
            {
                mainMenu.PressSelected();
                lastMoveTime = Time.time;
            }
        }
        catch (System.TimeoutException)
        {
        }
    }

    void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}