# One API Meter

### Configuration Sample

`appsettings.Production.json`:  
```
{
  "Email": {
    "SiteName": "",
    "Host": "",
    "User": "user@your.domain",
    "Pwd": "",
    "From": "\"Display Name\" <from@your.domain>",
    "To": []
  },
  "OneApi": {
    "Server": {
      "Url": "https://api.example",
      "AccessToken": ""
    },
    "Statistics": [
      {
        "Name": "Usage",
        "Channels": [ 1, 3 ],
        "MonthlyUsagePerUser": true,
        "LastDayUsagePerUser": true,
        "Mapping": {
          "username of who actually visited": "username of who will be charged"
        }
      }
    ]
  }
}

```