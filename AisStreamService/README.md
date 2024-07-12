# .NET AisStream MicroService
Background MicroService that runs on AisStream.io to receive current AIS Data for a given List of boats. 
The Data will be stored in a mysql Database for further processing. The Service provides a simple query
mechanism to retrieve the current AIS Data for a given boat by name, a group of boats by a group identifier 
or or a given set of MMSIs.

## Configuration
### TechStack
* ASP.NET 8.0
* MySQL
* Docker
* Caprover

### Environment Variables
* `AIS_API_KEY`=Your API Key for the AisStream.io API
* `WEB_API_KEY`=Your API Key for the Web API
* `DBServer`= The Server where the Database is running
* `DBUser`= The User to connect to the Database
* `DBpassword`= The Password to connect to the Database
* `DBname`= The Name of the Database
* (Optional) `Area`= The Area to query the AIS Data from. Default is *worldwide*. Format: `[lat1,lon1],[lat2,lon2]` *e.g. `[55,6],[53,10]` would limit the area to roughly the North Sea.*
* (Optional) `Boats`= The List of Boats to receive AIS Data for. Format: `"MMSI1","MMSI2","MMSI3"` *e.g. `"211111111","211111112","211111113"`*
* (Optional) `Group`= The Name of the Default Group new boats are added to. Default is *ungrouped*.

### Database
The Database is a simple MySQL (*tested on mariaDB 10.6.5*) Database. Migrations will be applied on container Startup.

## Warning
If neither `Area` nor `Boats` are set, the Service will receive a vast amount of data from all ships all over the world. This can significantly impact your server performance and the amount of data to be stored in the Database. Also consider bandwidth limits. If your Socket congests because data can not be processed the Server will disqualify your subscription and close the socket.

# License
MIT License
### Copyright 2024 Hinnerk Weiler

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

# Releasenotes
* 0.1   Initial Release
