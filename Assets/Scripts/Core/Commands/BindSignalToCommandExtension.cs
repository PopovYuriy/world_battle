using System;
using Tools.CSharp;
using Zenject;

namespace Core.Commands
{
    public static class BindSignalToCommandExtension
    {
        public static void BindSignalToCommand<TSignal, TCommand>(this DiContainer container) where TCommand : ICommand
        {
            var signalType = typeof(TSignal);
            BindSignalToCommandImpl(container, signalType, typeof(TCommand), false);
        }
        
        public static void BindSignalToAsyncCommand<TSignal, TCommand>(this DiContainer container) where TCommand : ICommandAsync
        {
            var signalType = typeof(TSignal);
            BindSignalToCommandImpl(container, signalType, typeof(TCommand), false);
        }

        public static void DeclareSignalAndBindToCommand<TSignal, TCommand>(this DiContainer container)
            where TCommand : ICommand
        {
            var signalType = typeof(TSignal);
            var signalDeclarationInfo = SignalExtensions.CreateDefaultSignalDeclarationBindInfo(container, signalType);
            container.Bind<SignalDeclaration>()
                .AsCached()
                .WithArguments(signalDeclarationInfo)
                .WhenInjectedInto(typeof(SignalBus), typeof(SignalDeclarationAsyncInitializer));
            BindSignalToCommandImpl(container, signalType, typeof(TCommand), false);
        }
        
        public static void DeclareSignalAndBindToAsyncCommand<TSignal, TCommand>(this DiContainer container)
            where TCommand : ICommandAsync
        {
            var signalType = typeof(TSignal);
            var signalDeclarationInfo = SignalExtensions.CreateDefaultSignalDeclarationBindInfo(container, signalType);
            container.Bind<SignalDeclaration>()
                .AsCached()
                .WithArguments(signalDeclarationInfo)
                .WhenInjectedInto(typeof(SignalBus), typeof(SignalDeclarationAsyncInitializer));
            BindSignalToCommandImpl(container, signalType, typeof(TCommand), true);
        }

        static void BindSignalToCommandImpl(DiContainer container, Type signalType, Type commandType, bool isAsyncCommand)
        {
            var signalBindInfo = new SignalBindingBindInfo(signalType);
            var bindStatement = container.StartBinding();
            bindStatement.SetFinalizer(new NullBindingFinalizer());
            container.Bind(commandType).AsTransient();
            container.Bind<IDisposable>()
                .To<SignalCallbackWrapper>()
                .AsCached()
                // Note that there's a reason we don't just make SignalCallbackWrapper have a generic
                // argument for signal type - because when using struct type signals it throws
                // exceptions on AOT platforms
                .WithArguments(signalBindInfo, (Action<object>)(o =>
                {
                    var signalType = o.GetType();
                    container.Bind(signalType).FromInstance(o).WhenInjectedInto(commandType);

                    if (isAsyncCommand)
                    {
                        var command = (ICommandAsync)container.Resolve(commandType);
                        command.Execute().Run();
                    }
                    else
                    {
                        var command = (ICommand)container.Resolve(commandType);
                        command.Execute();
                    }
                    
                    container.Unbind(signalType);
                }))
                .NonLazy();
        }
    }
}