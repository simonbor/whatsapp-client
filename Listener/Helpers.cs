﻿using Listener.Models;
using System;
using System.Linq;

namespace Listener
{
    public static class Helpers
    {
        public static Models.ChanceReq GetRequest(WaNotification waNotification)
        {
            Console.WriteLine();
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"userNotification.Id: ", $"{waNotification.Id}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"CreationTime: ", $"{waNotification.CreationTime}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"Description: ", $"{waNotification.Description}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"DisplayName: ", $"{waNotification.DisplayName}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"titleText: ", $"{waNotification.TitleText}");
            ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, $"bodyText: ", $"{waNotification.BodyText}");
            Console.WriteLine();

            var cellPhoneLength = waNotification.BodyText.Split(':')[0].Length;
            var bodyText = waNotification.BodyText.Substring(cellPhoneLength + 1).Trim();

            return new Models.ChanceReq
            {
                Address = new Models.Address
                {
                    Text = bodyText,
                    CityId = 1,
                    CountryId = 367
                },
                Driver = new Models.Driver
                {
                    MobileNum = waNotification.BodyText.Split(':')[0].Trim()
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
            var cellPhoneLength = waNotification.BodyText.Split(':')[0].Length;
            var bodyText = waNotification.BodyText.Substring(cellPhoneLength + 1).Trim();

            // whether the app is Chrome
            if (waNotification.DisplayName != "Google Chrome")
            {
                ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray, 
                    $"{waNotification.Id} Wrong application name ({waNotification.DisplayName})");
                return false;
            }

            // where the group is parking group
            if (!config.Whatsapp.Groups.Exists(group => waNotification.TitleText.Contains(group, StringComparison.OrdinalIgnoreCase)))
            {
                ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray,
                    $"{waNotification.Id} Wrong groupName ({waNotification.TitleText})");
                return false;
            }

            // where the body contains address
            if (!bodyText.Any(char.IsDigit))
            {
                ConsoleProxy.WriteLine(null, ConsoleColor.DarkGray,
                    $"{waNotification.Id} Wrong address ({bodyText})");
                return false;
            }

            return true;
        }
    }
}
