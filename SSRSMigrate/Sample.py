import clr

class Plugin:
    def __init__(self):
        pass

    def GetConnectionString(self):
        return "Data Source=(local);Initial Catalog=Database;Trusted_Connection=yes;Connect Timeout=0;"

    def OnLoad(self):
        # Called when the script is loaded. This is where you can do stuff like open a connection to a database.
        Engine.LogLine("OnLoad")

        SQLUtil.Initialize(self.GetConnectionString)

    def OnMigration_Start(self, sourcePath, destPath):
        Engine.LogLine("OnMigration_Start - %s" % (sourcePath, ))

        count = SQLUtil.ExecuteScalar[int]("SELECT COUNT(*) FROM Table")

        Engine.LogLine("Count: %s" % (count, ))

    def OnMigration_Completed(self, msg, sourcePath, destPath):
        Engine.LogLine("OnMigration_Completed - %s; migrated from %s to %s" % (msg, sourcePath, destPath))

    def OnMigration_FolderItem(self, item, status):
        Engine.LogLine("OnMigration_FolderItem - %s" % (item.Path, ))

    def OnMigration_DataSourceItem(self, item, status):
        Engine.LogLine("OnMigration_DataSourceItem - %s" % (item.Path, ))

    def OnMigration_ReportItem(self, item, status):
        Engine.LogLine("OnMigration_ReportItem - %s" % (item.Path, ))

        SQLUtil.ExecuteNonQuery("UPDATE Table SET Path='%s' WHERE Path='%s'" % (source.ToPath, source.FromPath))