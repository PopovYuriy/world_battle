using Firebase.Database;
using UnityEngine;

namespace App.Services.Database.Observers
{
    public sealed class ValueRemovedObserver<T> : IDataObserver<T>
    {
        private readonly Query _databaseReference;
        
        public event DataChangedHandler<T> OnChangeOccured;

        public ValueRemovedObserver(Query databaseReference)
        {
            _databaseReference = databaseReference;
        }

        public void Dispose()
        {
            Stop();
        }

        public void Observe()
        {
            _databaseReference.ChildRemoved += ValueRemovedHandler;
        }

        public void Stop()
        {
            _databaseReference.ChildRemoved -= ValueRemovedHandler;
        }

        private void ValueRemovedHandler(object sender, ChildChangedEventArgs e)
        {
            if (e.DatabaseError != null)
            {
                Debug.LogError(e.DatabaseError.Message);
                return;
            }

            OnChangeOccured?.Invoke(e.Snapshot.Reference, default);
        }
    }
}