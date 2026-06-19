const int xPin = A0;
const int yPin = A1;
const int joystickButtonPin = 2;

const int ledPins[5] = {8, 9, 10, 11, 12};

void setup()
{
  Serial.begin(9600);

  pinMode(joystickButtonPin, INPUT_PULLUP);

  for (int i = 0; i < 5; i++)
  {
    pinMode(ledPins[i], OUTPUT);
    digitalWrite(ledPins[i], LOW);
  }
}

void loop()
{
  readUnityCommands();
  sendJoystickInput();

  delay(50);
}

void sendJoystickInput()
{
  int x = analogRead(xPin);
  int y = analogRead(yPin);
  bool buttonPressed = !digitalRead(joystickButtonPin);

  bool up = x < 200;
  bool down = x > 800;
  bool left = y > 800;
  bool right = y < 200;

  if (up && left) Serial.println("UP_LEFT");
  else if (up && right) Serial.println("UP_RIGHT");
  else if (down && left) Serial.println("DOWN_LEFT");
  else if (down && right) Serial.println("DOWN_RIGHT");
  else if (up) Serial.println("UP");
  else if (down) Serial.println("DOWN");
  else if (left) Serial.println("LEFT");
  else if (right) Serial.println("RIGHT");
  else if (buttonPressed) Serial.println("BTN");
  else Serial.println("NONE");
}

void readUnityCommands()
{
  if (Serial.available() > 0)
  {
    String command = Serial.readStringUntil('\n');
    command.trim();

    if (command.startsWith("LED:"))
    {
      int ledCount = command.substring(4).toInt();
      setLedCount(ledCount);
    }
  }
}

void setLedCount(int count)
{
  count = constrain(count, 0, 5);

  for (int i = 0; i < 5; i++)
  {
    if (i < count)
    {
      digitalWrite(ledPins[i], HIGH);
    }
    else
    {
      digitalWrite(ledPins[i], LOW);
    }
  }
}