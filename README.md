# MBTA-Test

To open solution you will need Visual Studio 2019.
You can download it for free from https://visualstudio.microsoft.com/downloads/

Solution consist of 3 project.

MbtaApiAdapter - Small service which will call MBTA API : https://www.mbta.com/developers/v3-api

MbtaCommon - Data contract and few elements of common functionality

MbtaWebInfoBoard - single page web application which will provide information

MbtaApiAdapter and MbtaWebInfoBoard communication using Pub/Sub over NetMQ (aka ZeroMQ) on local host port 6000.

MbtaWebInfoBoard application can be accessed https://localhost:5001/

You will have first start MbtaApiAdapter service and only after that you can start MbtaWebInfoBoard.

Once you get to web all and select Departure Board menuitem for the first time - it might take few seconds for the app to retrieve data.
Once you build solution.


To start MbtaApiAdapter:

cd .\src\MbtaApiAdapter

dotnet run



To start MbtaWebInfoBoard:

cd .\src\MbtaWebInfoBoard:

dotnet run

