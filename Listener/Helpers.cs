using Listener.Models;
using System;
using System.Linq;

namespace Listener
{
    public static class Helpers
    {
        public static Models.ChanceReq GetRequest(WaNotification waNotification)
        {
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"userNotification.Id: ", $"{waNotification.Id}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"CreationTime: ", $"{waNotification.CreationTime}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"Description: ", $"{waNotification.Description}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"DisplayName: ", $"{waNotification.DisplayName}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"titleText: ", $"{waNotification.TitleText}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"bodyText: ", $"{waNotification.BodyText}");
            Console.WriteLine();

            return new Models.ChanceReq
            {
                Address = new Models.Address
                {
                    StreetLocalName = waNotification.BodyText,
                    Building = -1,
                    CityId = 1,
                    CountryId = 367
                },
                Driver = new Models.Driver
                {
                    MobileNum = waNotification.BodyText.Split(':')[0]
                },
                Chance = new Models.Chance
                {
                    DateStart = waNotification.CreationTime.UtcDateTime.ToString()
                }
            };
        }

        public static bool IsValid(WaNotification waNotification)
        {
            var config = new Config.AppConfig();

            // whether the app is Chrome
            if (waNotification.DisplayName != "Google Chrome")
            {
                return false;
            }

            // where the group is parking group
            if (!config.Whatsapp.Groups.Exists(group => waNotification.TitleText.Contains(group, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            // where the body contain address
            if (!waNotification.BodyText.Split(':')[1].Any(char.IsDigit))
            {
                return false;
            }

            return true;
        }
    }
}
