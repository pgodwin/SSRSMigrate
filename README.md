SSRSMigrate
========

Goal of this project is to have a utility that will allow you to easily migrate reports from one Microsoft SQL Reporting Services (SSRS) server to another or from one root folder in SSRS to another folder.

This should make my job and those that I work with easier.

Planned Features
-----

* Export all items under a folder in SSRS to a zip file on disk that can be imported into another SSRS server, in case the destination server is offline.
* Direct migration of the contents of a folder in SSRS to another SSRS server.


Prerequisites
-----

You can get the third party libraries using NuGet.

Moq:
```
Install-Package Moq 
```

Ninject:
```
Install-Package Ninject 
```

DotNetZip:
```
Install-Package DotNetZip 
```

log4net:
```
 Install-Package log4net 
```

Usage
-----

NIL
