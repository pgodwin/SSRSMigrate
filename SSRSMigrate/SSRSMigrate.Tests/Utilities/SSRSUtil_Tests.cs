﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.Utilities;

namespace SSRSMigrate.Tests.Utilities
{
    [TestFixture]
    class SSRSUtil_Tests
    {
        #region Report Definitions
        private string reportDefinitionForUpdateReportDefExpected = @"<?xml version=""1.0"" encoding=""utf-8""?><Report xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"" xmlns:cl=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/componentdefinition"" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition""><AutoRefresh>0</AutoRefresh><DataSources><DataSource Name=""TestDataSource""><DataSourceReference>/SSRSMigrate_Tests/Test Data Source</DataSourceReference><rd:SecurityType>None</rd:SecurityType><rd:DataSourceID>3c1894f4-2cde-44e5-be42-4bbd3521535a</rd:DataSourceID></DataSource></DataSources><DataSets><DataSet Name=""DataSet1""><Query><DataSourceName>TestDataSource</DataSourceName><CommandText>
          SELECT
          Professionals.FirstName
          ,Professionals.LastName
          ,Professionals.MiddleName
          FROM
          Professionals
        </CommandText><rd:DesignerState><QueryDefinition xmlns=""http://schemas.microsoft.com/ReportingServices/QueryDefinition/Relational""><SelectedColumns><ColumnExpression ColumnOwner=""Professionals"" ColumnName=""FirstName"" /><ColumnExpression ColumnOwner=""Professionals"" ColumnName=""LastName"" /><ColumnExpression ColumnOwner=""Professionals"" ColumnName=""MiddleName"" /></SelectedColumns></QueryDefinition></rd:DesignerState></Query><Fields><Field Name=""FirstName""><DataField>FirstName</DataField><rd:TypeName>System.String</rd:TypeName></Field><Field Name=""LastName""><DataField>LastName</DataField><rd:TypeName>System.String</rd:TypeName></Field><Field Name=""MiddleName""><DataField>MiddleName</DataField><rd:TypeName>System.String</rd:TypeName></Field></Fields></DataSet></DataSets><ReportSections><ReportSection><Body><ReportItems><Tablix Name=""Tablix1""><TablixBody><TablixColumns><TablixColumn><Width>1in</Width></TablixColumn><TablixColumn><Width>1in</Width></TablixColumn></TablixColumns><TablixRows><TablixRow><Height>0.25in</Height><TablixCells><TablixCell><CellContents><Textbox Name=""Textbox2""><CanGrow>true</CanGrow><KeepTogether>true</KeepTogether><Paragraphs><Paragraph><TextRuns><TextRun><Value>First Name</Value><Style><FontFamily>Tahoma</FontFamily><FontSize>11pt</FontSize><FontWeight>Bold</FontWeight><Color>White</Color></Style></TextRun></TextRuns><Style /></Paragraph></Paragraphs><rd:DefaultName>Textbox2</rd:DefaultName><Style><Border><Color>#4e648a</Color><Style>Solid</Style></Border><BackgroundColor>#384c70</BackgroundColor><PaddingLeft>2pt</PaddingLeft><PaddingRight>2pt</PaddingRight><PaddingTop>2pt</PaddingTop><PaddingBottom>2pt</PaddingBottom></Style></Textbox></CellContents></TablixCell><TablixCell><CellContents><Textbox Name=""Textbox3""><CanGrow>true</CanGrow><KeepTogether>true</KeepTogether><Paragraphs><Paragraph><TextRuns><TextRun><Value>Last Name</Value><Style><FontFamily>Tahoma</FontFamily><FontSize>11pt</FontSize><FontWeight>Bold</FontWeight><Color>White</Color></Style></TextRun></TextRuns><Style /></Paragraph></Paragraphs><rd:DefaultName>Textbox3</rd:DefaultName><Style><Border><Color>#4e648a</Color><Style>Solid</Style></Border><BackgroundColor>#384c70</BackgroundColor><PaddingLeft>2pt</PaddingLeft><PaddingRight>2pt</PaddingRight><PaddingTop>2pt</PaddingTop><PaddingBottom>2pt</PaddingBottom></Style></Textbox></CellContents></TablixCell></TablixCells></TablixRow><TablixRow><Height>0.25in</Height><TablixCells><TablixCell><CellContents><Textbox Name=""FirstName""><CanGrow>true</CanGrow><KeepTogether>true</KeepTogether><Paragraphs><Paragraph><TextRuns><TextRun><Value>=Fields!FirstName.Value</Value><Style><FontFamily>Tahoma</FontFamily><Color>#4d4d4d</Color></Style></TextRun></TextRuns><Style /></Paragraph></Paragraphs><rd:DefaultName>FirstName</rd:DefaultName><Style><Border><Color>#e5e5e5</Color><Style>Solid</Style></Border><PaddingLeft>2pt</PaddingLeft><PaddingRight>2pt</PaddingRight><PaddingTop>2pt</PaddingTop><PaddingBottom>2pt</PaddingBottom></Style></Textbox></CellContents></TablixCell><TablixCell><CellContents><Textbox Name=""LastName""><CanGrow>true</CanGrow><KeepTogether>true</KeepTogether><Paragraphs><Paragraph><TextRuns><TextRun><Value>=Fields!LastName.Value</Value><Style><FontFamily>Tahoma</FontFamily><Color>#4d4d4d</Color></Style></TextRun></TextRuns><Style /></Paragraph></Paragraphs><rd:DefaultName>LastName</rd:DefaultName><Style><Border><Color>#e5e5e5</Color><Style>Solid</Style></Border><PaddingLeft>2pt</PaddingLeft><PaddingRight>2pt</PaddingRight><PaddingTop>2pt</PaddingTop><PaddingBottom>2pt</PaddingBottom></Style></Textbox></CellContents></TablixCell></TablixCells></TablixRow></TablixRows></TablixBody><TablixColumnHierarchy><TablixMembers><TablixMember /><TablixMember /></TablixMembers></TablixColumnHierarchy><TablixRowHierarchy><TablixMembers><TablixMember><KeepWithGroup>After</KeepWithGroup></TablixMember><TablixMember><Group Name=""Details"" /></TablixMember></TablixMembers></TablixRowHierarchy><DataSetName>DataSet1</DataSetName><Top>0.4in</Top><Height>0.5in</Height><Width>2in</Width><Style><Border><Style>None</Style></Border></Style></Tablix><Textbox Name=""ReportTitle""><CanGrow>true</CanGrow><KeepTogether>true</KeepTogether><Paragraphs><Paragraph><TextRuns><TextRun><Value>Test Report</Value><Style><FontFamily>Verdana</FontFamily><FontSize>20pt</FontSize></Style></TextRun></TextRuns><Style /></Paragraph></Paragraphs><rd:WatermarkTextbox>Title</rd:WatermarkTextbox><rd:DefaultName>ReportTitle</rd:DefaultName><Height>0.4in</Height><Width>5.5in</Width><ZIndex>1</ZIndex><Style><Border><Style>None</Style></Border><PaddingLeft>2pt</PaddingLeft><PaddingRight>2pt</PaddingRight><PaddingTop>2pt</PaddingTop><PaddingBottom>2pt</PaddingBottom></Style></Textbox></ReportItems><Height>2.25in</Height><Style><Border><Style>None</Style></Border></Style></Body><Width>6in</Width><Page><PageFooter><Height>0.45in</Height><PrintOnFirstPage>true</PrintOnFirstPage><PrintOnLastPage>true</PrintOnLastPage><ReportItems><Textbox Name=""ExecutionTime""><CanGrow>true</CanGrow><KeepTogether>true</KeepTogether><Paragraphs><Paragraph><TextRuns><TextRun><Value>=Globals!ExecutionTime</Value><Style /></TextRun></TextRuns><Style><TextAlign>Right</TextAlign></Style></Paragraph></Paragraphs><rd:DefaultName>ExecutionTime</rd:DefaultName><Top>0.2in</Top><Left>4in</Left><Height>0.25in</Height><Width>2in</Width><Style><Border><Style>None</Style></Border><PaddingLeft>2pt</PaddingLeft><PaddingRight>2pt</PaddingRight><PaddingTop>2pt</PaddingTop><PaddingBottom>2pt</PaddingBottom></Style></Textbox></ReportItems><Style><Border><Style>None</Style></Border></Style></PageFooter><LeftMargin>1in</LeftMargin><RightMargin>1in</RightMargin><TopMargin>1in</TopMargin><BottomMargin>1in</BottomMargin><Style /></Page></ReportSection></ReportSections><rd:ReportUnitType>Inch</rd:ReportUnitType><rd:ReportServerUrl>http://localhost/ReportServer_NEWSERVER</rd:ReportServerUrl><rd:ReportID>62eb8402-88fd-4b5b-82d7-59907f1518ef</rd:ReportID></Report>";

