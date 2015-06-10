# Transcription App

Transcription App is simple voice mail app which sends email notifications to user with transcripted message text. It demonstrates how to make calls, handle incoming calls to registered number, handle events, tune on call recording, create a transcription for recording. Also it shows how to register an application on catapult and buy new phone number.


## Getting started
You need to have
    - Git
    - .Net Framework 4.5
    - MS SQL Server 2012 or 2014
    - Visual Studio 2012 (with nuget) or 2013
    - Bandwidth Application Platform account
    - Azure account
    - Common Azure Tools for Visual Studio


## Build and Deploy

#### Clone the repository

```console
git clone https://github.com/bandwidthcom/csharp-bandwidth-examples
```
Open solution file in Visual Studio and make "Rebuid all"

Fill app settings (their values have large case, like CATAPULT_TOKEN)

Click right button on project "TranscriptionApp" in Visual Studio and select menu item "Publish"

![](/images/select-target.png)

Select target "Microsoft Azure Website", sign in on Azure (if need).

![](/images/select-site.png)

Press button "New" to create new site on azure.

![](/images/new-site.png)

Fill site's options and press button "Create".

Press "Publish" to deploy this app to created site.

After publishing browser with site will be started. Please check if it shows text "This app is ready to use"




## Demo

Open site in web browser and register new user.
First loading of start page can require a lot of time (it will create an application).
