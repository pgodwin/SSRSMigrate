import clr

class Plugin:
    def __init__(self):
        pass

    # SQLUtil requires that a delegate that returns the SQL Server's connection string is provided to it's Initialize method. 
    # This is the method that we will set that delegate to.
    def GetConnectionString(self):
        return "Data Source=(local);Initial Catalog=Database;Trusted_Connection=yes;Connect Timeout=0;"

    def OnLoad(self):
        # Called when the script is loaded. This is where you can do stuff like open a connection to a database.
        Logger.Info("OnLoad")

        SQLUtil.Initialize(self.GetConnectionString)

    def OnMigration_Start(self, sourcePath, destPath):
        Logger.Info("OnMigration_Start - %s" % (sourcePath, ))

        # ExecuteScalar is a generic method, so you need to provide the return type
        count = SQLUtil.ExecuteScalar[int]("SELECT COUNT(*) FROM Table")

        # String example:
        mystring = SQLUtil.ExecuteScalar[string]("SELECT TOP 1 CAST(MyColumn AS VARCHAR(50)) FROM Table")

        Logger.Info("Count: %s" % (count, ))

    def OnMigration_Completed(self, msg, sourcePath, destPath):
        Logger.Info("OnMigration_Completed - %s; migrated from %s to %s" % (msg, sourcePath, destPath))

    def OnMigration_FolderItem(self, item, status):
        Logger.Info("OnMigration_FolderItem - %s" % (item.Path, ))

    def OnMigration_DataSourceItem(self, item, status):
        Logger.Info("OnMigration_DataSourceItem - %s" % (item.Path, ))

    def OnMigration_ReportItem(self, item, status):
        Logger.Info("OnMigration_ReportItem - %s" % (item.Path, ))

        SQLUtil.ExecuteNonQuery("UPDATE Table SET Path='%s' WHERE Path='%s'" % (source.ToPath, source.FromPath))
