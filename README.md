# AbuseReport

Made for OpenSimulator dotnet8

in ```OpenSim.ini``` at the bottom of the file is fine
```ini
[AbuseReport]
ARModule=AbuseReportModule
Enabled=true
URL=https://yourgrid.whatever/abusereport
```
Data comes in json format as a PUT method

Data that comes in is...
```
Details,
ScreenshotID,
Summary
```
Though screenshot is still being worked on, it is not currently used.
This module allows users to report abuse in the OpenSimulator environment. It is configured through the `OpenSim.ini` file, where you can specify the module name, enable it, and set the URL to which the abuse reports will be sent.
The module will then send the data to the URL specified in the config file.
