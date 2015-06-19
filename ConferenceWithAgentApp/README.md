# ConferenceWithAgent App
ConferenceWithAgent App is simple conferencing app wit 2 participants (caller and "agent"). It demonstrates how to make calls, handle incoming calls to registered number, handle events, tune on call recording on creating call, create a conference. Also it shows how to register an application on catapult and buy new phone number.


## Getting started
You need to have
    - Git
    - .Net Framework 4.5
    - Visual Studio 2012 (with nuget) or 2013
    - Bandwidth Application Platform account
    - [ngrok](https://ngrok.com/) 


## Build

Clone the repository

```console
git clone https://github.com/bandwidthcom/csharp-bandwidth-examples
```
Go to directory ConferenceWithAgentApp and Open solution file in Visual Studio. Run "Rebuid all"

Run as Administrator `netsh http add urlacl url=http://+:9876/ user=Everyone` to allow to listen to port 9876 by non-admin users.

Run `ngrok http 9876` to setup port forwarding from internet to this app on local machine. Remember url which will be shown by this program (like http://XXXXXXX.ngrok.io)

Fill app settings in section `appSettings` of `app.config`. Use as `baseUrl` remembered ngrok value.

Run this application. 





## Demo

Open `baseUrl` in web browser. You will see phone number for incoming calls. Call to this number. The application will create a conference. Then it will call to "agent" and join both calls to  the conference. 
First start of app can require a lot of time (it will create an application and reserve a phone number).
