﻿using AddInSideViews;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Paratext.Data;
using TvpMain.Result;

namespace TvpMain.Util
{
    /// <summary>
    /// Process-wide error utilities.
    /// </summary>
    public class HostUtil
    {
        /// <summary>
        /// Thread-safe singleton pattern.
        /// </summary>
        private static readonly HostUtil _instance = new HostUtil();

        /// <summary>
        /// Thread-safe singleton accessor.
        /// </summary>
        public static HostUtil Instance => _instance;

        /// <summary>
        /// Global reference to plugin, to route logging.
        /// </summary>
        private TranslationValidationPlugin _translationValidationPlugin;

        /// <summary>
        /// Global reference to host interface, providing Paratext services including logging.
        /// </summary>
        private IHost _host;

        /// <summary>
        /// Property for assignment from plugin entry method.
        /// </summary>
        public TranslationValidationPlugin TranslationValidationPlugin { set => _translationValidationPlugin = value; }

        /// <summary>
        /// Property for assignment from plugin entry method.
        /// </summary>
        public IHost Host { set => _host = value; }

        /// <summary>
        /// Reports exception to log and message box w/o prefix text.
        /// </summary>
        /// <param name="ex"></param>
        public void ReportError(Exception ex)
        {
            ReportError(null, ex);
        }

        /// <summary>
        /// Reports exception to log and message box w/prefix text.
        /// </summary>
        /// <param name="prefixText">Prefix text (optional, may be null; default used when null).</param>
        /// <param name="ex">Exception (required).</param>
        public void ReportError(string prefixText, Exception ex)
        {
            ReportError(prefixText, true, ex);
        }

        /// <summary>
        /// Set up the ParatextData libraries for project input/output.
        /// </summary>
        public void InitParatextData()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyPath = Path.GetDirectoryName(executingAssembly.Location);
            if (assemblyPath == null)
            {
                throw new InvalidOperationException(
                    $"plugin assembly in unexpected location: {executingAssembly.Location}");
            }

            var assemblyDir = new DirectoryInfo(assemblyPath);
            if (assemblyDir.Parent?.Parent == null)
            {
                throw new InvalidOperationException(
                    $"plugin directory in unexpected location: {assemblyDir.FullName}");
            }

            PtxUtils.Platform.BaseDirectory = assemblyDir.Parent.Parent.FullName;
            ParatextData.Initialize();

#if DEBUG
            ReportNonFatalParatextDataErrors();
#endif
        }

        /// <summary>
        /// Reports non-fatal ParatextData initialization errors.
        /// </summary>
        public void ReportNonFatalParatextDataErrors()
        {
            var errorText = string.Join(Environment.NewLine,
                ScrTextCollection.ErrorMessages.Select(messageItem => $"Project: {messageItem.ProjectName}, type: {messageItem.ProjecType}, reason: {messageItem.Reason}, exception: {messageItem.Exception}."));
            if (!string.IsNullOrWhiteSpace(errorText))
            {
                ReportError("There were non-fatal initialization errors (performance may be impacted)."
                            + Environment.NewLine + Environment.NewLine
                            + errorText, false, null);
            }
        }

        /// <summary>
        /// Reports exception to log and message box w/prefix text.
        /// </summary>
        /// <param name="prefixText">Prefix text (optional, may be null; default used when null).</param>
        /// <param name="includeStackTrace">True to include stack trace, false otherwise.</param>
        /// <param name="ex">Exception (optional, may be null).</param>
        public void ReportError(string prefixText, bool includeStackTrace, Exception ex)
        {
            string messageText = null;
            if (ex == null)
            {
                messageText = (prefixText ?? "Error: Please contact support");
            }
            else
            {
                if (includeStackTrace)
                {
                    messageText = (prefixText ?? "Error: Please contact support.")
                                  + Environment.NewLine + Environment.NewLine
                                  + "Details: " + ex.ToString() + Environment.NewLine;
                }
                else
                {
                    messageText = (prefixText ?? "Error: Please contact support")
                                  + $" (Details: {ex.Message}).";
                }
            }

            MessageBox.Show(messageText, "Notice...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            LogLine(messageText, true);
        }

        /// <summary>
        /// Log text to Paratext's app log and the console.
        /// </summary>
        /// <param name="inputText">Input text (required).</param>
        /// <param name="isError">Error flag.</param>
        public void LogLine(string inputText, bool isError)
        {
            (isError ? Console.Error : Console.Out).WriteLine(inputText);
            _host?.WriteLineToLog(_translationValidationPlugin, inputText);
        }

        /// <summary>
        /// Retrieve the ignore list from the host's plugin data storage.
        /// </summary>
        /// <param name="projectName">Active project name (required).</param>
        /// <returns>Ignore list.</returns>
        public IList<IgnoreListItem> GetIgnoreList(string projectName)
        {
            var inputData =
                _host.GetPlugInData(_translationValidationPlugin,
                    projectName, MainConsts.IGNORE_LIST_ITEMS_DATA_ID);
            return inputData == null
                ? Enumerable.Empty<IgnoreListItem>().ToList()
                : JsonConvert.DeserializeObject<List<IgnoreListItem>>(inputData);
        }

        /// <summary>
        /// Stores the ignore list to the host's plugin data storage.
        /// </summary>
        /// <param name="projectName">Active project name (required).</param>
        /// <param name="outputItems">Ignore list.</param>
        public void PutIgnoreList(string projectName, IEnumerable<IgnoreListItem> outputItems)
        {
            _host.PutPlugInData(_translationValidationPlugin,
                projectName, MainConsts.IGNORE_LIST_ITEMS_DATA_ID,
                JsonConvert.SerializeObject(outputItems));
        }

        /// <summary>
        /// Retrieve the ignore list from the host's plugin data storage.
        /// </summary>
        /// <param name="projectName">Active project name (required).</param>
        /// <param name="bookId"></param>
        /// <returns>Ignore list.</returns>
        public IList<ResultItem> GetResultItems(string projectName, string bookId)
        {
            var inputData =
                _host.GetPlugInData(_translationValidationPlugin, projectName,
                    string.Format(MainConsts.RESULT_ITEMS_DATA_ID_FORMAT, bookId));
            return inputData == null
                ? Enumerable.Empty<ResultItem>().ToList()
                : JsonConvert.DeserializeObject<List<ResultItem>>(inputData);
        }

        /// <summary>
        /// Stores the ignore list to the host's plugin data storage.
        /// </summary>
        /// <param name="projectName">Active project name (required).</param>
        /// <param name="bookId"></param>
        /// <param name="outputItems">Ignore list.</param>
        public void PutResultItems(string projectName, string bookId, IEnumerable<ResultItem> outputItems)
        {
            _host.PutPlugInData(_translationValidationPlugin, projectName,
                string.Format(MainConsts.RESULT_ITEMS_DATA_ID_FORMAT, bookId),
                JsonConvert.SerializeObject(outputItems));
        }
    }
}
