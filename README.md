SSRSMigrate
========

Goal of this project is to have a utility that will allow you to easily migrate reports from one Microsoft SQL Reporting Services (SSRS) server to another or from one root folder in SSRS to another folder.

This should make my job and those that I work with easier.

Requirements
-----
This application requires .NET Framework 3.5

Features
-----

#### Server-to-Server migration
This method directly migrates all folders, data sources and report items contained within the specified path on the source server to the specified path on the destination server. 

The path specified for the destination server will contain all items from the path on the source server. For example, if you migrate path '/SourceProject' from the source server to the path '/DestinationProject' on the destination server, everything that is contained within '/SourceProject' will be created on the destination server under '/DestinationProject', so your source report of '/SourceProject/My Report' will now be '/DestinationProject/My Report' on the destination server.

The report and data source definitions that are migrated will be updated to point to the new paths on the destination server during this process.

You cannot migrate items from a newer version of SQL Reporting Services to an older version of SQL Reporting Services.

#### Export to disk
This method exports all folders, data sources and report items contained within the specified path to the directory you set, while maintaining the path structure.

Data source items are serialized to JSON format, folders are exported to the file system as directories and report items are exported in the RDL format.

Currently, you cannot use this method as a form of migration but is more or less intended as a quick way to get a copy of your items for local editing.

#### Export to ZIP archive
This method exports all folders, data sources and report items contained within the specified directory to a ZIP archive that you set. 

The ZIP archive is created using a format specifically used by the 'Import from ZIP archive' method. The archive contains a list of every item exported, and an MD5 check sum of the exported item, that will be used to verify file integrity during the 'Import from ZIP archive' method. The ZIP archive also contains the version of SQL Reporting Services that the archive was created from, in order to prevent importing into an older version of SQL Reporting Services.

#### Import from ZIP archive
This method imports all folders, data sources and report items contained within the specified ZIP archive to the path specified on the destination server.

The path specified for the destination server will contain all items from the archive For example, if the archive was created by exporting the path '/SourceProject' and you import to the path '/DestinationProject' on the destination server, everything that is contained within '/SourceProject' will be created on the destination server under '/DestinationProject', so your source report of '/SourceProject/My Report' will now be '/DestinationProject/My Report' on the destination server.

The report and data source definitions that are migrated will be updated to point to the new paths on the destination server during this process.

Logging
-----
By default, logging is set to an information level, which will not include debug logging messages.

Normal log messages are written to the SSRSMigrate.log file and errors are written to SSRSMigrateErrors.log.

**Enabling debug level logging**


You can enable debug level logging by editing the SSRSMigrate.exe.config file and changing the level value in the 'root' tag to 'DEBUG'.

 Default information level logging:

       <root>
          <level value="INFO" />
          <appender-ref ref="LogFileAppender" />
          <appender-ref ref="ErrorFileAppender" />
        </root>
 

 Change the 'value' attribute to 'DEBUG':
  

      <root>
          <level value="DEBUG" />
          <appender-ref ref="LogFileAppender" />
          <appender-ref ref="ErrorFileAppender" />
        </root>


