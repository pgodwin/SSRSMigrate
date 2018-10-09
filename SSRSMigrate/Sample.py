import clr

class Plugin:
    def __init__(self):
        pass

    def OnSourceRefresh_Start(self, sourcePath):
        Engine.LogLine("OnSourceRefresh_Start - %s" % (sourcePath, ))

    def OnSourceRefresh_StartError(self, er, msg):
        Engine.LogLine("OnSourceRefresh_StartError - %s" % (msg, ))

    def OnSourceRefresh_CompletedCancelled(self, msg):
        Engine.LogLine("OnSourceRefresh_CompletedCancelled - %s" % (msg, ))

    def OnSourceRefresh_CompletedError(self, er, msg):
        Engine.LogLine("OnSourceRefresh_CompletedError - %s" % (msg, ))

    def OnSourceRefresh_Completed(self, count):
        Engine.LogLine("OnSourceRefresh_Completed - %s" % (count, ))

    def OnSourceRefresh_FolderFound(self, item):
        Engine.LogLine("OnSourceRefresh_FolderFound - %s" % (item.Path, ))

    def OnSourceRefresh_DataSourceFound(self, item):
        Engine.LogLine("OnSourceRefresh_DataSourceFound - %s" % (item.Path, ))

    def OnSourceRefresh_ReportFound(self, item):
        Engine.LogLine("OnSourceRefresh_ReportFound - %s" % (item.Path, ))

    def OnMigration_Start(self, sourcePath, destPath):
        Engine.LogLine("OnMigration_Start - %s" % (sourcePath, ))

    def OnMigration_Completed(self, msg):
        Engine.LogLine("OnMigration_Completed - %s" % (msg, ))

    def OnMigration_StartError(self, er, msg):
        Engine.LogLine("OnMigration_StartError - %s" % (msg, ))

    def OnMigration_FolderItem(self, item, status):
        Engine.LogLine("OnMigration_FolderItem - %s" % (item.Path, ))

    def OnMigration_DataSourceItem(self, item, status):
        Engine.LogLine("OnMigration_DataSourceItem - %s" % (item.Path, ))

    def OnMigration_ReportItem(self, item, status):
        Engine.LogLine("OnMigration_ReportItem - %s" % (item.Path, ))