#!/cygdrive/c/Python27/python
# -*- coding: utf-8 -*-

import sys, serial, struct, time, atexit, argparse

parser = argparse.ArgumentParser(description="SDS011 PM2.5 PM10 measurement")
parser.add_argument("com", help="com port")
parser.add_argument("--cont", action="store_true", help="cont. mode")
args = parser.parse_args()

try:
    ser = serial.Serial(port = args.com, baudrate = 9600, timeout = 0.25)
    ser.flushInput()
    atexit.register(lambda: [sys.stdout.write("COM closing"), ser.close()])
except Exception as exp:
    print(exp)
    sys.exit()

def sensor_wake():
    bs = [
        b'\xaa', #head
        b'\xb4', #command 1
        b'\x06', #data byte 1
        b'\x01', #data byte 2 (set mode)
        b'\x01', #data byte 3 (sleep)
        b'\x00', #data byte 4
        b'\x00', #data byte 5
        b'\x00', #data byte 6
        b'\x00', #data byte 7
        b'\x00', #data byte 8
        b'\x00', #data byte 9
        b'\x00', #data byte 10
        b'\x00', #data byte 11
        b'\x00', #data byte 12
        b'\x00', #data byte 13
        b'\xff', #data byte 14 (device id byte 1)
        b'\xff', #data byte 15 (device id byte 2)
        b'\x06', #checksum
        b'\xab'] #tail
    for b in bs:
        ser.write(b)

def process_frame(d):
    r = struct.unpack('<HHxxBB', d[2:]) # < = little endian, H = unsigned short, x = pad byte, B = unsigned char
    pm25 = r[0]/10.0
    pm10 = r[1]/10.0
    checksum = sum(ord(v) for v in d[2:8]) % 0xFF
    print("PM 2.5: {:3} ug/m^3  PM 10: {:3} ug/m^3 CRC={:3}".format(pm25, pm10, "OK" if (checksum==r[2] and r[3]==0xAB) else "NOK"))
    sys.stdout.flush()
    return pm25, pm10

pm25_max, pm10_max = 0, 0
def process_frame_stars(d):
    global pm25_max, pm10_max
    r = struct.unpack('<HHxxBB', d[2:]) # < = little endian, H = unsigned short, x = pad byte, B = unsigned char
    pm25 = r[0]
    pm10 = r[1]
    pm25_max = max(pm25, pm25_max)
    pm10_max = max(pm10, pm10_max)
    checksum = sum(ord(v) for v in d[2:8])%256
    if checksum == r[2] and r[3] == 0xAB:
        linewidth = 130
        spaces = linewidth-pm25-pm10-17-2
        line = "[PM 2.5]%s>%s<%s[PM 10]" % ('.'*pm25, ' '*spaces, '*'*pm10)
        lst = list(line)
        lst[pm25_max+9] = ']'
        lst[linewidth-pm10_max-11] = '['
        line = "".join(lst)
    else:
        line = "???"
    print(line)
    sys.stdout.flush()
    return pm25, pm10

def sensor_read():
    while True:
        byte = 0
        while byte != b"\xaa":
            byte = ser.read(size=1)
        d = ser.read(size=9)
        if d[0] == b"\xc0" and len(d) == 9:
            return (byte + d)

# xAA, 0xB4, 0x06, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x05, 0xAB
def sensor_sleep():
    bs = [
        b'\xaa', #head
        b'\xb4', #command 1
        b'\x06', #data byte 1 (device id)
        b'\x01', #data byte 2 (set mode)
        b'\x00', #data byte 3 (sleep)
        b'\x00', #data byte 4 - reserved
        b'\x00', #data byte 5 - reserved
        b'\x00', #data byte 6 - reserved
        b'\x00', #data byte 7 - reserved
        b'\x00', #data byte 8 - reserved
        b'\x00', #data byte 9 - reserved
        b'\x00', #data byte 10 - reserved
        b'\x00', #data byte 11 - reserved
        b'\x00', #data byte 12 - reserved
        b'\x00', #data byte 13 - reserved
        b'\xff', #data byte 14 (device id byte 1)
        b'\xff', #data byte 15 (device id byte 2)
        b'\x05', #checksum
        b'\xab'] #tail
    for b in bs:
        ser.write(b)

if args.cont:
    sensor_wake()
    while True:
        data = sensor_read()
        process_frame_stars(data)
        time.sleep(1)
else:
    while True:
        sensor_wake()
        time.sleep(10)
        data = sensor_read()
        # process_frame(data)
        process_frame_stars(data)
        sensor_sleep()
        time.sleep(20)

