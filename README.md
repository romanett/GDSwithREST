# GDSwithREST

This Application is an OPC UA Global Discovery Server for certificate management with a REST-Interface for managing the application.

## OPC UA Features

The Application provides an OPC UA endpoint with the URL: "opc.tcp://localhost:58810/GlobalDiscoveryServer".

At this Endpoint OPC UA Applications which support the Pull Model can register.

For registering OPC UA Applications that support the Push Model a separate OPC UA GDS Client is needed, e g. the one provided by the OPC Foundation as a Windows Application:
https://github.com/OPCFoundation/UA-.NETStandard-Samples/tree/master/Samples/GDS

For registering with the GDS the following credentails can be used:
The sample GDS servers only implement the username/password authentication. The following combinations can be used to connect to the servers:

GDS Administrator:
Username: appadmin, PW: demo

This user has the ability to register and unregister applications and to issue new certificates. It should be used by the GDS Client application to connect.

GDS User:
Username: appuser, PW: demo

This user has only a limited ability to search for applications.

System Administrator:
Username: sysadmin, PW: demo

This user is defined for server push management and has the ability to access the server configuration nodes of the GDS server to update the server certificate and the trust lists.

## REST Features

The applications REST-API can be reached with the following endpoint: "https://localhost:8081/".

The API is documented with swagger so you can find an interactive documentation here: "https://localhost:8081/swagger".

If you want to implement your own REST endpoint the OPEN API JSON File can be found here: "https://localhost:8081/swagger/GDSwithREST API/swagger.json".

## Optional UI
If you want a WEB-UI to interact with the GDS check out my other OPC UA GDS Project GDSwithUI which depends on the GDSwithREST for the Backend.

[GDSwithUI](https://github.com/romanett/GDSwithUI/)

## Deployment

The application is provided as a Docker Container.

The application depends on a Micorosft SQL Server Database to work.

For seamless deployment use the Docker-Compose file to setup the Application + the Database with ease.

## How to switch to local references with fixes instead of the upstream NuGet packages

1. Clone https://github.com/romanett/UA-.NETStandard
2. Clone https://github.com/romanett/GDSwithREST/
3. Switch to branch BetaGDSReference
4. Put both Projects into the same folder
5. Build Project Opc.Ua.Gds.Server.Common
6. Build Project GDSwithREST
7. Run GDSwithREST using docker-compose up

## Disclaimer

This application is a proof of concept.
It is inherently unsecure not meant for use in a productive environment.

In the current implementation severe security concerns apply:
 - the database password is contained in the docker compose file as well as the application.config
 - full access to the db is granted via the SA user
 - the selfsigned server certificate & pw is stored in the git repository
 - the API can be accessed without authentification (see branch authentification_authorization for a reference implementation)
 - No logging is implemented
 - The OPC UA Endpoint uses a hardcoded PW


