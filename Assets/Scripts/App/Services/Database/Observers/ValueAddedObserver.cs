using Core.API.Common;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;

namespace App.Services.Database.Observers
{
    public sealed class ValueAddedObserver<T> : IDataObserver<T>
    {
        private readonly Query _databaseReference;
        
        private bool _retrieved;

        public event DataChangedHandler<T> OnChangeOccured;
        
        public ValueAddedObserver(Query databaseReference)
        {
            _databaseReference = databaseReference;
        }

        public void Dispose()
        {
            Stop();
        }

        public void Observe()
        {
            _databaseReference.ChildAdded += ChildAddedHandler;
        }

        public void Stop()
        {
            _databaseReference.ChildAdded -= ChildAddedHandler;
        }

        private void ChildAddedHandler(object sender, ChildChangedEventArgs e)
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