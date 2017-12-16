# musicLED
This is a Windows forms application that reads audio data generated by the computer (aka. what your PC outputs) and sends it to a serial port. On the other side I have an Arduino Leonardo that reads the RGB values and outputs them onto an addressable LED strip

To test it out you can just build it, flash your arduino with something like: [Example sketch](musicLED_Arduino/musicLED_Arduino.ino)

Note that this example Arduino sketch requires the [FastLED library](https://github.com/FastLED/FastLED)

If you have any questions, suggestions etc. email me at: janleskovec.dev@gmail.com
