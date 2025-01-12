namespace Core.Signals
{
    public abstract class SignalBase<TArg> : ISignal
    {
        public TArg Arg { get; }
            
        protected SignalBase(TArg arg)
        {
            Arg = arg;
        }
    }
    
    public abstract class SignalBase<TArg1, TArg2> : ISignal
    {
        public TArg1 Arg1 { get; }
        public TArg2 Arg2 { get; }
            
        protected SignalBase(TArg1 arg1, TArg2 arg2)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }
    }
}