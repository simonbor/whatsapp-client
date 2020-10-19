using System;
using System.Text;
using System.Threading;

// https://blog.pieeatingninjas.be/2018/04/05/creating-a-uwp-console-app-in-c/
// https://docs.microsoft.com/en-us/windows/uwp/design/shell/tiles-and-notifications/notification-listener
namespace Listener
{
    class Program
    {
        private static readonly Config.AppConfig config = new Config.AppConfig();

        static void Main(string[] args)
        {
            var notificationHandler = new NotificationHandler();
            Console.OutputEncoding = Encoding.Unicode;

            if(!notificationHandler.ToastNotificationActionTriggerIsPresent())
            {
                Console.WriteLine("ToastNotificationActionTrigger isn't present. Check Windows settings.");
                return;
            }

            if (notificationHandler.RequestNotificationListenerAccess().Result)
            {
                notificationHandler.PerformNotifications(null);
            }

            if (config.App.CheckForNotificationPeriodically)
            {
                int delay = config.App.CheckForNotificationPeriod;
                ConsoleProxy.WriteLine(null, ConsoleColor.Yellow, $"Checking for notifications in loop every {delay / 1000} sec.");
                var timer = new Timer(notificationHandler.PerformNotifications, null, delay, delay);

                Console.ReadLine();
                Console.WriteLine("\nDestroying timer.");
                timer.Dispose();
            }
            else
            {
                try
                {
                    notificationHandler.SubscribeToWindowsNotifications();
                    ConsoleProxy.WriteLine(null, ConsoleColor.Gray, "\nWaiting for new notifications...");

                    Console.ReadLine();
                }
                catch (Exception e)
                {
                    ConsoleProxy.WriteLine(null, ConsoleColor.DarkRed, $"{e.Message}");
                }
            }
        }
    }
}
