using Core.Signals;

namespace App.Signals
{
    public sealed class UpdateUserNameSignal : ISignal
    {
        public string Name { get; }

        public UpdateUserNameSignal(string name)
        {
            Name = name;
        }
    }
}