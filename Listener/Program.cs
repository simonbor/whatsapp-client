using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

// https://blog.pieeatingninjas.be/2018/04/05/creating-a-uwp-console-app-in-c/

namespace Listener
{
    class Program
    {
        private static readonly UserNotificationListener listener = UserNotificationListener.Current;

        static void Listener_NotificationChanged(UserNotificationListener sender, UserNotificationChangedEventArgs args)
        {
            ConsoleProxy.WriteLine(null, ConsoleColor.Yellow, $"Notification arrived id - {args.UserNotificationId}");

            var userNotifications = GetNotifications(listener).Result;
            foreach (UserNotification userNotification in userNotifications)
            {
                if (userNotification.Id == args.UserNotificationId)
                {
                    PrintNotification(userNotification);
                }
            }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //UserNotificationListener listener = UserNotificationListener.Current;
            listener.NotificationChanged += Listener_NotificationChanged;

            if (RequestAccess(listener).Result)
            {
                var userNotifications = GetNotifications(listener).Result;

                foreach (UserNotification userNotification in userNotifications)
                {
                    PrintNotification(userNotification);
                }
            }

            Console.ReadLine();
        }

        static void PrintNotification(UserNotification userNotification)
        {
            string titleText = "", bodyText = "";
            NotificationBinding toastBinding = userNotification.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);

            if (toastBinding != null)
            {
                IReadOnlyList<AdaptiveNotificationText> textElements = toastBinding.GetTextElements();
                titleText = textElements.FirstOrDefault()?.Text;
                bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
            }

            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"userNotification.Id: ", $"{userNotification.Id}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"CreationTime: ", $"{userNotification.CreationTime}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"Description: ", $"{userNotification.AppInfo.DisplayInfo.Description}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"DisplayName: ", $"{userNotification.AppInfo.DisplayInfo.DisplayName}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"titleText: ", $"{titleText}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"bodyText: ", $"{bodyText}");
            Console.WriteLine();
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
