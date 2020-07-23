using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Notifications;

namespace Listener
{
    public static class Helpers
    {
        public static Models.ChanceReq GetRequest(UserNotification userNotification)
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

            return new Models.ChanceReq
            {
                Address = new Models.Address
                {
                    StreetLocalName = bodyText,
                    Building = -1,
                    CityId = 1,
                    CountryId = 367
                },
                Driver = new Models.Driver
                {
                    MobileNum = bodyText.Split(':')[0]
                },
                Chance = new Models.Chance
                {
                    DateStart = userNotification.CreationTime.UtcDateTime.ToString()
                }
            };
        }

        public static bool IsValid(Models.ChanceReq request)
        {
            // ...

            return true;
        }
    }
}
