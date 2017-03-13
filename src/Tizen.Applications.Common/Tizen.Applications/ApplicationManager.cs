/*
 * Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tizen.Applications
{
    /// <summary>
    /// This class has the methods and events of the ApplicationManager.
    /// </summary>
    public static class ApplicationManager
    {
        private const string LogTag = "Tizen.Applications";
        private static EventHandler<ApplicationLaunchedEventArgs> s_launchedHandler;
        private static EventHandler<ApplicationTerminatedEventArgs> s_terminatedHandler;
        private static Interop.ApplicationManager.AppManagerAppContextEventCallback s_applicationChangedEventCallback;
        private static EventHandler<ApplicationEnabledEventArgs> _enabledHandler;
        private static EventHandler<ApplicationDisabledEventArgs> _disabledHandler;
        private static Interop.ApplicationManager.AppManagerEventCallback _eventCallback;
        private static IntPtr _eventHandle = IntPtr.Zero;

        /// <summary>
        /// Occurs whenever the installed application is enabled.
        /// </summary>
        public static event EventHandler<ApplicationEnabledEventArgs> ApplicationEnabled
        {
            add
            {
                if (_enabledHandler == null && _disabledHandler == null)
                {
                    RegisterApplicationEvent();
                }
                _enabledHandler += value;
            }
            remove
            {
                _enabledHandler -= value;
                if (_enabledHandler == null && _disabledHandler == null)
                {
                    UnRegisterApplicationEvent();
                }
            }
        }

        /// <summary>
        /// Occurs whenever the installed application is disabled.
        /// </summary>
        public static event EventHandler<ApplicationDisabledEventArgs> ApplicationDisabled
        {
            add
            {
                if (_disabledHandler == null && _enabledHandler == null)
                {
                    RegisterApplicationEvent();
                }
                _disabledHandler += value;
            }
            remove
            {
                _disabledHandler -= value;
                if (_disabledHandler == null && _enabledHandler == null)
                {
                    UnRegisterApplicationEvent();
                }
            }
        }

        /// <summary>
        /// Occurs whenever the installed applications get launched.
        /// </summary>
        public static event EventHandler<ApplicationLaunchedEventArgs> ApplicationLaunched
        {
            add
            {
                if (s_launchedHandler == null && s_terminatedHandler == null)
                {
                    RegisterApplicationChangedEvent();
                }
                s_launchedHandler += value;
            }
            remove
            {
                s_launchedHandler -= value;
                if (s_launchedHandler == null && s_terminatedHandler == null)
                {
                    UnRegisterApplicationChangedEvent();
                }
            }
        }

        /// <summary>
        /// Occurs whenever the installed applications get terminated.
        /// </summary>
        public static event EventHandler<ApplicationTerminatedEventArgs> ApplicationTerminated
        {
            add
            {
                if (s_launchedHandler == null && s_terminatedHandler == null)
                {
                    RegisterApplicationChangedEvent();
                }
                s_terminatedHandler += value;
            }
            remove
            {
                s_terminatedHandler -= value;
                if (s_launchedHandler == null && s_terminatedHandler == null)
                {
                    UnRegisterApplicationChangedEvent();
                }
            }
        }

        /// <summary>
        /// Gets the information of the installed applications asynchronously.
        /// </summary>
        public static async Task<IEnumerable<ApplicationInfo>> GetInstalledApplicationsAsync()
        {
            return await Task.Run(() =>
            {
                Interop.ApplicationManager.ErrorCode err = Interop.ApplicationManager.ErrorCode.None;
                List<ApplicationInfo> result = new List<ApplicationInfo>();

                Interop.ApplicationManager.AppManagerAppInfoCallback cb = (IntPtr infoHandle, IntPtr userData) =>
                {
                    if (infoHandle != IntPtr.Zero)
                    {
                        IntPtr clonedHandle = IntPtr.Zero;
                        err = Interop.ApplicationManager.AppInfoClone(out clonedHandle, infoHandle);
                        if (err != Interop.ApplicationManager.ErrorCode.None)
                        {
                            Log.Warn(LogTag, "Failed to clone the appinfo. err = " + err);
                            return false;
                        }
                        ApplicationInfo app = new ApplicationInfo(clonedHandle);
                        result.Add(app);
                        return true;
                    }
                    return false;
                };
                err = Interop.ApplicationManager.AppManagerForeachAppInfo(cb, IntPtr.Zero);
                if (err != Interop.ApplicationManager.ErrorCode.None)
                {
                    throw ApplicationManagerErrorFactory.GetException(err, "Failed to foreach the appinfo.");
                }
                return result;
            });
        }

        /// <summary>
        /// Gets the information of the installed applications with the ApplicationInfoFilter asynchronously.
        /// </summary>
        /// <param name="filter">Key-value pairs for filtering.</param>
        public static async Task<IEnumerable<ApplicationInfo>> GetInstalledApplicationsAsync(ApplicationInfoFilter filter)
        {
            return await Task.Run(() =>
            {
                List<ApplicationInfo> result = new List<ApplicationInfo>();

                Interop.ApplicationManager.AppInfoFilterCallback cb = (IntPtr infoHandle, IntPtr userData) =>
                {
                    if (infoHandle != IntPtr.Zero)
                    {
                        IntPtr clonedHandle = IntPtr.Zero;
                        Interop.ApplicationManager.ErrorCode err = Interop.ApplicationManager.AppInfoClone(out clonedHandle, infoHandle);
                        if (err != Interop.ApplicationManager.ErrorCode.None)
                        {
                            Log.Warn(LogTag, "Failed to clone the appinfo. err = " + err);
                            return false;
                        }
                        ApplicationInfo app = new ApplicationInfo(clonedHandle);
                        result.Add(app);
                        return true;
                    }
                    return false;
                };
                filter.Fetch(cb);
                return result;
            });
        }

        /// <summary>
        /// Gets the information of the installed applications with the ApplicationInfoMetadataFilter asynchronously.
        /// </summary>
        /// <param name="filter">Key-value pairs for filtering.</param>
        public static async Task<IEnumerable<ApplicationInfo>> GetInstalledApplicationsAsync(ApplicationInfoMetadataFilter filter)
        {
            return await Task.Run(() =>
            {
                List<ApplicationInfo> result = new List<ApplicationInfo>();

                Interop.ApplicationManager.AppInfoFilterCallback cb = (IntPtr infoHandle, IntPtr userData) =>
                {
                    if (infoHandle != IntPtr.Zero)
                    {
                        IntPtr clonedHandle = IntPtr.Zero;
                        Interop.ApplicationManager.ErrorCode err = Interop.ApplicationManager.AppInfoClone(out clonedHandle, infoHandle);
                        if (err != Interop.ApplicationManager.ErrorCode.None)
                        {
                            Log.Warn(LogTag, "Failed to clone the appinfo. err = " + err);
                            return false;
                        }
                        ApplicationInfo app = new ApplicationInfo(clonedHandle);
                        result.Add(app);
                        return true;
                    }
                    return false;
                };
                filter.Fetch(cb);
                return result;
            });
        }

        /// <summary>
        /// Gets the information of the running applications asynchronously.
        /// </summary>
        public static async Task<IEnumerable<ApplicationInfo>> GetRunningApplicationsAsync()
        {
            return await Task.Run(() =>
            {
                Interop.ApplicationManager.ErrorCode err = Interop.ApplicationManager.ErrorCode.None;
                List<ApplicationInfo> result = new List<ApplicationInfo>();

                Interop.ApplicationManager.AppManagerAppContextCallback cb = (IntPtr contextHandle, IntPtr userData) =>
                {
                    if (contextHandle != IntPtr.Zero)
                    {
                        string appid = string.Empty;
                        err = Interop.ApplicationManager.AppContextGetAppId(contextHandle, out appid);
                        if (err != Interop.ApplicationManager.ErrorCode.None)
                        {
                            Log.Warn(LogTag, "Failed to get appid. err = " + err);
                            return false;
                        }
                        IntPtr infoHandle = IntPtr.Zero;
                        err = Interop.ApplicationManager.AppManagerGetAppInfo(appid, out infoHandle);
                        if (err != Interop.ApplicationManager.ErrorCode.None)
                        {
                            Log.Warn(LogTag, "Failed to get the application information.");
                            return false;
                        }
                        IntPtr cloneContextHandle = IntPtr.Zero;
                        err = Interop.ApplicationManager.AppContextClone(out cloneContextHandle, contextHandle);
                        if (err != Interop.ApplicationManager.ErrorCode.None)
                        {
                            Log.Warn(LogTag, "Failed to clone the application context handle");
                            Interop.ApplicationManager.AppInfoDestroy(infoHandle);
                            return false;
                        }
                        ApplicationInfo app = new ApplicationInfo(infoHandle, cloneContextHandle);
                        if (app != null)
                        {
                            result.Add(app);
                            return true;
                        }
                    }
                    return false;
                };

                err = Interop.ApplicationManager.AppManagerForeachAppContext(cb, IntPtr.Zero);
                if (err != Interop.ApplicationManager.ErrorCode.None)
                {
                    throw ApplicationManagerErrorFactory.GetException(err, "Failed to foreach appcontext.");
                }
                return result;
            });
        }

        /// <summary>
        /// Gets the information of all running applications asynchronously.
        /// </summary>
        public static async Task<IEnumerable<ApplicationInfo>> GetAllRunningApplicationsAsync()
        {
            return await Task.Run(() =>
            {
                Interop.ApplicationManager.ErrorCode err = Interop.ApplicationManager.ErrorCode.None;
                List<ApplicationInfo> result = new List<ApplicationInfo>();

                Interop.ApplicationManager.AppManagerAppContextCallback cb = (IntPtr contextHandle, IntPtr userData) =>
                {
                    if (contextHandle != IntPtr.Zero)
                    {
                        string appid = string.Empty;
                        err = Interop.ApplicationManager.AppContextGetAppId(contextHandle, out appid);
                        if (err != Interop.ApplicationManager.ErrorCode.None)
                        {
                            Log.Warn(LogTag, "Failed to get appid. err = " + err);
                            return false;
                        }
                        IntPtr infoHandle = IntPtr.Zero;
                        err = Interop.ApplicationManager.AppManagerGetAppInfo(appid, out infoHandle);
                        if (err != Interop.ApplicationManager.ErrorCode.None)
                        {
                            Log.Warn(LogTag, "Failed to get the application information.");
                            return false;
                        }
                        IntPtr cloneContextHandle = IntPtr.Zero;
                        err = Interop.ApplicationManager.AppContextClone(out cloneContextHandle, contextHandle);
                        if (err != Interop.ApplicationManager.ErrorCode.None)
                        {
                            Log.Warn(LogTag, "Failed to clone the application context handle");
                            Interop.ApplicationManager.AppInfoDestroy(infoHandle);
                            return false;
                        }
                        ApplicationInfo app = new ApplicationInfo(infoHandle, cloneContextHandle);
                        if (app != null)
                        {
                            result.Add(app);
                            return true;
                        }
                    }
                    return false;
                };

                err = Interop.ApplicationManager.AppManagerForeachRunningAppContext(cb, IntPtr.Zero);
                if (err != Interop.ApplicationManager.ErrorCode.None)
                {
                    throw ApplicationManagerErrorFactory.GetException(err, "Failed to foreach appcontext.");
                }
                return result;
            });
        }

        /// <summary>
        /// Gets the information of the specified application with the application id.
        /// </summary>
        /// <param name="applicationId">Application id.</param>
        public static ApplicationInfo GetInstalledApplication(string applicationId)
        {
            IntPtr infoHandle = IntPtr.Zero;
            Interop.ApplicationManager.ErrorCode err = Interop.ApplicationManager.AppManagerGetAppInfo(applicationId, out infoHandle);
            if (err != Interop.ApplicationManager.ErrorCode.None)
            {
                throw ApplicationManagerErrorFactory.GetException(err, "Failed to get the installed application information.");
            }
            ApplicationInfo app = new ApplicationInfo(infoHandle);
            return app;
        }

        private static void RegisterApplicationChangedEvent()
        {
            Interop.ApplicationManager.ErrorCode err = Interop.ApplicationManager.ErrorCode.None;
            s_applicationChangedEventCallback = (IntPtr contextHandle, Interop.ApplicationManager.AppContextEvent state, IntPtr userData) =>
            {
                if (contextHandle == IntPtr.Zero) return;
                try
                {
                    string appid = string.Empty;
                    err = Interop.ApplicationManager.AppContextGetAppId(contextHandle, out appid);
                    if (err != Interop.ApplicationManager.ErrorCode.None)
                    {
                        throw ApplicationManagerErrorFactory.GetException(err, "Failed to get application id.");
                    }

                    ApplicationInfo appInfo = GetInstalledApplication(appid);
                    if (state == Interop.ApplicationManager.AppContextEvent.Launched)
                    {
                        s_launchedHandler?.Invoke(null, new ApplicationLaunchedEventArgs { ApplicationInfo = appInfo });
                    }
                    else if (state == Interop.ApplicationManager.AppContextEvent.Terminated)
                    {
                        s_terminatedHandler?.Invoke(null, new ApplicationTerminatedEventArgs { ApplicationInfo = appInfo });
                    }
                }
                catch (Exception e)
                {
                    Log.Warn(LogTag, e.Message);
                }
            };
            err = Interop.ApplicationManager.AppManagerSetAppContextEvent(s_applicationChangedEventCallback, IntPtr.Zero);
            if (err != Interop.ApplicationManager.ErrorCode.None)
            {
                throw ApplicationManagerErrorFactory.GetException(err, "Failed to register the application context event.");
            }
        }

        private static void UnRegisterApplicationChangedEvent()
        {
            Interop.ApplicationManager.AppManagerUnSetAppContextEvent();
        }

        private static void RegisterApplicationEvent()
        {
            Interop.ApplicationManager.ErrorCode err = Interop.ApplicationManager.ErrorCode.None;
            err = Interop.ApplicationManager.AppManagerEventCreate(out _eventHandle);
            if (err != Interop.ApplicationManager.ErrorCode.None)
            {
                throw ApplicationManagerErrorFactory.GetException(err, "Failed to create the application event handle");
            }

            err = Interop.ApplicationManager.AppManagerEventSetStatus(_eventHandle, Interop.ApplicationManager.AppManagerEventStatusType.All);
            if (err != Interop.ApplicationManager.ErrorCode.None)
            {
                Interop.ApplicationManager.AppManagerEventDestroy(_eventHandle);
                _eventHandle = IntPtr.Zero;
                throw ApplicationManagerErrorFactory.GetException(err, "Failed to set the application event");
            }

            _eventCallback = (string appType, string appId, Interop.ApplicationManager.AppManagerEventType eventType, Interop.ApplicationManager.AppManagerEventState eventState, IntPtr eventHandle, IntPtr UserData) =>
            {
                if (eventType == Interop.ApplicationManager.AppManagerEventType.Enable)
                {
                    _enabledHandler?.Invoke(null, new ApplicationEnabledEventArgs(appId, (ApplicationEventState)eventState));
                }
                else if (eventType == Interop.ApplicationManager.AppManagerEventType.Disable)
                {
                    _disabledHandler?.Invoke(null, new ApplicationDisabledEventArgs(appId, (ApplicationEventState)eventState));
                }
            };
            err = Interop.ApplicationManager.AppManagerSetEventCallback(_eventHandle, _eventCallback, IntPtr.Zero);
            if (err != Interop.ApplicationManager.ErrorCode.None)
            {
                Interop.ApplicationManager.AppManagerEventDestroy(_eventHandle);
                _eventHandle = IntPtr.Zero;
                throw ApplicationManagerErrorFactory.GetException(err, "Failed to set the application event callback");
            }
        }

        private static void UnRegisterApplicationEvent()
        {
            if (_eventHandle != IntPtr.Zero)
            {
                Interop.ApplicationManager.AppManagerUnSetEventCallback(_eventHandle);
                Interop.ApplicationManager.AppManagerEventDestroy(_eventHandle);
                _eventHandle = IntPtr.Zero;
            }
        }
    }

    internal static class FilterExtension
    {
        private const string LogTag = "Tizen.Applications";
        internal static void Fetch(this ApplicationInfoFilter filter, Interop.ApplicationManager.AppInfoFilterCallback callback)
        {
            if (filter is ApplicationInfoMetadataFilter)
            {
                ApplicationInfoMetadataFilter metaFilter = (ApplicationInfoMetadataFilter)filter;
                metaFilter.Fetch(callback);
                return;
            }

            IntPtr nativeHandle = MakeNativeAppInfoFilter(filter.Filter);
            if (nativeHandle == IntPtr.Zero)
            {
                throw ApplicationManagerErrorFactory.NativeFilterHandleIsInvalid();
            }
            try
            {
                Interop.ApplicationManager.ErrorCode err = Interop.ApplicationManager.AppInfoFilterForeachAppinfo(nativeHandle, callback, IntPtr.Zero);
                if (err != Interop.ApplicationManager.ErrorCode.None)
                {
                    throw ApplicationManagerErrorFactory.GetException(err, "Failed to get application information list with filter.");
                }
            }
            finally
            {
                Interop.ApplicationManager.AppInfoFilterDestroy(nativeHandle);
            }
        }

        internal static void Fetch(this ApplicationInfoMetadataFilter filter, Interop.ApplicationManager.AppInfoFilterCallback callback)
        {
            IntPtr nativeHandle = MakeNativeAppMetadataFilter(filter.Filter);
            if (nativeHandle == IntPtr.Zero)
            {
                throw ApplicationManagerErrorFactory.NativeFilterHandleIsInvalid();
            }
            try
            {
                Interop.ApplicationManager.ErrorCode err = Interop.ApplicationManager.AppInfoMetadataFilterForeach(nativeHandle, callback, IntPtr.Zero);
                if (err != Interop.ApplicationManager.ErrorCode.None)
                {
                    throw ApplicationManagerErrorFactory.GetException(err, "Failed to get metadata list with filter.");
                }
            }
            finally
            {
                Interop.ApplicationManager.AppInfoMetadataFilterDestroy(nativeHandle);
            }
        }

        private static IntPtr MakeNativeAppInfoFilter(IDictionary<string, string> filter)
        {
            if (filter == null || filter.Count == 0)
            {
                throw ApplicationManagerErrorFactory.FilterIsInvalid();
            }

            IntPtr infoHandle = IntPtr.Zero;
            Interop.ApplicationManager.ErrorCode err = Interop.ApplicationManager.AppInfoFilterCreate(out infoHandle);
            if (err != Interop.ApplicationManager.ErrorCode.None)
            {
                throw ApplicationManagerErrorFactory.GetException(err, "Failed to create the filter handle.");
            }

            foreach (var item in filter)
            {
                if ((item.Key == ApplicationInfoFilter.Keys.Id) ||
                    (item.Key == ApplicationInfoFilter.Keys.Type) ||
                    (item.Key == ApplicationInfoFilter.Keys.Category))
                {
                    err = Interop.ApplicationManager.AppInfoFilterAddString(infoHandle, item.Key, item.Value);
                }
                else if ((item.Key == ApplicationInfoFilter.Keys.NoDisplay) ||
                         (item.Key == ApplicationInfoFilter.Keys.TaskManage))
                {
                    err = Interop.ApplicationManager.AppInfoFilterAddBool(infoHandle, item.Key, Convert.ToBoolean(item.Value));
                }
                else
                {
                    Log.Warn(LogTag, string.Format("'{0}' is not supported key for the filter.", item.Key));
                }
                if (err != Interop.ApplicationManager.ErrorCode.None)
                {
                    Interop.ApplicationManager.AppInfoFilterDestroy(infoHandle);
                    throw ApplicationManagerErrorFactory.GetException(err, "Failed to add item to the filter.");
                }
            }
            return infoHandle;
        }

        private static IntPtr MakeNativeAppMetadataFilter(IDictionary<string, string> filter)
        {
            if (filter == null || filter.Count == 0)
            {
                throw ApplicationManagerErrorFactory.FilterIsInvalid();
            }

            IntPtr infoHandle = IntPtr.Zero;
            Interop.ApplicationManager.ErrorCode err = Interop.ApplicationManager.AppInfoMetadataFilterCreate(out infoHandle);
            if (err != Interop.ApplicationManager.ErrorCode.None)
            {
                throw ApplicationManagerErrorFactory.GetException(err, "Failed to create the filter for searching with metadata.");
            }
            foreach (var item in filter)
            {
                err = Interop.ApplicationManager.AppInfoMetadataFilterAdd(infoHandle, item.Key, item.Value);
                if (err != Interop.ApplicationManager.ErrorCode.None)
                {
                    Interop.ApplicationManager.AppInfoMetadataFilterDestroy(infoHandle);
                    throw ApplicationManagerErrorFactory.GetException(err, "Failed to add the item to the filter.");
                }
            }
            return infoHandle;
        }
    }

    internal static class ApplicationManagerErrorFactory
    {
        internal static Exception NativeFilterHandleIsInvalid()
        {
            return new InvalidOperationException("The native handle for filtering is invalid.");
        }

        internal static Exception FilterIsInvalid()
        {
            return new ArgumentException("The filter is invalid.");
        }

        internal static Exception GetException(Interop.ApplicationManager.ErrorCode err, string message)
        {
            string errMessage = String.Format("{0} err = {1}", message, err);
            switch (err)
            {
                case Interop.ApplicationManager.ErrorCode.InvalidParameter:
                    return new ArgumentException(errMessage);
                default:
                    return new InvalidOperationException(errMessage);
            }
        }
    }
}
