#include <TimerOne.h>

#include <hapkit3.h>


struct Packet
{
  float force;
  float position;
  float velocity;
  float acceleration;
};
#define RECVBUFSIZE 512
char recvBuf[RECVBUFSIZE] = { '\0' };
int  recvBytes = 0;
bool string_complete = false;
TimerOne timer_tck;
TimerOne send_timer;
Hapkit* hapkit = NULL;
static FILE uartout = {0};
Packet *packet = new Packet();

void hapticLoop()
{
  hapkit->getSensor()->readSensor();

  if (hapkit->isCalibrated())
  {
    hapkit->update();

    hapkit->setForce(packet->force);
    packet->position = hapkit->getPosition();
    packet->velocity = hapkit->getVelocity();
    packet->acceleration = hapkit->getAcceleration();
  }
}

#define BAUD_RATE 2000000
#define BAUD_RATE2 57600

void setup()
{
	Serial.begin(BAUD_RATE2);
 send_timer.initialize(100000);
  send_timer.attachInterrupt(snd);
 #if 0
  hapkit = new Hapkit(HAPKIT_YELLOW, 2, A2);
  hapkit->setUpdateRate(300.0); // 500 Hz

  timer_tck.initialize(1000000 / hapkit->getUpdateRate());
  timer_tck.attachInterrupt(hapticLoop);

  

  hapkit->calibrate();
  #endif
  packet->force = 1.2224421412141242131231f;
  packet->position = 12.242422141412412412421f;
  packet->velocity = 1.121242142174124210242;
  packet->acceleration = 1337.1337f;

}

void serialEvent() {
  while (Serial.available() > 0) {
    char chr = Serial.read();
    if (chr == '\n') {
      recvBuf[recvBytes++] = '\0';
      string_complete = true;
      continue;
    }
    recvBuf[recvBytes++] = chr;
  }
}

void loop() {
  // recv();
 if (string_complete) {
   parseMsg();
 }
 
 


}


void snd() {
  Serial.print("Prm ");
  Serial.print(packet->force, 5);
 #if 0
  Serial.print(" ");
  Serial.print(packet->velocity, 5);
  Serial.print(" ");
  Serial.print(packet->acceleration, 5);
  #endif
  Serial.print("\n");
}

void snd_ack() {
  Serial.print("Ok");
  
  Serial.print("\n");
}

void parseMsg() {
  
  recvBytes = 0;
  string_complete = false;
  

  if (recvBuf[0] == 'P' && recvBuf[1] == '1')
  {
    
    char *header = strtok(recvBuf, " ");
    char *param1 = strtok(NULL, " ");
    
    packet->force = atof(param1);
    
    //snd_ack();
    Serial.print("Ok\n");
    //Serial.print(packet->force, 5);
    //Serial.print("\n");
  }
  else
  {
    //Serial.println("Unknown message");
  } 

  
    //Serial.print("Ok\n");

}



static int uart_putchar (char c, FILE *stream)
{
    Serial.write(c);
    return 0;
}

const hapkit_effect_t potential_well[] = {
  {
    .position = -0.04,
    .width = 0.003,
    .k_spring = 500.0,
    .k_dumper = 0.7,
  },
  {
    .position = -0.02,
    .width = 0.003,
    .k_spring = 500.0,
    .k_dumper = 0.7,
  },
  {
    .position = 0.0,
    .width = 0.003,
    .k_spring = 1500.0,
    .k_dumper = 0.7,
  },
  {
    .position = 0.02,
    .width = 0.003,
    .k_spring = 500.0,
    .k_dumper = 0.7,
  },
  {
    .position = 0.04,
    .width = 0.003,
    .k_spring = 500.0,
    .k_dumper = 0.7,
  },
};
