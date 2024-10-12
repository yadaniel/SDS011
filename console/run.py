#!c:/Python3/python.exe

from sds011lib import SDS011QueryReader
from serial import Serial
import time, pytz, datetime as dt
import sys, os
import atexit

match len(sys.argv):
    case 1:
        from serial.tools.list_ports import comports
        print("usage: sds011.py <com> <outfile>")
        ports = [port.device for port in comports()]
        print(f"available ports => {ports}")
        sys.exit(1)
    case 2:
        print("usage: sds011.py <com> <outfile>")
        sys.exit(1)

COM = sys.argv[1]
OUTFILE = sys.argv[2]

if os.path.exists(OUTFILE):
    print(f"'{OUTFILE}' already exists")
    sys.exit(2)

try:
    sensor = SDS011QueryReader(COM)
except Exception as ex:
    print(f"'{COM}' could not be opened")
    print(f"exception => {ex}")
    sys.exit(3)

def shutdown():
    print("sensor shutdown\n", flush=True)
    sensor.sleep()

atexit.register(shutdown)

pm25 = []
pm10 = []

tz = pytz.timezone("Europe/Berlin")

while True:
    sensor.wake()
    time.sleep(5)

    aqi = sensor.query()
    pm25.append(aqi.pm25)
    pm10.append(aqi.pm10)
    
    # timestamp = dt.datetime.now()                     # 2024-10-10 20:54:34.212638
    # timestamp = tz.localize(dt.datetime.now())        # 2024-10-10 20:54:34.212638+02:00
    timestamp = dt.datetime.now(tz)                     # 2024-10-10 20:54:34.212638+02:00
    line = f"[{timestamp}]: pm25 = {aqi.pm25:5}, pm10 = {aqi.pm10:5}"
    print(f"{line}")

    with open(OUTFILE, "a") as outfile:
        outfile.write(f"{line}\n")

    sensor.sleep()
    time.sleep(5)


