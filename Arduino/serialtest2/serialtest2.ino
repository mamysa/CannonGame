#define RECVBUFSIZE 512
char recvBuf[RECVBUFSIZE] = { '\0' };
int  recvBytes = 0;
bool string_complete = false;

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
 // recv();
 snd();
 if (string_complete) {
  parseMsg();
 }
}

void snd() {
  Serial.print("Prm ");
  Serial.print(String(angle));
  Serial.print(" ");
  Serial.print(String(force));
  Serial.print("\n");
}

void snd_ack() {
  Serial.print("Ok");
  Serial.print("\n");
}

void parseMsg() {
  recvBytes = 0;
  string_complete = false;
  if (recvBuf[0] == 'P' && recvBuf[1] == '1') {
    
    char *header = strtok(recvBuf, " ");
    char *param1 = strtok(NULL, " ");
    char *param2 = strtok(NULL, " ");
    
    int p1 = atoi(param1);
    int p2 = atoi(param2);

    
    
    
    angle = p1;
    force = p2;
    
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


