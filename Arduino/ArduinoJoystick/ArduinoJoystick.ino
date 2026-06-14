const int xPin = A0;
const int yPin = A1;
const int buttonPin = 3;

void setup()
{
  Serial.begin(9600);
  pinMode(buttonPin, INPUT_PULLUP);
}

void loop()
{
  int x = analogRead(xPin);
  int y = analogRead(yPin);
  bool buttonPressed = !digitalRead(buttonPin);

  if (x < 200)
  {
    Serial.println("UP");
  }
  else if (x > 800)
  {
    Serial.println("DOWN");
  }
  else if (y > 800)
  {
    Serial.println("LEFT");
  }
  else if (y < 200)
  {
    Serial.println("RIGHT");
  }
  else if (buttonPressed)
  {
    Serial.println("BTN");
  }
  else
  {
    Serial.println("NONE");
  }

  delay(50);
}