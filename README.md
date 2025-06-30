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
RegionName,
AbuserID,
Category,
CheckFlags,
Details,
ObjectID,
Position,
ReportType,
ScreenshotID,
Summary,
Reporter
```
