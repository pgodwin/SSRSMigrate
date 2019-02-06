using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ninject.Extensions.Logging;
using NUnit.Framework;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.TestHelper.Logging;

namespace SSRSMigrate.Tests.PythonEngine
{
    [TestFixture]
    class PythonEngine_Tests
    {
        private Mock<IReportServerReader> reportServerReaderMock;
        private Mock<IReportServerWriter> reportServerWriterMock;
        private Mock<IReportServerRepository> reportServerRepositoryMock;
        private Mock<IFileSystem> fileSystemMock;
        private MockLogger loggerMock;
        private Mock<ILogger> scriptLoggerMock;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {

        }

        [SetUp]
        public void SetUp()
        {
            reportServerReaderMock = new Mock<IReportServerReader>();
            reportServerWriterMock = new Mock<IReportServerWriter>();
            reportServerRepositoryMock = new Mock<IReportServerRepository>();
            fileSystemMock = new Mock<IFileSystem>();
            loggerMock = new MockLogger();
            scriptLoggerMock = new Mock<ILogger>();
        }


        [Test]
        public void OnLoad()
        {
            string scriptFile = "C:\\test.py";

            string script = @"class Plugin:
    def __init__(self):
        pass

    def OnLoad(self):
        Logger.Info(""OnLoad_Test"")";

            fileSystemMock.Setup(fs => fs.File.Exists(scriptFile))
                .Returns(true);

            fileSystemMock.Setup(fs => fs.File.ReadAllText(scriptFile))
                .Returns(script);

            var engine = new ScriptEngine.PythonEngine(
                reportServerReaderMock.Object,
                reportServerWriterMock.Object,
                reportServerRepositoryMock.Object,
                fileSystemMock.Object,
                loggerMock,
                scriptLoggerMock.Object);

            engine.Load(scriptFile);

            scriptLoggerMock.Verify(l => l.Info("OnLoad_Test"));
        }

        [Test]
        public void OnLoad_SyntaxError()
        {
            string scriptFile = "C:\\test.py";

            string script = @"class Plugin ERROR
    def __init__(self):
        pass

    def OnLoad(self):
        Logger.Info(""OnLoad_Test"")";

            fileSystemMock.Setup(fs => fs.File.Exists(scriptFile))
                .Returns(true);

            fileSystemMock.Setup(fs => fs.File.ReadAllText(scriptFile))
                .Returns(script);
            
            fileSystemMock.Setup(fs => fs.Path.GetFileName(scriptFile))
                .Returns("test.py");

            var engine = new ScriptEngine.PythonEngine(
                reportServerReaderMock.Object,
                reportServerWriterMock.Object,
                reportServerRepositoryMock.Object,
                fileSystemMock.Object,
                loggerMock,
                scriptLoggerMock.Object);

            Exception ex = Assert.Throws<Exception>(
                delegate
                {
                    engine.Load(scriptFile); 

                });

            StringAssert.Contains(@"Syntax error in ""test.py"":", ex.Message);
        }

        [Test]
        public void CallMethod()
        {
            string scriptFile = "C:\\test.py";

            string script = @"class Plugin:
    def __init__(self):
        pass

    def TestMethod(self, variable):
        Logger.Info(variable)";

            fileSystemMock.Setup(fs => fs.File.Exists(scriptFile))
                .Returns(true);

            fileSystemMock.Setup(fs => fs.File.ReadAllText(scriptFile))
                .Returns(script);

            var engine = new ScriptEngine.PythonEngine(
                reportServerReaderMock.Object,
                reportServerWriterMock.Object,
                reportServerRepositoryMock.Object,
                fileSystemMock.Object,
                loggerMock,
                scriptLoggerMock.Object);

            string expected = "Test Value";

            engine.Load(scriptFile);

            engine.CallMethod("TestMethod", expected);

            scriptLoggerMock.Verify(l => l.Info(expected));
        }