        private string reportDefinitionForUpdateReportDefArgument = @"<?xml version=""1.0"" encoding=""utf-8""?><Report xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"" xmlns:cl=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/componentdefinition"" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition""><AutoRefresh>0</AutoRefresh><DataSources><DataSource Name=""TestDataSource""><DataSourceReference>/SSRSMigrate_Tests/Test Data Source</DataSourceReference><rd:SecurityType>None</rd:SecurityType><rd:DataSourceID>3c1894f4-2cde-44e5-be42-4bbd3521535a</rd:DataSourceID></DataSource></DataSources><DataSets><DataSet Name=""DataSet1""><Query><DataSourceName>TestDataSource</DataSourceName><CommandText>
          SELECT
          Professionals.FirstName
          ,Professionals.LastName
          ,Professionals.MiddleName
          FROM
          Professionals
        </CommandText><rd:DesignerState><QueryDefinition xmlns=""http://schemas.microsoft.com/ReportingServices/QueryDefinition/Relational""><SelectedColumns><ColumnExpression ColumnOwner=""Professionals"" ColumnName=""FirstName"" /><ColumnExpression ColumnOwner=""Professionals"" ColumnName=""LastName"" /><ColumnExpression ColumnOwner=""Professionals"" ColumnName=""MiddleName"" /></SelectedColumns></QueryDefinition></rd:DesignerState></Query><Fields><Field Name=""FirstName""><DataField>FirstName</DataField><rd:TypeName>System.String</rd:TypeName></Field><Field Name=""LastName""><DataField>LastName</DataField><rd:TypeName>System.String</rd:TypeName></Field><Field Name=""MiddleName""><DataField>MiddleName</DataField><rd:TypeName>System.String</rd:TypeName></Field></Fields></DataSet></DataSets><ReportSections><ReportSection><Body><ReportItems><Tablix Name=""Tablix1""><TablixBody><TablixColumns><TablixColumn><Width>1in</Width></TablixColumn><TablixColumn><Width>1in</Width></TablixColumn></TablixColumns><TablixRows><TablixRow><Height>0.25in</Height><TablixCells><TablixCell><CellContents><Textbox Name=""Textbox2""><CanGrow>true</CanGrow><KeepTogether>true</KeepTogether><Paragraphs><Paragraph><TextRuns><TextRun><Value>First Name</Value><Style><FontFamily>Tahoma</FontFamily><FontSize>11pt</FontSize><FontWeight>Bold</FontWeight><Color>White</Color></Style></TextRun></TextRuns><Style /></Paragraph></Paragraphs><rd:DefaultName>Textbox2</rd:DefaultName><Style><Border><Color>#4e648a</Color><Style>Solid</Style></Border><BackgroundColor>#384c70</BackgroundColor><PaddingLeft>2pt</PaddingLeft><PaddingRight>2pt</PaddingRight><PaddingTop>2pt</PaddingTop><PaddingBottom>2pt</PaddingBottom></Style></Textbox></CellContents></TablixCell><TablixCell><CellContents><Textbox Name=""Textbox3""><CanGrow>true</CanGrow><KeepTogether>true</KeepTogether><Paragraphs><Paragraph><TextRuns><TextRun><Value>Last Name</Value><Style><FontFamily>Tahoma</FontFamily><FontSize>11pt</FontSize><FontWeight>Bold</FontWeight><Color>White</Color></Style></TextRun></TextRuns><Style /></Paragraph></Paragraphs><rd:DefaultName>Textbox3</rd:DefaultName><Style><Border><Color>#4e648a</Color><Style>Solid</Style></Border><BackgroundColor>#384c70</BackgroundColor><PaddingLeft>2pt</PaddingLeft><PaddingRight>2pt</PaddingRight><PaddingTop>2pt</PaddingTop><PaddingBottom>2pt</PaddingBottom></Style></Textbox></CellContents></TablixCell></TablixCells></TablixRow><TablixRow><Height>0.25in</Height><TablixCells><TablixCell><CellContents><Textbox Name=""FirstName""><CanGrow>true</CanGrow><KeepTogether>true</KeepTogether><Paragraphs><Paragraph><TextRuns><TextRun><Value>=Fields!FirstName.Value</Value><Style><FontFamily>Tahoma</FontFamily><Color>#4d4d4d</Color></Style></TextRun></TextRuns><Style /></Paragraph></Paragraphs><rd:DefaultName>FirstName</rd:DefaultName><Style><Border><Color>#e5e5e5</Color><Style>Solid</Style></Border><PaddingLeft>2pt</PaddingLeft><PaddingRight>2pt</PaddingRight><PaddingTop>2pt</PaddingTop><PaddingBottom>2pt</PaddingBottom></Style></Textbox></CellContents></TablixCell><TablixCell><CellContents><Textbox Name=""LastName""><CanGrow>true</CanGrow><KeepTogether>true</KeepTogether><Paragraphs><Paragraph><TextRuns><TextRun><Value>=Fields!LastName.Value</Value><Style><FontFamily>Tahoma</FontFamily><Color>#4d4d4d</Color></Style></TextRun></TextRuns><Style /></Paragraph></Paragraphs><rd:DefaultName>LastName</rd:DefaultName><Style><Border><Color>#e5e5e5</Color><Style>Solid</Style></Border><PaddingLeft>2pt</PaddingLeft><PaddingRight>2pt</PaddingRight><PaddingTop>2pt</PaddingTop><PaddingBottom>2pt</PaddingBottom></Style></Textbox></CellContents></TablixCell></TablixCells></TablixRow></TablixRows></TablixBody><TablixColumnHierarchy><TablixMembers><TablixMember /><TablixMember /></TablixMembers></TablixColumnHierarchy><TablixRowHierarchy><TablixMembers><TablixMember><KeepWithGroup>After</KeepWithGroup></TablixMember><TablixMember><Group Name=""Details"" /></TablixMember></TablixMembers></TablixRowHierarchy><DataSetName>DataSet1</DataSetName><Top>0.4in</Top><Height>0.5in</Height><Width>2in</Width><Style><Border><Style>None</Style></Border></Style></Tablix><Textbox Name=""ReportTitle""><CanGrow>true</CanGrow><KeepTogether>true</KeepTogether><Paragraphs><Paragraph><TextRuns><TextRun><Value>Test Report</Value><Style><FontFamily>Verdana</FontFamily><FontSize>20pt</FontSize></Style></TextRun></TextRuns><Style /></Paragraph></Paragraphs><rd:WatermarkTextbox>Title</rd:WatermarkTextbox><rd:DefaultName>ReportTitle</rd:DefaultName><Height>0.4in</Height><Width>5.5in</Width><ZIndex>1</ZIndex><Style><Border><Style>None</Style></Border><PaddingLeft>2pt</PaddingLeft><PaddingRight>2pt</PaddingRight><PaddingTop>2pt</PaddingTop><PaddingBottom>2pt</PaddingBottom></Style></Textbox></ReportItems><Height>2.25in</Height><Style><Border><Style>None</Style></Border></Style></Body><Width>6in</Width><Page><PageFooter><Height>0.45in</Height><PrintOnFirstPage>true</PrintOnFirstPage><PrintOnLastPage>true</PrintOnLastPage><ReportItems><Textbox Name=""ExecutionTime""><CanGrow>true</CanGrow><KeepTogether>true</KeepTogether><Paragraphs><Paragraph><TextRuns><TextRun><Value>=Globals!ExecutionTime</Value><Style /></TextRun></TextRuns><Style><TextAlign>Right</TextAlign></Style></Paragraph></Paragraphs><rd:DefaultName>ExecutionTime</rd:DefaultName><Top>0.2in</Top><Left>4in</Left><Height>0.25in</Height><Width>2in</Width><Style><Border><Style>None</Style></Border><PaddingLeft>2pt</PaddingLeft><PaddingRight>2pt</PaddingRight><PaddingTop>2pt</PaddingTop><PaddingBottom>2pt</PaddingBottom></Style></Textbox></ReportItems><Style><Border><Style>None</Style></Border></Style></PageFooter><LeftMargin>1in</LeftMargin><RightMargin>1in</RightMargin><TopMargin>1in</TopMargin><BottomMargin>1in</BottomMargin><Style /></Page></ReportSection></ReportSections><rd:ReportUnitType>Inch</rd:ReportUnitType><rd:ReportServerUrl>http://localhost/ReportServer</rd:ReportServerUrl><rd:ReportID>62eb8402-88fd-4b5b-82d7-59907f1518ef</rd:ReportID></Report>";
        #endregion

