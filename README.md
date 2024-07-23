# .NET AisStream MicroService
Background MicroService that runs on AisStream.io to receive current AIS Data for a given List of boats. 
The Data will be stored in a mysql Database for further processing. The Service provides a simple query
mechanism to retrieve the current AIS Data for a given boat by name, a group of boats by a group identifier 
or or a given set of MMSIs. The Service is designed to be used in a larger Microservice Architecture to 
provide AIS Data to the other Services. No Visualisation is included. The Service just monitors the AIS Data 
and updates the last known location in the Database. 

This Service can not be used to track ships historic travel data. The Service is designed to provide only 
the last known location of a monitored ship.

## Releasenotes
* 0.1  Initial Release
* 0.2  Added More Messagetypes and Support for ClassB AIS Data
* 0.3  Store Speed and Course in Database
* 0.4  

## Configuration
The Service is fully configured via Environment Variables. No local Storage will be used and no persistend folders are required. The following Variables are available:
### Environment Variables
* `AIS_API_KEY`=Your API Key for the AisStream.io API
* `WEB_API_KEY`=Your API Key for the Web API
* `DBServer`= The Server where the Database is running
* `DBUser`= The User to connect to the Database
* `DBpassword`= The Password to connect to the Database
* `DBname`= The Name of the Database
* (Optional) `Area`= The Area to query the AIS Data from. Default is *worldwide*. Format: `lat1,lon1,lat2,lon2` *e.g. `55.5,6.2],[53.1,10.1]` would limit the area to roughly the North Sea.*
* (Optional) `Boats`= The List of Boats to receive AIS Data for. Format: `MMSI1,MMSI2,MMSI3,...` *e.g. `211111111,211111112,211111113`*
* (Optional) `Group`= The Name of the Default Group new boats are added to. Default is *ungrouped*.

### Database
The Database is configured for MySQL (*tested on mariaDB 10.6.5*). You need to set up an empty Database and provide the connection data as Environment Variables.

## Warning
If neither `Area` nor `Boats` are defined, the Service will receive a vast amount of data from all ships all over the world. This can be well above 500 datasets per second and 
can significantly impact your server performance and the amount of data to be stored in the Database. You will end up with more than 100k datasets in a few seconds and 
if your Server congests because data can not be processed quick enough aisstream will disqualify your subscription and close the socket very quick!

## Usage
The Service is designed to be used in a larger Microservice Architecture. The Service provides a simple Web API to query the current AIS Data for a given boat or group of boats.
To Request Data the Service provides the following Endpoints:
* GET `/Boat` Returns a Json containing all Boats in the Database.
* POST `/v1/ais/query` to query the AIS Data for a group of boats, an individual boat or a set of MMSIs. The POST body is a JSON:
`{
  "mmsiNumbers": [
    211111111,211111112
  ],
  "shipName": "string",
  "group": "string",
  "apiKey": "string"
}`
*apiKey* is required and must match the `WEB_API_KEY` Environment Variable. The other fields are optional. If no field is provided the Service will return a `NotFound` likewise when no matching Vessels are found.
**Returnvalue** is GeoJson FeatureCollection with the current AIS Data for the requested Vessels.that can be easily used as a map overlay.

## Contribution
This is a private Project and I do not expect any contributions. However if you have any ideas or suggestions or special addons feel free to open an issue. I am happy to 
discuss and improve the Service. If you want to contribute feel free to fork the Project and send me a pull request. I will review and merge if it fits the purpose of the 
Service.

# License
MIT License
### Copyright 2024 Hinnerk Weiler

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

## Achknowledgements
This Project uses the following Libraries or Services:
* [AisStream.io](https://aisstream.io) for the AIS Data. The Service offers an open Websocket to subscribe to realtime live AIS data from all over the world. That is so cool and opens so many possibilities. However the Service is still in beta and let us hope they stay for free ater that!
* https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql for the EntityFramework Core MySQL Provider / Copyright (c) 2017 Pomelo Foundation (MIT License)
* As usual there is a lot of Stackoverflow and other resources that helped me to get this up and running. Thanks to all of you for sharing knowledge and advice!
* I use and can recommend [Caprover](https://caprover.com) for an easy deployment and management of the Services. This is a great tool to deploy and manage Docker Containers on your own Server.