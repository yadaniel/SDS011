using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;


public class SDS011 {
	public List<string> commandList;
	public string responseXX { get; private set; } = string.Empty;
	private static SDS011 self;
	public bool threadRunning;
    private Thread thread;
    private string logFilename = "sds011.log";
    public string communicationLog = string.Empty;
    public SerialPort serialPort = new SerialPort();
    public int pm25;
	public int pm10;
	public bool pm_valid;
    
    public void startThread() {
        if (thread == null) {
            commandList = new List<string>();
            //commandList.Add("xx");
            thread = new Thread(runThread);
            thread.Start();
            threadRunning = true;
        }
    }
    
    public void stopThread() {
        if (thread != null) {
            this.Dispose();
        }
        threadRunning = false;
    }
    
    public void runThread() {
        while (true) {
            if (commandList.Count > 0) {
                string nextCommand = popCommand();
                // refill commandQueue
                switch (nextCommand) {
                    case "xx":
                        break;
                    default:
                        break;
                }
                communicationLog = nextCommand + Environment.NewLine + communicationLog;
                DateTime timeStampSend = DateTime.Now, timeStampRecv = DateTime.Now;
                serialPort.Write(string.Format("{0}\r", nextCommand));
                byte[] buffer = new byte[1024];
                string response = string.Empty;
                try {
                	int read = 0;
                	while(read != 1024) {
                		read += serialPort.Read(buffer, read, 1024-read);
                		if((read > 0) && (buffer[read-1] == 0x0D)) {
                			timeStampRecv = DateTime.Now;
							response = Encoding.ASCII.GetString(buffer);
							response = response.Substring(0, response.IndexOf('\0'));
                			break;
                		}
                	}
                }
                catch (TimeoutException) {
					response = "timeout";
                }
                catch(Exception) {
                    response = "exception";
                }
				communicationLog = response + Environment.NewLine + communicationLog;
				
                using(StreamWriter tw = new StreamWriter(logFilename, append:true)) {
                	tw.WriteLine(string.Format("[{0}] => {1}", timeStampSend.ToString(), nextCommand));
                	tw.WriteLine(string.Format("[{0}] <= {1}", timeStampRecv.ToString(), response));
                }
				
                if (response.StartsWith("xx")) {
                    responseXX = response;
                }
				
    		} else {
                communicationLog = "autoread" + Environment.NewLine + communicationLog;
                DateTime timeStampSend = DateTime.Now, timeStampRecv = DateTime.Now;
                byte[] buffer = new byte[10];
                string response = string.Empty;
                try {
                	int read = 0;
                	while(true) {
                		read = serialPort.Read(buffer, offset: 0, count:1);
                		if((read == 1) && (buffer[0] == 0xAA)) {
                			read += serialPort.Read(buffer, offset: 1, count:9);
                		}
                		if((read == 10) && (buffer[1]==0xC0) && (buffer[9]==0xAB)) {
                			int checksum = 0;
                			for(int i=2; i<8; i++) {
                				checksum += buffer[i];
                			}
                			checksum %= 256;
                			if(checksum == buffer[8]) {
                				//MessageBox.Show("checksum ok");
                				pm25 = buffer[3]<<8 | buffer[2];
                				pm10 = buffer[5]<<8 | buffer[4];
                				pm_valid = true;
                				timeStampRecv = DateTime.Now;
                				foreach(byte b in buffer) {
                					switch(b & 0xF0) {
                						case 0x00: response += "0"; break;
                						case 0x10: response += "1"; break;
                						case 0x20: response += "2"; break;
                						case 0x30: response += "3"; break;
                						case 0x40: response += "4"; break;
                						case 0x50: response += "5"; break;
                						case 0x60: response += "6"; break;
                						case 0x70: response += "7"; break;
                						case 0x80: response += "8"; break;
                						case 0x90:  response += "9"; break;
                						case 0xA0: response += "A"; break;
                						case 0xB0: response += "B"; break;
                						case 0xC0: response += "C"; break;
                						case 0xD0: response += "D"; break;
                						case 0xE0: response += "E"; break;
                						case 0xF0: response += "F"; break;
                					}
                					switch(b & 0x0F) {
                						case 0x00: response += "0"; break;
                						case 0x01: response += "1"; break;
                						case 0x02: response += "2"; break;
                						case 0x03: response += "3"; break;
                						case 0x04: response += "4"; break;
                						case 0x05: response += "5"; break;
                						case 0x06: response += "6"; break;
                						case 0x07: response += "7"; break;
                						case 0x08: response += "8"; break;
                						case 0x09:  response += "9"; break;
                						case 0x0A: response += "A"; break;
                						case 0x0B: response += "B"; break;
                						case 0x0C: response += "C"; break;
                						case 0x0D: response += "D"; break;
                						case 0x0E: response += "E"; break;
                						case 0x0F: response += "F"; break;
                					}
                				}
                				break;
                			}
                		}
                	}
                }
                catch (TimeoutException) {
					response = "timeout";
                }
                catch(Exception) {
                    response = "exception";
                }
				//communicationLog = response + Environment.NewLine + communicationLog;
                using(StreamWriter tw = new StreamWriter(logFilename, append:true)) {
                	tw.WriteLine(string.Format("[{0}] <= {1}", timeStampRecv.ToString(), response));
                }
    		}
        }
    }
    
    public static SDS011 getInstance() {
        if (self == null) {
            self = new SDS011();
        }
        return self;
    }

    public void Dispose() {
        thread?.Suspend();
        thread = null;
        if (serialPort.IsOpen) {
            serialPort.Close();
        }
    }
    
    public bool tryConnectCOM(string port) {
        // setup serial port
        serialPort.PortName = port;
        serialPort.ReadTimeout = 1000;
        serialPort.BaudRate = 9600;
        serialPort.DataBits = 8;
        serialPort.StopBits = StopBits.One;
        serialPort.Parity = Parity.None;
        serialPort.Handshake = Handshake.None;
        try {
            serialPort.Open();
        }
        catch {
            return false;
        }
        return true;
    }
    
    public void pushCommandFirst(string command) {
        if (commandList != null) {
            lock (commandList) {
                commandList.Insert(0, command);
            }
        }
    }

    public void pushCommand(string command) {
        if(commandList != null) {
            lock (commandList) {
                commandList.Add(command);
            }
        }
    }

    public string popCommand() {
        string retValue = string.Empty;
        lock (commandList) {
            retValue = commandList.First();
            commandList.RemoveAt(0);
        }
        return retValue;
    }
}
