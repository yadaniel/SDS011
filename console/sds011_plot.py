#!c:/Python3/python.exe

import sys, os, re
import time, datetime as dt
import matplotlib.pylab as plt
import pdb

if len(sys.argv) != 2:
    print("usage: sds011_plot.py <infile>")
    sys.exit(1)

infile = sys.argv[1]

if not os.path.exists(infile):
    print(f"'{infile}' does not exist")
    sys.exit(2)

ts = []
pm25 = []
pm10 = []

# print(f"[{dt.datetime.now()}]: pm25 = {aqi.pm25:5}, pm10 = {aqi.pm10:5}", flush=True)

pattern = re.compile(r"^\[(?P<timestamp>.*?)\]: pm25 = (?P<pm25>.*?), pm10 = (?P<pm10>.*?)$")


with open(infile) as f:
    for line in f.readlines():
        if line.strip() == "":
            continue

        # print(line)

        if m := pattern.match(line):
            timestamp = m.group("timestamp")
            # ts.append(dt.datetime.strptime(timestamp, "%Y-%m-%d %H:%M:%S.%f"))    # 2024-10-10 20:54:34.212638
            # ts.append(dt.datetime.strptime(timestamp, "%Y-%m-%d %H:%M:%S.%f%z"))  # 2024-10-10 20:54:34.212638+02:00

            try:
                timestamp = dt.datetime.strptime(timestamp, "%Y-%m-%d %H:%M:%S.%f")
            except ValueError as ex:
                try:
                    timestamp = dt.datetime.strptime(timestamp, "%Y-%m-%d %H:%M:%S.%f%z")
                except ValueError as ex:
                    print(f"timestamp '{timestamp}' could not be parsed")
                    sys.exit(3)

            ts.append(timestamp)
            pm25.append(float(m.group("pm25")))
            pm10.append(float(m.group("pm10")))
        else:
            print(f"could not parse '{line}'")

# pdb.set_trace()
# sys.exit()

plt.plot(ts, pm25, 'r*-', ts, pm10, 'b*-')
plt.legend(["pm25", "pm10"])
plt.grid()
plt.show()

