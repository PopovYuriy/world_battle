using System;
using System.Collections.Generic;
using Zenject;

namespace Core.Modules
{
    public sealed class ModulesResolver
    {
        private readonly DiContainer                          _container;
        private readonly Dictionary<Type, IModuleInitializer> _moduleInitializers;

        private bool _resolved;
        private bool _disposed;

        private Action _onComplete;
        
        public ModulesResolver(DiContainer container)
        {
            _container = container;
            _moduleInitializers = new Dictionary<Type, IModuleInitializer>();
        }
        
        public void Dispose()
        {
            _moduleInitializers.Clear();
            _disposed = true;
        }

        public ModulesResolver AddModuleWithInstaller<TInstaller, TModule>()
            where TInstaller : ModuleInstaller
            where TModule : IModule
        {
            if (_disposed)
                throw new ObjectDisposedException($"{nameof(ModulesResolver)} is disposed.");
            
            if (_resolved)
                throw new InvalidOperationException($"{nameof(ModulesResolver)} is already resolved. You should add modules before resolving.");
            
            if (!_moduleInitializers.TryAdd(typeof(TInstaller), new ModuleInitializerWithInstaller<TInstaller, TModule>(_container)))
                throw new ArgumentException($"Type {typeof(TInstaller)} already registered.");

            return this;
        }
        
        public ModulesResolver AddModule<TModule>()
            where TModule : IModule
        {
            if (_disposed)
                throw new ObjectDisposedException($"{nameof(ModulesResolver)} is disposed.");
            
            if (_resolved)
                throw new InvalidOperationException($"{nameof(ModulesResolver)} is already resolved. You should add modules before resolving.");
            
            if (!_moduleInitializers.TryAdd(typeof(TModule), new ModuleInitializer<TModule>(_container)))
                throw new ArgumentException($"Type {typeof(TModule)} already registered.");

            return this;
        }

        public ModulesResolver AddModuleInstaller<TInstaller>()
            where TInstaller : ModuleInstaller
        {
            if (_disposed)
                throw new ObjectDisposedException($"{nameof(ModulesResolver)} is disposed.");
            
            if (_resolved)
                throw new InvalidOperationException($"{nameof(ModulesResolver)} is already resolved. You should add modules before resolving.");
            
            if (!_moduleInitializers.TryAdd(typeof(TInstaller), new ModuleInitializerWithInstallerOnly<TInstaller>(_container)))
                throw new ArgumentException($"Type {typeof(TInstaller)} already registered.");

            return this;
        }

        public ModulesResolver Resolve(Action onComplete)
        {
            if (_disposed)
                throw new ObjectDisposedException($"{nameof(ModulesResolver)} is disposed.");
            
            if (_resolved)
                throw new InvalidOperationException($"{nameof(ModulesResolver)} is already resolved.");
            
            _onComplete = onComplete;
            
            ProjectContext.PostResolve += ProjectContextPostResolveHandler;
            
            InstallModules();
            
            return this;
        }

        private void ProjectContextPostResolveHandler()
        {
            ProjectContext.PostResolve -= ProjectContextPostResolveHandler;

            InitializeModules();
            
            _onComplete?.Invoke();
        }

        private void InstallModules()
        {
            foreach (var moduleInitializer in _moduleInitializers.Values)
                moduleInitializer.Install();
        }

        private void InitializeModules()
        {
            foreach (var moduleInitializer in _moduleInitializers.Values)
                moduleInitializer.Initialize();
        }
    }
}