using Core.API.Common;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;

namespace App.Services.Database.Observers
{
    public sealed class ValueChangeObserver<T> : IDataObserver<T>
    {
        private readonly Query _databaseQuery;

        private bool _retrieved;

        public event DataChangedHandler<T> OnChangeOccured;

        public ValueChangeObserver(Query databaseQuery)
        {
            _databaseQuery = databaseQuery;
        }

        public void Dispose()
        {
            Stop();
        }

        public void Observe()
        {
            _databaseQuery.ValueChanged += ValueChangedHandler;
        }

        public void Stop()
        {
            _databaseQuery.ValueChanged -= ValueChangedHandler;
        }

        private void ValueChangedHandler(object sender, ValueChangedEventArgs e)
        {
            if (e.DatabaseError != null)
            {
                Debug.LogError(e.DatabaseError.Message);
                return;
            }
            
            if (!_retrieved)
            {
                _retrieved = true;
                return;
            }
            
            if (e.Snapshot.Value == null)
                return;

            var value = JsonConvert.DeserializeObject<T>(e.Snapshot.GetRawJsonValue());
            OnChangeOccured?.Invoke(value);
        }
    }
}