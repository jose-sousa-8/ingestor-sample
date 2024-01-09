# User Traffic Ingestion Tool

## Pre Requisites 
- Docker
- Make

## Running the sample

1. in the root folder run ``make sample``
   - This should create once instance of pixel service and one instance of storage service at http://localhost:9999 and http://localhost:8888 respectively
2. GET http://localhost:8888/visits should return the content of the tracking file
3. GET http://localhost:9999/track should trigger a tracking event and return a 1 pixel GIF image
4. GET http://localhost:8888/visits should now have one line per track endpoint call with the respective ip, referer and user-agent headers and timestamp

Ensure the broker container is running since sometimes it may need to be restarted on boot
        
