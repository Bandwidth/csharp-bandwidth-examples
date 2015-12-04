## Bandwidth App Platform Example Applications
===

There are two examples of how to use the [Bandwidth C# SDK](https://github.com/bandwidthcom/csharp-bandwidth) and [Bandwidth's App Platform](http://ap.bandwidth.com/?utm_medium=social&utm_source=github&utm_campaign=dtolb&utm_content=) to implement a server application.  

[Hello Flipper](./DolphinApp/README.md) demonstrates how to play audio, speak text to callers, and gather DTMF from the caller.

[Chaos Conference](./ChaosConference/README.md) is a very simple conferencing app that joins users to a conference by making outbound calls to each attendee

[Sip App](./SipApp/README.md) is simple application which allows to make calls directly to sip account, redirect outgoing calls from sip account to another number, redirect incoming calls from specific number to sip account. Also this application demonstrate how to receive/create an application, domain, endpoint, buy phone numbers.

[Sip App](./CallApp/README.md) is simple application which allows to make 2 callback calls and bridge them.

[Transcription App](./TranscriptionApp/README.md) is simple voice mail app which sends email notifications to user with transcripted message text. It demonstrates how to make calls, handle incoming calls to registered number, handle events, tune on call recording, create a transcription for recording. Also it shows how to register an application on catapult and buy new phone number.

[ConferenceWithAgent App](./ConferenceWithAgentApp/README.md) is simple conferencing app wit 2 participants (caller and "agent"). It demonstrates how to make calls, handle incoming calls to registered number, handle events, tune on call recording on creating call, create a conference. Also it shows how to register an application on catapult and buy new phone number.

The README for each project details how to build each project and deploy it to Azure.  Each README also shows how to invoke the demos using simple CURL commands.



