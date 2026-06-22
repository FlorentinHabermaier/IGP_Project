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

    private float lastShopMoveTime = 0f;
    private float shopMoveCooldown = 0.25f;

    private MainMenu mainMenu;
    private float lastMenuMoveTime = 0f;
    private float menuMoveCooldown = 0.25f;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        shopManager = FindFirstObjectByType<ShopManager>();
        mainMenu = FindFirstObjectByType<MainMenu>();;

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

            if (mainMenu != null)
            {
                if (input == "BTN")
                {
                    if (!buttonWasPressed)
                    {
                        buttonWasPressed = true;
                        mainMenu.PressSelected();
                    }

                    return;
                }
                else
                {
                    buttonWasPressed = false;
                }

                if (Time.unscaledTime - lastMenuMoveTime > menuMoveCooldown)
                {
                    if (input == "DOWN")
                    {
                        mainMenu.SelectNext();
                        lastMenuMoveTime = Time.unscaledTime;
                    }
                    else if (input == "UP")
                    {
                        mainMenu.SelectPrevious();
                        lastMenuMoveTime = Time.unscaledTime;
                    }
                }

                return;
            }

            if (input == "BTN")
            {
                if (!buttonWasPressed)
                {
                    buttonWasPressed = true;

                    if (shopManager != null)
                    {
                        if (shopManager.IsOpen())
                        {
                            shopManager.BuySelected();
                        }
                        else
                        {
                            shopManager.OpenShop();
                        }
                    }
                }

                return;
            }
            else
            {
                buttonWasPressed = false;
            }

            if (shopManager != null && shopManager.IsOpen())
            {
                if (Time.unscaledTime - lastShopMoveTime > shopMoveCooldown)
                {
                    if (input == "RIGHT" || input == "DOWN")
                    {
                        shopManager.SelectNext();
                        lastShopMoveTime = Time.unscaledTime;
                    }
                    else if (input == "LEFT" || input == "UP")
                    {
                        shopManager.SelectPrevious();
                        lastShopMoveTime = Time.unscaledTime;
                    }
                }

                playerMovement.SetMovementInput(Vector2.zero);
                return;
            }

            // Normale Player-Bewegung
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
            serialPort.WriteLine("LED:0"); 
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