        [Test]
        public void CallMethod_MissingMember()
        {
            string scriptFile = "C:\\test.py";

            string script = @"class Plugin:
    def __init__(self):
        pass

    def OnLoad(self):
        Logger.Info(""OnLoad"")";

            fileSystemMock.Setup(fs => fs.File.Exists(scriptFile))
                .Returns(true);

            fileSystemMock.Setup(fs => fs.File.ReadAllText(scriptFile))
                .Returns(script);

            var engine = new ScriptEngine.PythonEngine(
                reportServerReaderMock.Object,
                reportServerWriterMock.Object,
                reportServerRepositoryMock.Object,
                fileSystemMock.Object,
                loggerMock,
                scriptLoggerMock.Object);

            engine.Load(scriptFile); 
            engine.CallMethod("MissingMethod");

            scriptLoggerMock.Verify(l => l.Error("Missing member when trying to execute method '{0}'.", It.Is<object[]>(o => o.Contains("MissingMethod"))), 
                Times.AtLeastOnce);
        }

        [Test]
        public void CallFunction(
            [Random(1, 100, 5)] int a, 
            [Random(1, 100, 5)] int b)
        {
            string scriptFile = "C:\\test.py";

            string script = @"class Plugin:
    def __init__(self):
        pass

    def MyFunction(self, a, b):
        return a * b";

            fileSystemMock.Setup(fs => fs.File.Exists(scriptFile))
                .Returns(true);

            fileSystemMock.Setup(fs => fs.File.ReadAllText(scriptFile))
                .Returns(script);

            var engine = new ScriptEngine.PythonEngine(
                reportServerReaderMock.Object,
                reportServerWriterMock.Object,
                reportServerRepositoryMock.Object,
                fileSystemMock.Object,
                loggerMock,
                scriptLoggerMock.Object);

            int expected = a * b;

            engine.Load(scriptFile);

            int actual = engine.CallFunction("MyFunction", a, b);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CallFunction_MissingMember()
        {
            string scriptFile = "C:\\test.py";

            string script = @"class Plugin:
    def __init__(self):
        pass

    def MyFunction(self, a, b):
        return a * b";

            fileSystemMock.Setup(fs => fs.File.Exists(scriptFile))
                .Returns(true);

            fileSystemMock.Setup(fs => fs.File.ReadAllText(scriptFile))
                .Returns(script);

            var engine = new ScriptEngine.PythonEngine(
                reportServerReaderMock.Object,
                reportServerWriterMock.Object,
                reportServerRepositoryMock.Object,
                fileSystemMock.Object,
                loggerMock,
                scriptLoggerMock.Object);

            engine.Load(scriptFile); 
            engine.CallFunction("MissingMethod");

            scriptLoggerMock.Verify(l => l.Error("Missing member when trying to execute method '{0}'.", It.Is<object[]>(o => o.Contains("MissingMethod"))), 
                Times.AtLeastOnce);
        }

        [Test]
        public void SetGlobalVariable(
            [Random(1, 100, 5)] int a)
        {
            string scriptFile = "C:\\test.py";

            string script = @"class Plugin:
    def __init__(self):
        pass

    def MyFunction(self, a):
        return GlobalVariable * 10";

            fileSystemMock.Setup(fs => fs.File.Exists(scriptFile))
                .Returns(true);

            fileSystemMock.Setup(fs => fs.File.ReadAllText(scriptFile))
                .Returns(script);

            var engine = new ScriptEngine.PythonEngine(
                reportServerReaderMock.Object,
                reportServerWriterMock.Object,
                reportServerRepositoryMock.Object,
                fileSystemMock.Object,
                loggerMock,
                scriptLoggerMock.Object);

            int expected = a * 10;

            engine.Load(scriptFile);

            engine.SetGlobalVariable("GlobalVariable", a);

            int actual = engine.CallFunction("MyFunction", a);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetGlobalVariable(
            [Random(1, 100, 5)] int a)
        {
            string scriptFile = "C:\\test.py";

            string script = @"class Plugin:
    def __init__(self):
        pass

    def MyMethod(self, a):
        global GlobalVariable

        GlobalVariable = a * 10";

            fileSystemMock.Setup(fs => fs.File.Exists(scriptFile))
                .Returns(true);

            fileSystemMock.Setup(fs => fs.File.ReadAllText(scriptFile))
                .Returns(script);

            var engine = new ScriptEngine.PythonEngine(
                reportServerReaderMock.Object,
                reportServerWriterMock.Object,
                reportServerRepositoryMock.Object,
                fileSystemMock.Object,
                loggerMock,
                scriptLoggerMock.Object);

            int expected = a * 10;

            engine.Load(scriptFile);

            engine.CallMethod("MyMethod", a);

            int actual = engine.GetGlobalVariable("GlobalVariable");

            Assert.AreEqual(expected, actual);
        }

    }
}
