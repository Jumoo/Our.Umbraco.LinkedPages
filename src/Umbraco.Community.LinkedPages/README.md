# About

Linked pages allows you to quickly and easily examine and manage the relationships between pages inside Umbraco.

Linked pages adds a new "Linked Pages" item to the context menu of items in the content tree.

From here you can quickly and easily see the related items for any page.

![Linked pages dialog](https://raw.githubusercontent.com/Jumoo/Our.Umbraco.LinkedPages/33c07fc06106031fe5d61dd387edd51e43e47c8c/screenshots/LinkedDialog_v18.png)

# Config

You can control which relationships are displayed in the linked pages dialog via settings in the app settings file

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
