using Core.UI.Screens;
using UnityEngine;

namespace UI.Screens.LoadingScreen
{
    public sealed class LoadingScreenController : ScreenControllerAbstract<LoadingScreenView>
    {
        public override void Initialize() { }

        public override void Dispose() { }

        public override void Show() => View.Show();

        public override void Close() => Object.Destroy(View.gameObject);
    }
}