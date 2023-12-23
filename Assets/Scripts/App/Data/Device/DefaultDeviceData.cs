using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace App.Data.Device
{
    public class DefaultDeviceData : IDeviceData
    {
        public string UserId {
            get
            {
                const string customUidKey = "defaultUidStorageKey";
                if (PlayerPrefs.HasKey(customUidKey))
                    return PlayerPrefs.GetString(customUidKey);

                var uid = $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}-{Random.Range(1000000, 9999999)}";
                PlayerPrefs.SetString(customUidKey, uid);
                return uid;
            }
        }
    }
}