        #region Helper Methods
        private byte[] StringToByteArray(string text)
        {
            char[] charArray = text.ToCharArray();

            byte[] byteArray = new byte[charArray.Length];

            for (int i = 0; i < charArray.Length; i++)
            {
                byteArray[i] = Convert.ToByte(charArray[i]);
            }

            return byteArray;
        }
        #endregion

        [SetUp]
        public void SetUp()
        {
        }

        #region SSRSUtil.UpdateReportDefinition Tests
        //TODO This fails, need to fix it.
        //SSRSMigrate.Tests.Utilities.SSRSUtil_Tests.UpdateReportDefinition:
        //Expected string length 6856 but was 6874. Strings differ at index 6763.
        //Expected: "...calhost/ReportServer_NEWSERVER</rd:ReportServerUrl><rd:Rep..."
        //But was:  "...calhost/ReportServer_NEWSERVER/SSRSMigrats_Tests</rd:Repor..."
        //--------------------------------------------^

        [Test]
        public void UpdateReportDefinition()
        {
            byte[] expected = StringToByteArray(reportDefinitionForUpdateReportDefExpected);
            byte[] argument = StringToByteArray(reportDefinitionForUpdateReportDefArgument);

            byte[] actualByteArray = SSRSUtil.UpdateReportDefinition(
                "http://localhost/ReportServer/SSRSMigrate_Tests",
                "http://localhost/ReportServer_NEWSERVER/SSRSMigrats_Tests",
                argument);

            UTF8Encoding decoder = new UTF8Encoding();
            string actual = decoder.GetString(actualByteArray);

            Assert.AreEqual(reportDefinitionForUpdateReportDefExpected, actual);
        }

