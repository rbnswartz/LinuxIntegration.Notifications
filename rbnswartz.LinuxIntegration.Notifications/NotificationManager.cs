using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using rbnswartz.LinuxIntegration.Notifications.Dbus;
using Tmds.DBus;

namespace rbnswartz.LinuxIntegration.Notifications
{
    public class NotificationManager
    {
        private readonly IDbusNotifications notificationInterface;
        private readonly string appName;
        private readonly string appIcon;
        private readonly string[] capabilities;

        public bool SupportsActions => capabilities.Contains("actions");
        public bool SupportsBodyMarkup => capabilities.Contains("body-markup");
        public bool SupportsBodyHyperLinks => capabilities.Contains("body-hyperlinks");

        public NotificationManager(string appName, string appIcon="")
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new PlatformNotSupportedException("Non-linux platforms aren't supported");
            }
            this.appName = appName;
            this.appIcon = appIcon;
            var connection = Connection.Session;
            notificationInterface = connection.CreateProxy<IDbusNotifications>("org.freedesktop.Notifications", "/org/freedesktop/Notifications");
            this.capabilities = notificationInterface.GetCapabilitiesAsync().Result;
        }

        public async Task HideNotificationAsync(uint notificationId)
        {
            await notificationInterface.CloseNotificationAsync(notificationId);
        }
        public async Task<uint> ShowNotificationAsync(string summary, string body, uint replaceNotification = 0, int expiration = 0, Dictionary<string,Action> actions = null)
        {
            var actionDictionary = new Dictionary<string, Action>();
            var parsedActions = Array.Empty<string>();
            if (actions != null)
            {
                var tmp = new List<string>();
                foreach (var (key, value) in actions)
                {
                    var tmpName = Guid.NewGuid().ToString();
                    tmp.Add(tmpName);
                    tmp.Add(key);
                    actionDictionary.Add(tmpName,value);
                }

                parsedActions = tmp.ToArray();
            }
            var createdNotificationId =  await notificationInterface.NotifyAsync(appName,replaceNotification,appIcon,summary,body,parsedActions, new Dictionary<string, object>(),expiration);
            if (actionDictionary.Count != 0)
            {
                await notificationInterface.WatchActionInvokedAsync(input =>
                {
                    if (actionDictionary.ContainsKey(input.actionKey))
                    {
                        actionDictionary[input.actionKey]();
                    }
                });
            }
            return createdNotificationId;
        }
    }
}