using Listener.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

namespace Listener
{
    public class NotificationHandler
    {
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private static readonly UserNotificationListener listener = UserNotificationListener.Current;
        private static readonly Config.AppConfig config = new Config.AppConfig();

        public void SubscribeToWindowsNotifications()
        {
            listener.NotificationChanged += Listener_NotificationChanged;
        }

        private async Task<bool> ProcessNotification(WaNotification waNotification)
        {
            var request = Helpers.GetRequest(waNotification);
            var client = new HttpClient { BaseAddress = new Uri(config.Server.Url) };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            if (!config.App.DryRun)
            {
                var result = await client.PostAsync("chance", content);
                var resultJsonString = await result.Content.ReadAsStringAsync();
                var resultBody = JsonConvert.DeserializeObject<ChanceRes>(resultJsonString);

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    ConsoleProxy.WriteLine(null, ConsoleColor.DarkRed, 
                        $"Error has ocurred while sending WA notification: {waNotification.Id.ToString()}, HttpStatus is: {result.StatusCode}");
                    return false;
                }

                if (resultBody.StatusCode != 200)
                {
                    ConsoleProxy.WriteLine(null, ConsoleColor.Red, 
                        $"Error has ocurred while processing WA notification: {waNotification.Id.ToString()}, " +
                        $"StatusCode: '{resultBody.StatusCode}', StatusText: '{resultBody.StatusText}'");
                    return false;
                }
            }

            return true;
        }

        public async void PerformNotifications(object autoEvent)
        {
            await semaphoreSlim.WaitAsync(); // working as a lock (){}
            try
            {
                var userNotifications = await listener.GetNotificationsAsync(NotificationKinds.Toast);

                foreach (UserNotification userNotification in userNotifications)
                {
                    var waNotification = new WaNotification(userNotification);
                    if (Helpers.IsValid(waNotification))
                    {
                        var passed = await ProcessNotification(waNotification);
                        if (passed)
                        {
                            listener.RemoveNotification(userNotification.Id);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleProxy.WriteLine(null, ConsoleColor.DarkRed, e.Message);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private async void Listener_NotificationChanged(UserNotificationListener sender, UserNotificationChangedEventArgs args)
        {
            var userNotifications = await listener.GetNotificationsAsync(NotificationKinds.Toast);

            foreach (UserNotification userNotification in userNotifications)
            {
                if (userNotification.Id == args.UserNotificationId)
                {
                    var waNotification = new WaNotification(userNotification);

                    ConsoleProxy.WriteLine(null, ConsoleColor.Yellow, $"Notification arrived " +
                        $"at: {DateTime.Now.ToLocalTime().ToShortTimeString()}, id: {args.UserNotificationId}");
                    if (Helpers.IsValid(waNotification))
                    {
                        var passed = await ProcessNotification(waNotification);
                        if (passed)
                        {
                            listener.RemoveNotification(userNotification.Id);
                        }
                    }
                }
            }
        }

        public async Task<bool> RequestNotificationListenerAccess()
        {
            // Check whether the listener is supported
            if (ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
            {
                try
                {
                    // request access to the user's notifications (must be called from UI thread) or
                    // just turn Notifications On at Windows Apps & Features -> Listener -> App Permissions
                    UserNotificationListenerAccessStatus accessStatus = await listener.RequestAccessAsync();
                    if (accessStatus != UserNotificationListenerAccessStatus.Allowed)
                    {
                        ConsoleProxy.WriteLine(null, ConsoleColor.DarkRed, "Notification listener access isn't allowed in Windows settings");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    ConsoleProxy.WriteLine(null, ConsoleColor.DarkRed, e.Message);
                }

                return true;
            }
            else
            {
                ConsoleProxy.WriteLine(null, ConsoleColor.DarkRed, "The Windows version is not compatible - Notification Listener is not exists");
            }

            return false;
        }

        public bool ToastNotificationActionTriggerIsPresent()
        {
            return ApiInformation.IsTypePresent("Windows.ApplicationModel.Background.ToastNotificationActionTrigger");
        }
    }
}
