# Sip App

A simple App  which allows to make calls directly to sip account, redirect outgoing calls from sip account to another number, redirect incoming calls from specific number to sip accountl


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

Fill app settings (they are started from ENTER_XXX_HERE)

Click right button on project "SipnApp" in Visual Studio and select menu item "Publish"

![](/images/select-target.png)

Select target "Microsoft Azure Website", sign in on Azure (if need).

![](/images/select-site.png)

Press button "New" to create new site on azure.

![](/images/new-site.png)

Fill site's options and press button "Create".

Press "Publish" to deploy this app to created site.

After publishing browser with site will be started. Please check if it shows text "This app is ready to use"




## Demo

Open site in web browser and follow to instructions on the start page.
First loading of start page can require a lot of time (it will create an application, a domain, an endpoint, 2 phone numbers).
