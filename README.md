# GooglePhotos_Download

A simple code to download your GooglePhotos to your FTP Server

## Setup 🛠️

2 JSON are Requiered :  
The First One : A JSON Which contains all your GoogleApi Logs [See Here How To Setup Your Own](https://support.google.com/googleapi/answer/6158862?hl=en)
``` 
[
    {
        "user": "**USERNAME (to retrieve it in the code)**",
        "client_id": "**CLIENT_ID**",
        "client_secret": "**CLIENT_SECRET**"
    },
    {
        ...
    },
    ...
]
``` 

The Second One : A JSON with all Required Informations to the code
```
 {
        "username":"...",
        "monthToGet":"...",
        "yearToGet":"...",
        "ftpServerURL":"...",
        "ftpServerPort":"...",
        "ftpUsername":"...",
        "ftpPasswd":"...",
        "pathToSave":"...",
        "distantPath":"..."
    }
```

⚠️ You can put {username} | {year} | {month} in any path, the code will traduce them with the "username" "year" and "month" given before

## Execution 

```Usage: CLIENT_PHOTO.exe <JSONSetupPath> <JSONGGApiLogs> <JSONImagesPath>```