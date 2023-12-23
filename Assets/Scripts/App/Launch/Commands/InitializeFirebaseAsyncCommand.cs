using System;
using System.Threading.Tasks;
using App.Data.Device;
using Core.Commands;
using Firebase;
using UnityEngine;
using Zenject;

namespace App.Launch.Commands
{
    public sealed class InitializeFirebaseAsyncCommand : ICommandAsync
    {
        [Inject] private IDeviceData _deviceData;
        
        public async Task Execute()
        {
            if (Application.isEditor)
                return;
               
            await InitializeAsync();
        }
        
        private async Task<DependencyStatus> InitializeAsync()
        {
            if (Application.isEditor)
                return DependencyStatus.UnavailableDisabled;

            var dependencyStatus = await FirebaseApp.CheckDependenciesAsync();

            if (dependencyStatus == DependencyStatus.Available) 
                return DependencyStatus.Available;
            
            Debug.LogWarning("Firebase DependencyStatus NotAvailable");
            await FirebaseApp.FixDependenciesAsync();
            dependencyStatus = await FirebaseApp.CheckDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.LogWarning("Firebase Dependency Fixed");
                return DependencyStatus.Available;
            }
           
            var exception = new Exception($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            Debug.LogError(exception);
            return DependencyStatus.UnavailableInvalid;
        }
    }
}