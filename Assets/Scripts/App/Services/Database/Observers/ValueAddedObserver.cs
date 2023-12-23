using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;

namespace App.Services.Database.Observers
{
    public sealed class ValueAddedObserver<T> : IDataObserver<T>
    {
        private readonly Query _databaseReference;

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
            
            if (e.Snapshot.Value == null)
                return;

            T value = JsonConvert.DeserializeObject<T>(e.Snapshot.GetRawJsonValue());
            OnChangeOccured?.Invoke(e.Snapshot.Reference, value);
        }
    }
}