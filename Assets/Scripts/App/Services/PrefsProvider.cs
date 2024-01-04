using UnityEngine;

namespace App.Services
{
    public static class PrefsProvider
    {
        private const string FirstTimeNameChangedKey = "first_time_name_changed";
        
        public static bool FirstTimeNameChanged
        {
            get => PlayerPrefs.HasKey(FirstTimeNameChangedKey);
            set => PlayerPrefs.SetInt(FirstTimeNameChangedKey, 1);
        }
    }
}