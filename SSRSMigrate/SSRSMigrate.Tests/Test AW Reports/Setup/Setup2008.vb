'
' File: Setup.rss
' Example usage:
'
'	rs -i Setup.rss -s http://localhost:8080/ReportServer
'

Public Sub Main()
    ' Create folder structure
    CreateFolder("SSRSMigrate_AW_Tests", "/")
    CreateFolder("Reports", "/SSRSMigrate_AW_Tests")
    CreateFolder("Sub Folder","/SSRSMigrate_AW_Tests/Reports")
    CreateFolder("Data Sources", "/SSRSMigrate_AW_Tests")
    
    ' Create data sources
    CreateDataSource("AWDataSource", "/SSRSMigrate_AW_Tests/Data Sources", "Data Source=(local)\SQL2008;Initial Catalog=AdventureWorks2008")
    CreateDataSource("Test Data Source", "/SSRSMigrate_AW_Tests/Data Sources", "Data Source=(local)\SQL2008;Initial Catalog=AdventureWorks2008")
    
    ' Load reports from disk
    Dim companySalesDef As [Byte]() = FileToByteArray("..\2005\Company Sales.rdl")
    Dim salesOrderDetailsDef As [Byte]() = FileToByteArray("..\2005\Sales Order Detail.rdl")
    Dim storeContactsDef As [Byte]() = FileToByteArray("..\2005\Store Contacts.rdl")
    
    ' Upload reports
    UploadReport("Company Sales", "/SSRSMigrate_AW_Tests/Reports", companySalesDef)
    UploadReport("Sales Order Detail", "/SSRSMigrate_AW_Tests/Reports", salesOrderDetailsDef)
    UploadReport("Store Contacts", "/SSRSMigrate_AW_Tests/Reports", storeContactsDef)
End Sub

Public Function FileToByteArray(ByVal fileName As String) As [Byte]()
    Try
        Dim stream As FileStream = File.OpenRead(fileName)
        
        Dim definition = New [Byte](stream.Length - 1) {}
        stream.Read(definition, 0, CInt(stream.Length))
        stream.Close()
        
        Return definition
    Catch e As IOException
        Console.WriteLine(e.Message)
    End Try
End Function

Public Sub CreateFolder(ByVal folderName As String, ByVal parentPath As String)
    Try
        rs.CreateFolder(folderName, parentPath, Nothing)
        Console.WriteLine("Folder: {0}/{1} created.", parentPath, folderName)
    Catch e As Exception
        Console.WriteLine("Folder Error: {0}", e.Message)
    End Try
End Sub

Public Sub UploadReport(ByVal reportName As String, ByVal parentPath As String, ByVal reportDefinition As [Byte]())
    Try
       rs.CreateReport(reportName, parentPath, True, reportDefinition, Nothing)
       Console.WriteLine("Report: {0}/{1} created.", parentPath, reportName)
    Catch e As Exception
        Console.WriteLine("Report Error: {0}", e.Message)
    End Try
End Sub

Public Sub CreateDataSource(ByVal name As String, ByVal parentPath As String, ByVal connectionString As String)
    Dim definition As New DataSourceDefinition()
    
    definition.CredentialRetrieval = CredentialRetrievalEnum.Integrated
    definition.ConnectString = connectionString
    definition.Enabled = True
    definition.EnabledSpecified = True
    definition.ImpersonateUser = False
    definition.ImpersonateUserSpecified = True
    definition.WindowsCredentials = False
    definition.Extension = "SQL"
    
    Try
        rs.CreateDataSource(name, parentPath, False, definition, Nothing)
    
        Console.WriteLine("Data Source: {0}/{1} created.", parentPath, name)
    Catch e As Exception
        Console.WriteLine("Data Source Error: {0}", e.Message)
    End Try
End Sub