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

    //hapkit->setForce(packet->force);
    hapkit->setForce(-hapkit->getAcceleration() *0.5);
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
  
 
  hapkit = new Hapkit(HAPKIT_YELLOW, 2, A2);
  hapkit->setUpdateRate(300.0); // 500 Hz


  

  timer_tck.initialize(1000000 / hapkit->getUpdateRate());
  timer_tck.attachInterrupt(hapticLoop);

  //send_timer.initialize(1000000);
  //send_timer.attachInterrupt(snd);

  

  hapkit->calibrate();
  
  packet->force = 0.01f;
  packet->position = 0.0f;
  packet->velocity = 0.0f;
  packet->acceleration = 0.0;

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
  snd();
  delay(200);
 if (string_complete) {
   parseMsg();
 }
}


void snd() {
  Serial.print("Prm ");
  Serial.print(packet->position, 5);
  Serial.print(" ");
  Serial.print(packet->velocity, 5);
  Serial.print(" ");
  Serial.print(packet->acceleration, 5);
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
