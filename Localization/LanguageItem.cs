﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Rasyidf.Localization
{
    public class LanguageItem
    {
        public static LanguageItem Default = new LanguageItem()
        {
            CultureName = CultureInfo.InstalledUICulture.NativeName,
            EnglishName = CultureInfo.InstalledUICulture.EnglishName,
            CultureId = CultureInfo.InstalledUICulture.Name
        };

        public CultureInfo Culture => CultureInfo.GetCultureInfo(CultureId);

        #region Public Methods

        public TValue Translate<TValue>(string uid, string vid)
        {
            return (TValue)Translate(uid, vid, null, typeof(TValue));
        }

        public TValue Translate<TValue>(string uid, string vid, TValue defaultValue)
        {
            try
            {
                return (TValue)Translate(uid, vid, defaultValue, typeof(TValue));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public object Translate(string uid, string valueId, object defaultValue, Type type)
        {
            return OnTranslate(uid, valueId, defaultValue, type);
        }

        public static LanguageItem GetResources(CultureInfo cultureInfo)
        {
            if (cultureInfo is null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            if (!LanguageService.RegisteredPacks.ContainsKey(cultureInfo)) return Default;

            LanguageItem dictionary = LanguageService.RegisteredPacks[cultureInfo];
            return dictionary;
        }

        #endregion Public Methods
         
        public string Version { get; set; }
        public string CultureId { get; set; }
        public string CultureName { get; set; }
        public string EnglishName { get; set; }


        internal Dictionary<string, Dictionary<string, string>> Data =
            new Dictionary<string, Dictionary<string, string>>();

        private object OnTranslate(string uid, string vid, object defaultValue, Type type)
        {
            if (string.IsNullOrEmpty(uid))
            {
                #region Trace

                Debug.Print("Uid must not be null or empty");

                #endregion Trace

                return defaultValue;
            }
            if (string.IsNullOrEmpty(vid))
            {
                #region Trace

                Debug.WriteLine(string.Format("Vid must not be null or empty"));

                #endregion Trace

                return defaultValue;
            }
            if (!Data.ContainsKey(uid))
            {
                #region Trace

                Debug.WriteLine($"Uid {uid} was not found in the {EnglishName} dictionary");

                #endregion Trace

                return defaultValue;
            }

            Dictionary<string, string> innerData = Data[uid];

            if (!innerData.ContainsKey(vid))
            {
                #region Trace

                Debug.WriteLine($"Vid {vid} was not found for Uid {uid}, in the {EnglishName} dictionary");

                #endregion Trace

                return defaultValue;
            }
            string textValue = innerData[vid];
            try
            {
                if (type == typeof(object))
                    return textValue;


                return TypeDescriptor.GetConverter(type)
                                     .ConvertFromString(textValue);

            }
            catch (Exception ex)
            {
                #region Trace

                Debug.WriteLine($"Failed to translate text {textValue} in dictionary {EnglishName}:\n{ex.Message}");

                #endregion Trace

                return defaultValue;
            }
        }
    }
} 