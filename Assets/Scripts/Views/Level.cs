using UnityEngine;
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
    using ViewModel = ViewModels.Level;

    public class Level : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Loading));

        public Button plusBtn;
        public Button minusBtn;
        public Button closeBtn;
        public Button confirmBtn;
        public Button openCupboardBtn;
	    public Text answerText;
	    public Animation answerAnimation;
	    public Animator animator;

        private ViewModel viewModel;
        private IDisposable subscription;

        private IUIViewLocator viewLocator;

        protected override void OnCreate(IBundle bundle)
        {
            this.viewLocator = Context.GetApplicationContext().GetService<IUIViewLocator>();
            this.viewModel = new ViewModel();

            BindingSet<Level, ViewModel> bindingSet = this.CreateBindingSet(viewModel);
            bindingSet.Bind().For(v => v.OnDismissRequest).To(vm => vm.DismissRequest);
            bindingSet.Bind().For(v => v.OnAnswerWrong).To(vm => vm.AnswerWrongRequest);

	        bindingSet.Bind(this.plusBtn).For(v => v.onClick).To(vm => vm.doPlus).OneWay();
	        bindingSet.Bind(this.minusBtn).For(v => v.onClick).To(vm => vm.doMinus).OneWay();
            bindingSet.Bind(this.closeBtn).For(v => v.onClick).To(vm => vm.doClose).OneWay(); 
            bindingSet.Bind(this.confirmBtn).For(v => v.onClick).To(vm => vm.doConfirm).OneWay(); 
            bindingSet.Bind(this.answerText).For(v => v.text).To(vm => vm.AnswerValue).OneWay(); 

            bindingSet.Build();
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

        protected void OnDismissRequest(object sender, InteractionEventArgs args)
        {
            this.Dismiss();
        }

	    protected void OnAnswerWrong(object sender, InteractionEventArgs args)
	    {
	        var suc = this.answerAnimation.Play("wrong");
	    }

	    //public void openCupboard(object sender, InteractionEventArgs args)
	    public void openCupboard()
	    {
	        var isOpen = this.animator.GetCurrentAnimatorStateInfo(0).IsName("openCupboard");
	        this.animator.SetTrigger(isOpen ? "close" : "open");
	    }
    }
}
