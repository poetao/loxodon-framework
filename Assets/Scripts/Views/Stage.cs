﻿using UnityEngine;
using UnityEngine.UI;
using System;

using Loxodon.Log;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;

namespace Game.Views
{
    using ViewModel = ViewModels.Stage;

    public class Stage : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Loading));

        public Text progressBarText;
        public Slider progressBarSlider;
        public Text tipText;
        public Button button;

        private ViewModel viewModel;
        private IDisposable subscription;

        private IUIViewLocator viewLocator;

        protected override void OnCreate(IBundle bundle)
        {
            this.viewLocator = Context.GetApplicationContext().GetService<IUIViewLocator>();
            this.viewModel = new ViewModel();

            //this.subscription = this.viewModel.Messenger.Subscribe ();

            //this.SetDataContext (viewModel);

            /* databinding, Bound to the ViewModel. */
            BindingSet<Stage, ViewModel> bindingSet = this.CreateBindingSet(viewModel);
            bindingSet.Bind().For(v => v.OnOpenLoginWindow).To(vm => vm.LoginRequest);
            bindingSet.Bind().For(v => v.OnDismissRequest).To(vm => vm.DismissRequest);

            //bindingSet.Bind(this.progressBarSlider).For("value", "onValueChanged").To("ProgressBar.Progress").TwoWay();
            //bindingSet.Bind (this.progressBarSlider).For (v => v.value, v => v.onValueChanged).To (vm => vm.ProgressBar.Progress).TwoWay ();

            /* //by the way,You can expand your attributes. 		 
		        ProxyFactory proxyFactory = ProxyFactory.Default;
		        PropertyInfo info = typeof(GameObject).GetProperty ("activeSelf");
		        proxyFactory.Register (new ProxyPropertyInfo<GameObject, bool> (info, go => go.activeSelf, (go, value) => go.SetActive (value)));
		    */

            //bindingSet.Bind(this.progressBarSlider.gameObject).For(v => v.activeSelf).To(vm => vm.ProgressBar.Enable).OneWay();
            //bindingSet.Bind(this.progressBarText).For(v => v.text).ToExpression(vm => string.Format("{0}%", Mathf.FloorToInt(vm.ProgressBar.Progress * 100f))).OneWay();/* expression binding,support only OneWay mode. */
            //bindingSet.Bind(this.tipText).For(v => v.text).To(vm => vm.ProgressBar.Tip).OneWay();

            //bindingSet.Bind(this.button).For(v => v.onClick).To(vm=>vm.OnClick()).OneWay(); //Method binding,only bound to the onClick event.
            //bindingSet.Bind(this.button).For(v => v.onClick).To(vm => vm.Click).OneWay();//Command binding,bound to the onClick event and interactable property.
            bindingSet.Build();

            this.viewModel.Unzip();
        }

        public override void DoDismiss()
        {
            base.DoDismiss();
            if (this.subscription != null)
            {
                this.subscription.Dispose();
                this.subscription = null;
            }
        }

        public void OnOpenLevel(int level)
        {
            var window = viewLocator.LoadWindow<Level>(this.WindowManager, $"UI/Level{level}");
            window.Create();
            window.Show();
	        window.viewModel.StageLevel = level; 
        }

        protected void OnDismissRequest(object sender, InteractionEventArgs args)
        {
            this.Dismiss();
        }

        protected void OnOpenLoginWindow(object sender, InteractionEventArgs args)
        {
            try
            {
                var loginWindow = viewLocator.LoadWindow<Login>(this.WindowManager, "UI/Login");
                var callback = args.Callback;
                var loginViewModel = args.Context;

                if (callback != null)
                {
                    EventHandler handler = null;
                    handler = (window, e) =>
                    {
                        loginWindow.OnDismissed -= handler;
                        if (callback != null)
                            callback();
                    };
                    loginWindow.OnDismissed += handler;
                }

                loginWindow.SetDataContext(loginViewModel);
                loginWindow.Create();
                loginWindow.Show();
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.Warn(e);
            }
        }
    }
}
