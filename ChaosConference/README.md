# Chaos Conference

Simple Flask application for handling conference calls.


## Getting started

You need to have

    - Git
    - .Net Framework 4.5
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

Fill app settings (you can find them in web.config.sample bellow comment `Fill settings here`)

Click right button on project "ChaosConference" in Visual Studio and select menu item "Publish"

![](/images/select-target.png)

Select target "Microsoft Azure Website", sign in on Azure (if need).

![](/images/select-site.png)

Press button "New" to create new site on azure.

![](/images/new-site.png)

Fill site's options and press button "Create".

Press "Publish" to deploy this app to created site.

After publishing browser with site will be started. Please check if it shows text "This app is ready to use"

![](/images/ready.png)

## Demo

Start incoming call from command line:

```console
curl -d '{"to": "+YOUR-NUMBER"}' http://YOUR-AZURE-DOMAIN/start/demo --header "Content-Type:application/json"
```
