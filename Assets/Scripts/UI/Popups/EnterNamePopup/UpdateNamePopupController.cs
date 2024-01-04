using App.Signals;
using Core.UI.Screens;
using UnityEngine;
using Zenject;

namespace UI.Popups.EnterNamePopup
{
    public sealed class UpdateNamePopupController : ScreenControllerAbstract<EnterNamePopupView, UpdateNamePopupData>
    {
        private SignalBus _signalBus;

        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public override void Initialize()
        {
            View.OnNameConfirmed += NameConfirmedHandler;
            View.Initialize();
            View.SetDefaultName(Data.CurrentName);
        }

        public override void Dispose()
        {
            View.OnNameConfirmed -= NameConfirmedHandler;
        }

        public override void Show()
        {
            View.Show();
        }

        public override void Close()
        {
            Object.Destroy(View.gameObject);
            Dispose();
        }

        private void NameConfirmedHandler(string name)
        {
            Close();
            _signalBus.Fire(new UpdateUserNameSignal(name));
        }
    }
}