        [Test]
        public void UpdateReportDefinition_NullDefinition()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    SSRSUtil.UpdateReportDefinition(
                        "http://localhost/ReportServer/SSRSMigrate_Tests",
                        "http://localhost/ReportServer_NEWSERVER/SSRSMigrats_Tests",
                        null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: reportDefinition"));
        }

        [Test]
        public void UpdateReportDefinition_NullDestinationPath()
        {
            byte[] argument = StringToByteArray(reportDefinitionForUpdateReportDefArgument);

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    SSRSUtil.UpdateReportDefinition(
                        "http://localhost/ReportServer/SSRSMigrate_Tests",
                        null,
                        argument);
                });

            Assert.That(ex.Message, Is.EqualTo("destinationPath"));
        }

        [Test]
        public void UpdateReportDefinition_EmptyDestinationPath()
        {
            byte[] argument = StringToByteArray(reportDefinitionForUpdateReportDefArgument);

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    SSRSUtil.UpdateReportDefinition(
                        "http://localhost/ReportServer/SSRSMigrate_Tests",
                        "",
                        argument);
                });

            Assert.That(ex.Message, Is.EqualTo("destinationPath"));
        }
        #endregion

        #region SSRSUtil.GetFullDestinationPathForItem Tests
        [Test]
        public void GetFullDestinationPathForItem_InstanceChange()
        {
            string expected = "http://localhost/ReportServer_NEWSERVER/SSRSMigrate_Tests/Reports/Test Report";

            string actual = SSRSUtil.GetFullDestinationPathForItem(
                "http://localhost/ReportServer/SSRSMigrate_Tests",
                "http://localhost/ReportServer_NEWSERVER/SSRSMigrate_Tests",
                "http://localhost/ReportServer/SSRSMigrate_Tests/Reports/Test Report");

            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void GetFullDestinationPathForItem_FolderChange()
        {
            string expected = "http://localhost/ReportServer/SSRSMigrate_NewFolder/Reports/Test Report";

            string actual = SSRSUtil.GetFullDestinationPathForItem(
                "http://localhost/ReportServer/SSRSMigrate_Tests",
                "http://localhost/ReportServer/SSRSMigrate_NewFolder",
                "http://localhost/ReportServer/SSRSMigrate_Tests/Reports/Test Report");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetFullDestinationPathForItem_InstanceAndFolderChange()
        {
            string expected = "http://localhost/ReportServer_NEWSERVER/SSRSMigrate_NewFolder/Reports/Test Report";

            string actual = SSRSUtil.GetFullDestinationPathForItem(
                "http://localhost/ReportServer/SSRSMigrate_Tests",
                "http://localhost/ReportServer_NEWSERVER/SSRSMigrate_NewFolder",
                "http://localhost/ReportServer/SSRSMigrate_Tests/Reports/Test Report");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetFullDestinationPathForItem_NullSoucePath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    SSRSUtil.GetFullDestinationPathForItem(
                        null,
                        "http://localhost/ReportServer/SSRSMigrate_NewFolder",
                        "http://localhost/ReportServer/SSRSMigrate_Tests/Reports/Test Report");
                });

            Assert.That(ex.Message, Is.EqualTo("sourcePath"));
        }

        [Test]
        public void GetFullDestinationPathForItem_NullDestinationPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    SSRSUtil.GetFullDestinationPathForItem(
                        "http://localhost/ReportServer/SSRSMigrate_Tests",
                        null,
                        "http://localhost/ReportServer/SSRSMigrate_Tests/Reports/Test Report");
                });

            Assert.That(ex.Message, Is.EqualTo("destinationPath"));
        }

        [Test]
        public void GetFullDestinationPathForItem_NullItemPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    SSRSUtil.GetFullDestinationPathForItem(
                        "http://localhost/ReportServer/SSRSMigrate_Tests",
                        "http://localhost/ReportServer/SSRSMigrate_NewFolder",
                        null);
                });

            Assert.That(ex.Message, Is.EqualTo("itemPath"));
        }

        [Test]
        public void GetFullDestinationPathForItem_EmptySoucePath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    SSRSUtil.GetFullDestinationPathForItem(
                        "",
                        "http://localhost/ReportServer/SSRSMigrate_NewFolder",
                        "http://localhost/ReportServer/SSRSMigrate_Tests/Reports/Test Report");
                });

            Assert.That(ex.Message, Is.EqualTo("sourcePath"));
        }

        [Test]
        public void GetFullDestinationPathForItem_EmptyDestinationPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    SSRSUtil.GetFullDestinationPathForItem(
                        "http://localhost/ReportServer/SSRSMigrate_Tests",
                        "",
                        "http://localhost/ReportServer/SSRSMigrate_Tests/Reports/Test Report");
                });

            Assert.That(ex.Message, Is.EqualTo("destinationPath"));
        }

        [Test]
        public void GetFullDestinationPathForItem_EmptyItemPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    SSRSUtil.GetFullDestinationPathForItem(
                        "http://localhost/ReportServer/SSRSMigrate_Tests",
                        "http://localhost/ReportServer/SSRSMigrate_NewFolder",
                        "");
                });

            Assert.That(ex.Message, Is.EqualTo("itemPath"));
        }
        #endregion
    }
}
