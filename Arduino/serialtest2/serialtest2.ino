#define RECVBUFSIZE 512
char recvBuf[RECVBUFSIZE] = { '\0' };
int  recvBytes = 0;


int angle = 12;
int force = 1687;
void setup() {
	Serial.begin(57600);
  int iters = 0;
  while (!Serial) {
    iters ++;
  }
}

void loop() {
  recv();
}

void snd() {
  Serial.print("Prm ");
  Serial.print(String(angle));
  Serial.print("\n");
}

void snd_ack() {
  Serial.print("Ok");
  Serial.print("\n");
}

void recv() {
  while (Serial.available() > 0) {
    char chr = Serial.read();
    if (chr == '\n') {
      recvBuf[recvBytes++] = '\0';
      parseMsg();
      recvBytes = 0;
      return;
    }
    recvBuf[recvBytes++] = chr;
  }
}

void parseMsg() {
  if (recvBuf[0] == 'P' && recvBuf[1] == '1') {
    
    char *header = strtok(recvBuf, " ");
    char *param1 = strtok(NULL, " ");
    char *param2 = strtok(NULL, " ");
    
    int p1 = atoi(param1);
    int p2 = atoi(param2);

    Serial.print("Param1 is: ");
    Serial.println(p1);
    Serial.print("Param2 is: ");
    Serial.println(p2);
  }
  else if (recvBuf[0] == 'P' && recvBuf[1] == 'R' && recvBuf[2] == 'M') {
    snd();
  }
  else if (recvBuf[0] == 'R' && recvBuf[1] == 'S') {
    snd_ack();    
  }
  else {
    Serial.println("Unknown message");
  } 
}



