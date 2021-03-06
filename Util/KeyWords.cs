﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace movl_test_bot.Util
{
    public static class KeyWords
    {
        public static string[] statusWords = { "ist", "war", "geht", "ging", "ordnung", "waren" };
        public static string[] allPersonsWords = { "alle", "allen", "jedem", "jede", "alles", "personen", "betreuten" };
        public static string[] actionOnWords = { "aktiviere", "aktivier", "aktivieren", "an", "ein" };
        public static string[] actionOffWords = { "deaktiviere", "deaktivier", "deaktivieren", "aus", "ab" };
        public static string[] actionAddWords = { "füge", "fügen" };
        public static string[] functionHolidayWords = { "urlaub", "urlaubs", "ferien" };
        public static string[] functionHelperWords = { "helfer", "hilfen", "betreuer" };
    }
}