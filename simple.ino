#include <Adafruit_NeoPixel.h>
#ifdef __AVR__
 #include <avr/power.h>
#endif
#define PIN        7
#define NUMPIXELS 30 
Adafruit_NeoPixel pixels(NUMPIXELS, PIN, NEO_GRB + NEO_KHZ800);

#define DELAYVAL 500
#define BUFLENGHT 1
byte buf[BUFLENGHT];

void setup() 
{
  Serial.begin(9600);
#if defined(__AVR_ATtiny85__) && (F_CPU == 16000000)
  clock_prescale_set(clock_div_1);
#endif
  pixels.begin(); 
}

void loop() 
{
  if(Serial.available())
  {
    Serial.readBytes(buf, BUFLENGHT);
    if (buf[0] == 1)
    {
      for(int i=0; i<NUMPIXELS; i++) 
       { 
         pixels.setPixelColor(i, pixels.Color(0, 250, 0));
         pixels.show();  
       }
    }
    else if (buf[0] == 0)
    {
         pixels.clear();
         pixels.show();  
    }
  }
}
