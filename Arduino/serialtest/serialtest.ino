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

void setup()
{
	Serial.begin(2000000);
  int iters = 0;
  while (!Serial)
  {
    iters ++;
  }
  hapkit = new Hapkit(HAPKIT_YELLOW, 2, A2);

  hapkit->setUpdateRate(500.0); // 500 Hz

  timer_tck.initialize(1000000 / hapkit->getUpdateRate());
  timer_tck.attachInterrupt(hapticLoop);

  hapkit->calibrate();
  packet->force = 0.0f;
  packet->position = 0.0f;
  packet->velocity = 0.0f;
  packet->acceleration = 0.0f;
}

void serialEvent() {
  while (Serial.available() > 0) {
    char chr = Serial.read();
    if (chr == '\n') {
      recvBuf[recvBytes++] = '\0';
      string_complete = true;
    }
    recvBuf[recvBytes++] = chr;
  }
}

void loop() {
  // recv();
 snd();
 if (string_complete)
 {
   parseMsg();
 }
}


void snd() {
  Serial.print("Prm ");
  Serial.print(String(packet->position));
  Serial.print(" ");
  Serial.print(String(packet->velocity));
  Serial.print(" ");
  Serial.print(String(packet->acceleration));
  Serial.print("\n");
}

void snd_ack() {
  Serial.print("Ok");
  Serial.print("\n");
}

void parseMsg() {
  recvBytes = 0;
  string_complete = false;
  Packet *packet = new Packet();

  if (recvBuf[0] == 'P' && recvBuf[1] == '1')
  {
    
    char *header = strtok(recvBuf, " ");
    char *param1 = strtok(NULL, " ");
    
    packet->force = atof(param1);
  }
  else
  {
    Serial.println("Unknown message");
  } 

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
