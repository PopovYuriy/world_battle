using System;
using Firebase.Database;

namespace App.Services.Database.Observers
{
    public delegate void DataChangedHandler<in TData>(DatabaseReference dataReference, TData data);
    
    public interface IDataObserver<out T> : IDisposable
    {
        event DataChangedHandler<T> OnChangeOccured;

        void Observe();
        void Stop();
    }
}