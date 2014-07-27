﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS;
using Moq;

namespace SSRSMigrate.Tests.SSRS
{
    [TestFixture]
    class ReportServerReader_ReportTests
    {
        ReportServerReader reader = null;

        #region GetReport - Expected ReportItem
        ReportItem expectedReportItem = null;
        string expectedReportItemDefinition = null;

        #endregion

        #region GetReports - Expected ReportItems
        List<ReportItem> expectedReportItems = null;
        string[] expectedReportItemsDefinitions = null;
        #endregion

        #region GetReports - Actual ReportItems
        List<ReportItem> actualReportItems = null; 
        #endregion

        [SetUp]
        public void SetUp()
        {
            // Setup GetReport - Expected ReportItem
            expectedReportItem = new ReportItem()
            {

            };

            // Setup GetReports - Expected ReportItems
            expectedReportItems = new List<ReportItem>()
            {

            };

            actualReportItems = new List<ReportItem>();

            // Setup IReportServerRepository mock
            var reportServerRepositoryMock = new Mock<IReportServerRepository>();

            reader = new ReportServerReader(reportServerRepositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}
