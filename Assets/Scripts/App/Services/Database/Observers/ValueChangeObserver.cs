using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;

namespace App.Services.Database.Observers
{
    public sealed class ValueChangeObserver<T> : IDataObserver<T>
    {
        private readonly Query _databaseReference;

        private bool _retrieved;

        public event DataChangedHandler<T> OnChangeOccured;

        public ValueChangeObserver(Query databaseReference)
        {
            _databaseReference = databaseReference;
        }

        public void Dispose()
        {
            Stop();
        }

        public void Observe()
        {
            _databaseReference.ValueChanged += ValueChangedHandler;
        }

        public void Stop()
        {
            _databaseReference.ValueChanged -= ValueChangedHandler;
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

            T value = JsonConvert.DeserializeObject<T>(e.Snapshot.GetRawJsonValue());
            OnChangeOccured?.Invoke(e.Snapshot.Reference, value);
        }
    }
}