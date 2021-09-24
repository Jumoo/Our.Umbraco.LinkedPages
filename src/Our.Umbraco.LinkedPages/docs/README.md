# About 

Linked pages, lets you quickly and eaisly examine and mange the relationships between pages inside Umbraco. 

Linked pages adds a new "Linked Pages" item to the context menu of items in the Content tree.

From here you can quickly and easily see the related items for any page. 

![Linked pages dialog](https://raw.githubusercontent.com/KevinJump/Our.Umbraco.LinkedPages/dev/multi-target/screenshots/LinksDialog.PNG)


# Config 

you can control what relationships are displayed in the linked pages dialog via settings in web.config / app settings 


### Web.Config 

```
<appSettings>
   ...
   <add key="LinkedPages:RelationType" value="[relationtypestoshow]" />
   <add key="LinkedPages:ShowType" value="true/false" />
   <add key="LinkedPaged:Ignore" value="umbMedia,umbDocument" />
</appSettings>
```
    
### appsettings.json

```
{
    "LinkedPages" : {
        "RelationType" : "",
        "ShowType" : false,
        "Ignore": "umbMedia,umbDocument"
    }
}
```