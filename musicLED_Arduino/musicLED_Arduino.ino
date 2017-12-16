#include <FastLED.h>

#define LED_PIN     3
#define NUM_LEDS    144
#define BRIGHTNESS  200
#define LED_TYPE    WS2811
#define COLOR_ORDER GRB
#define NUM_COLORS 144
CRGB leds[NUM_LEDS];
CRGB colors[NUM_COLORS];
CRGB lastRecievedColorLeft;
CRGB lastRecievedColorRight;

void setup() {
    delay(3000);
    Serial.begin(9600);
    FastLED.addLeds<LED_TYPE, LED_PIN, COLOR_ORDER>(leds, NUM_LEDS).setCorrection( TypicalSMD5050 );
    FastLED.setBrightness(255);
}


void loop() {    
    ReadSerial();
    UpdateLEDs(lastRecievedColorLeft, lastRecievedColorRight);
    FastLED.show();
}

void ReadSerial(){
  if (Serial.available() > 0){
    byte color[6];
    Serial.readBytes(color, 6);
    lastRecievedColorLeft = CRGB(color[0], color[1], color[2]);
    lastRecievedColorRight = CRGB(color[3], color[4], color[5]);
    
    lastRecievedColorLeft.r = (int)(lastRecievedColorLeft.r * (float)(BRIGHTNESS / 255.0f));
    lastRecievedColorLeft.g = (int)(lastRecievedColorLeft.g * (float)(BRIGHTNESS / 255.0f));
    lastRecievedColorLeft.b = (int)(lastRecievedColorLeft.b * (float)(BRIGHTNESS / 255.0f));
    
    lastRecievedColorRight.r = (int)(lastRecievedColorRight.r * (float)(BRIGHTNESS / 255.0f));
    lastRecievedColorRight.g = (int)(lastRecievedColorRight.g * (float)(BRIGHTNESS / 255.0f));
    lastRecievedColorRight.b = (int)(lastRecievedColorRight.b * (float)(BRIGHTNESS / 255.0f));
  }
}

void UpdateLEDs(CRGB colorLeft, CRGB colorRight)
{ 
    for( int i = 0; i < NUM_COLORS / 2; i++) {
      colors[i] = colors[i + 1];
      colors[NUM_COLORS - 1 - i] = colors[NUM_COLORS - 2 - i];
    }
    colors[(NUM_COLORS - 1) / 2] = colorLeft;
    colors[((NUM_COLORS - 1) / 2 + 1)] = colorRight;

    for( int i = 0; i < NUM_LEDS; i++) {
      leds[i] = BlendLEDs(colors, NUM_COLORS, i, NUM_LEDS);
    }
}

CRGB BlendLEDs(CRGB col[], int colNum, int index, int ledNum){
  CRGB output;
  float x = index * ((float)(colNum - 1) / (ledNum - 1));
  float distance = x - (int)(float)x;
  if (distance == 0.0f){
    output = col[(int)x];
  }else{
    output = CRGB(
      (col[(int)x].r * (1 - distance)) + (col[(int)x + 1].r * (distance)),
      (col[(int)x].g * (1 - distance)) + (col[(int)x + 1].g * (distance)),
      (col[(int)x].b * (1 - distance)) + (col[(int)x + 1].b * (distance))
      );
  }
  return output;
}
