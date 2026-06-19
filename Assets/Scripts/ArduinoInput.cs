using UnityEngine;
using System.IO.Ports;

public class ArduinoInput : MonoBehaviour
{
    public string portName = "COM3";
    public int baudRate = 9600;

    private SerialPort serialPort;
    private PlayerMovement playerMovement;

    private ShopManager shopManager;
    private bool buttonWasPressed = false;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        shopManager = FindFirstObjectByType<ShopManager>();

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

            // NEU: Button öffnet/schließt den Shop nur einmal pro Klick.
            if (input == "BTN")
            {
                if (!buttonWasPressed)
                {
                    if (shopManager != null)
                    {
                        shopManager.ToggleShop();
                    }

                    buttonWasPressed = true;
                }

                return;
            }
            else
            {
                buttonWasPressed = false;
            }

            Vector2 movement = Vector2.zero;

            if (input == "UP") movement = Vector2.up;
            else if (input == "DOWN") movement = Vector2.down;
            else if (input == "LEFT") movement = Vector2.left;
            else if (input == "RIGHT") movement = Vector2.right;
            else if (input == "UP_LEFT") movement = new Vector2(-1, 1).normalized;
            else if (input == "UP_RIGHT") movement = new Vector2(1, 1).normalized;
            else if (input == "DOWN_LEFT") movement = new Vector2(-1, -1).normalized;
            else if (input == "DOWN_RIGHT") movement = new Vector2(1, -1).normalized;

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
            serialPort.WriteLine("LED:0"); // NEU: LEDs beim Stoppen ausschalten
            serialPort.Close();
        }
    }

        void OnDisable()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.WriteLine("LED:0");
        }
    }

    public void SendLedCount(int count)
    {
        if (serialPort == null || !serialPort.IsOpen)
            return;

        count = Mathf.Clamp(count, 0, 5);
        serialPort.WriteLine("LED:" + count);
    }
}