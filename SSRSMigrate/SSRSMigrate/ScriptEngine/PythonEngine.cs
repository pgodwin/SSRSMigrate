﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Hosting;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Ninject.Extensions.Logging;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.Utility;
using Exception = System.Exception;

namespace SSRSMigrate.ScriptEngine
{
    public class PythonEngine: MarshalByRefObject
    {
        #region Constants
        private const string cLogFile = "ScriptLog.txt";
        private const string cErrorLogFile = "ScriptErrors.txt";
        #endregion

        #region Private Variables
        private readonly IFileSystem mFileSystem;
        private readonly ILogger mLogger;
        private readonly ILogger mScriptLogger;
        private readonly IReportServerReader mReportServerReader;
        private readonly IReportServerWriter mReportServerWriter;
        private readonly IReportServerRepository mReportServerRepository;

        private AppDomain mAppDomain;
        private Microsoft.Scripting.Hosting.ScriptEngine mScriptEngine;
        private ScriptSource mScriptSource;
        private ScriptScope mScriptScope;
        private object mScriptClass;

        private string mEnginePath;
        private bool mDebug = false;
        private string mScriptPath;
        private bool mLoaded = false;
        
        #endregion

        #region Public Properties
        public Microsoft.Scripting.Hosting.ScriptEngine Engine
        {
            get { return this.mScriptEngine; }
            set { this.mScriptEngine = value; }
        }

        public ScriptSource Source
        {
            get { return this.mScriptSource; }
            set { this.mScriptSource = value; }
        }

        public ScriptScope Scope
        {
            get { return this.mScriptScope; }
            set { this.mScriptScope = value; }
        }

        public string EnginePath
        {
            get { return this.mEnginePath; }
            set { this.mEnginePath = value; }
        }

        public string ScriptPath
        {
            get { return this.mScriptPath; }
        }

        public bool Loaded
        {
            get { return this.mLoaded; }
        }
        public bool Debug
        {
            get { return this.mDebug; }
            set { this.mDebug = value; }
        }

        public string LogPath
        {
            get
            {
                return this.mFileSystem.Path.Combine(this.mEnginePath, cLogFile);
            }
        }

        public string ErrorLogPath
        {
            get
            {
                return this.mFileSystem.Path.Combine(this.mEnginePath, cErrorLogFile);
            }
        }
        #endregion

        public PythonEngine(
            IReportServerReader reportServerReader,
            IReportServerWriter reportServerWriter,
            IReportServerRepository reportServerRepository,
            IFileSystem fileSystem,
            ILogger logger,
            ILogger scriptLogger)
        {
            if (reportServerReader == null)
                throw new ArgumentNullException("reportServerReader");

            if (reportServerWriter == null)
                throw new ArgumentNullException("reportServerWriter");

            if (reportServerRepository == null)
                throw new ArgumentNullException("reportServerRepository");

            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");

            if (logger == null)
                throw new ArgumentNullException("logger");

            if (scriptLogger == null)
                throw new ArgumentNullException("scriptLogger");

            this.mReportServerReader = reportServerReader;
            this.mReportServerWriter = reportServerWriter;
            this.mReportServerRepository = reportServerRepository;
            this.mFileSystem = fileSystem;
            this.mLogger = logger;
            this.mScriptLogger = scriptLogger;

            this.mAppDomain = AppDomain.CreateDomain("sandbox");
            this.mScriptEngine = Python.CreateEngine();
            this.mScriptEngine.Runtime.ImportModule("clr");

            this.mEnginePath = Environment.CurrentDirectory;
        }

        public void Load(string script, string className = "Plugin")
        {
            if (string.IsNullOrEmpty(script))
                throw new ArgumentException("script");

            if (string.IsNullOrEmpty(className))
                throw new ArgumentException("className");

            if (!this.mFileSystem.File.Exists(script))
                throw new FileNotFoundException("Script not found.", script);

            try
            {
                this.mScriptPath = script;

                this.mScriptSource = this.mScriptEngine.CreateScriptSourceFromFile(script);
                this.mScriptScope = this.mScriptEngine.CreateScope();

                this.mScriptScope.SetVariable("Engine", this);

                this.mScriptScope.SetVariable("SSRSUtil", DynamicHelpers.GetPythonTypeFromType(typeof (SSRSUtil)));
                this.mScriptScope.SetVariable("FileSystem", this.mFileSystem);

                this.mScriptScope.SetVariable("ReportServerReader", this.mReportServerReader);
                this.mScriptScope.SetVariable("ReportServerWriter", this.mReportServerWriter);
                this.mScriptScope.SetVariable("ReportServerRepository", this.mReportServerRepository);

                this.mScriptScope.SetVariable("SQLUtil", DynamicHelpers.GetPythonTypeFromType(typeof (SQLUtil)));
                
                CompiledCode compiled = this.mScriptSource.Compile();
                compiled.Execute(this.mScriptScope);

                this.mScriptClass = this.mScriptEngine.Operations.Invoke(this.mScriptScope.GetVariable(className));

                this.mScriptSource.Execute(this.mScriptScope);

                this.mLoaded = true;

                this.LogLine(string.Format("Loading script '{0}'...", this.mScriptPath));

                this.CallMethod("OnLoad");
            }
            catch (SyntaxErrorException er)
            {
                var eo = this.mScriptEngine.GetService<ExceptionOperations>();
                string error = eo.FormatException(er);

                string msg = String.Format("Syntax error in \"{0}\":\n\r{1}", 
                   this.mFileSystem.Path.GetFileName(script),
                   error);

                throw new Exception(msg, er);
            }
        }

        #region Script Engine Methods
        public void SetGlobalVariable(string name, object value)
        {
            this.mScriptScope.SetVariable(name, value);
        }

        public dynamic GetGlobalVariable(string name)
        {
            return this.mScriptScope.GetVariable(name);
        }

        public void CallMethod(string method, params dynamic[] arguments)
        {
            try
            {
                ObjectOperations ops = this.mScriptEngine.Operations;

                if (this.mScriptClass != null)
                {
                    object instance = ops.GetMember(this.mScriptClass, method);

                    if (instance != null)
                    {
                        ops.InvokeMember(this.mScriptClass, method, arguments);
                    }
                }
            }
            catch (MissingMemberException er)
            {
                this.mLogger.Error(er, "Missing member when trying to execute method '{0}'.", method);
            }
            catch (Exception er)
            {
                this.mLogger.Error(er, "Script exception in method '{0}'.", method);

                throw er;
            }
        }

        public dynamic CallFunction(string method, params dynamic[] arguments)
        {
            try
            {
                ObjectOperations ops = this.mScriptEngine.Operations;

                if (this.mScriptClass != null)
                {
                    object instance = ops.GetMember(this.mScriptClass, method);

                    if (instance != null)
                    {
                        ops.InvokeMember(this.mScriptClass, method, arguments);
                    }
                }

                return null;
            }
            catch (MissingMemberException er)
            {
                this.mLogger.Error(er, "Missing member when trying to execute method '{0}'.", method);

                return null;
            }
            catch (Exception er)
            {
                this.mLogger.Error(er, "Script exception in method '{0}'.", method);

                throw er;
            }
        }
        #endregion

        #region Logging
        public void LogLine(string message)
        {
            this.mScriptLogger.Info(message);
        }

        public void LogDebug(string message)
        {
            this.mScriptLogger.Debug(message);
        }

        public void LogWarn(string message)
        {
            this.mScriptLogger.Warn(message);
        }

        public void LogTrace(string message)
        {
            this.mScriptLogger.Trace(message);
        }
        #endregion
    }
}
