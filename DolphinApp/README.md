# Chaos Conference

Simple Flask application for handling calls.


## Getting started
You need to have
    - Git
    - .Net Framework 4.5
    - Visual Studio 2012 (with nuget) or 2013
    - Bandwidth Application Platform account
    - Azure account
    - Common Azure Tools for Visual Studio


## Setup

#### Clone the repository

```console
git clone https://github.com/bandwidthcom/csharp-bandwidth-examples
```
Open solution file in Visual Studio and make "Rebuid all"

Create new web site on Azure (if need, .Net 4.5)

Open in browser Azure website dashboard and switch to tab "Configure".

Fill app settings like on the picture. Fill setting `DOMAIN` with azure web site domain.

![](/images/dolphin-app-config.png)

Press button "Save" to apply changes.

## Deploy

Click right button on project "DolphinApp" in Visual Studio and select menu item "Publish"

Select target "Microsoft Azure Website", sign in on Azure (if need) and select site on Azure where this project will be hosted.

![](/images/select-target.png)

![](/images/select-site.png)

Press "Publish"

After publishing browser with site will be started. Please check if it shows text "This app is ready to use"

![](/images/ready.png)


## Demo

Start incoming call from command line:

```console
curl -d '{"to": "+YOUR-NUMBER"}' http://YOUR-AZURE-DOMAIN/start/demo --header "Content-Type:application/json"
```
