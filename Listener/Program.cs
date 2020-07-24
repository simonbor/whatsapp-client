using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

// https://blog.pieeatingninjas.be/2018/04/05/creating-a-uwp-console-app-in-c/
// https://docs.microsoft.com/en-us/windows/uwp/design/shell/tiles-and-notifications/notification-listener
namespace Listener
{
    class Program
    {
        private static readonly UserNotificationListener listener = UserNotificationListener.Current;
        private static readonly Config.AppConfig config = new Config.AppConfig();

        static async void Listener_NotificationChanged(UserNotificationListener sender, UserNotificationChangedEventArgs args)
        {
            var userNotifications = await GetNotifications(sender);

            foreach (UserNotification userNotification in userNotifications)
            {
                if (userNotification.Id == args.UserNotificationId)
                {
                    ConsoleProxy.WriteLine(null, ConsoleColor.Yellow, $"Notification arrived id - {args.UserNotificationId}");
                    await ProcessNotification(userNotification);
                }
            }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            listener.NotificationChanged += Listener_NotificationChanged;

            if (RequestAccess(listener).Result)
            {
                var userNotifications = GetNotifications(listener).Result;

                foreach (UserNotification userNotification in userNotifications)
                {
                    ProcessNotification(userNotification).Wait();
                }
            }

            Console.ReadLine();
        }

        static async Task ProcessNotification(UserNotification userNotification)
        {
            var request = Helpers.GetRequest(userNotification);

            if (!config.App.Dryrun && Helpers.IsValid(request))
            {
                var client = new HttpClient { BaseAddress = new Uri(config.Server.Url) };
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var result = await client.PostAsync("chance", content);

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    ConsoleProxy.WriteLine(null, ConsoleColor.DarkRed, $"Error! HttpStatus is: {result.StatusCode}");
                }
            }
        }

        static async Task<IReadOnlyList<UserNotification>> GetNotifications(UserNotificationListener listener)
        {
            // Get all the current notifications from the platform
            return await listener.GetNotificationsAsync(NotificationKinds.Toast);
        }

        static async Task<bool> RequestAccess(UserNotificationListener listener)
        {
            // Check whether the listener is supported
            if (ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
            {
                // request access to the user's notifications (must be called from UI thread) or
                // just turn Notifications On at Windows Apps & Features -> Listener -> App Permissions
                UserNotificationListenerAccessStatus accessStatus = await listener.RequestAccessAsync();

                if(accessStatus != UserNotificationListenerAccessStatus.Allowed)
                {
                    ConsoleProxy.WriteLine(null, ConsoleColor.DarkRed, "Notification listener access isn't allowed in Windows settings");
                }

                return true;
            }
            else
            {
                ConsoleProxy.WriteLine(null, ConsoleColor.DarkRed, "The Windows version is not compatible - no Listener exists");
            }

            return false;
        }
    }
}
