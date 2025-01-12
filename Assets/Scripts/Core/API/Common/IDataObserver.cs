using System;

namespace Core.API.Common
{
    public delegate void DataChangedHandler<in TData>(TData data);
    
    public interface IDataObserver<out T> : IDisposable
    {
        event DataChangedHandler<T> OnChangeOccured;

        void Observe();
        void Stop();
    }
}