using System;
using System.Collections;
using System.Threading;
using UnityEngine;

using Loxodon.Log;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Commands;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Contexts;
#if NETFX_CORE
using System.Threading.Tasks;
#endif

namespace Game.ViewModels
{
    public class Level : ViewModelBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ViewModelBase));

	    private int stageLevel = 0;
        private ProgressBar progressBar = new ProgressBar();
        private SimpleCommand command;
        private Localization localization;

        private InteractionRequest<Login> loginRequest;
        private InteractionRequest dismissRequest;

        public Level() : this(null)
        {
        }

        public Level(IMessenger messenger) : base(messenger)
        {
            ApplicationContext context = Context.GetApplicationContext();
            this.localization = context.GetService<Localization>();
            var accountService = context.GetService<Services.IAccount>();
            var globalPreferences = context.GetGlobalPreferences();

            this.loginRequest = new InteractionRequest<Login>(this);
            this.dismissRequest = new InteractionRequest(this);

            var loginViewModel = new Login(accountService, localization, globalPreferences);

            this.command = new SimpleCommand(() =>
            {
                this.command.Enabled = false;
                this.loginRequest.Raise(loginViewModel, vm =>
                {
                    this.command.Enabled = true;

                    if (vm.Account != null)
                        this.LoadScene();
                });
            });
        }

        public int StageLevel
        {
            get { return this.stageLevel; }
	        set { this.stageLevel = value; }
        }

        public ICommand Click
        {
            get { return this.command; }
        }

        public IInteractionRequest LoginRequest
        {
            get { return this.loginRequest; }
        }

        public IInteractionRequest DismissRequest
        {
            get { return this.dismissRequest; }
        }

        public void OnClick()
        {
            log.Debug("onClick");
            dismissRequest.Raise();
        }

        /// <summary>
        /// run on the background thread.
        /// </summary>
        /// <param name="promise">Promise.</param>
        protected void DoUnzip(IProgressPromise<float> promise)
        {
            var progress = 0f;
            while (progress < 1f)
            {
                progress += 0.01f;
                promise.UpdateProgress(progress);
#if NETFX_CORE
                Task.Delay(30).Wait();       
#else
                //Thread.Sleep (50);
                Thread.Sleep(30);
#endif
            }
            promise.SetResult();
        }

        /// <summary>
        /// Simulate a unzip task.
        /// </summary>
        public void Unzip()
        {
        }


        /// <summary>
        /// run on the main thread.
        /// </summary>
        /// <returns>The check.</returns>
        /// <param name="promise">Promise.</param>
        protected IEnumerator DoLoadScene(IProgressPromise<float> promise)
        {
            ResourceRequest request = Resources.LoadAsync<GameObject>("Scenes/Jungle");
            while (!request.isDone)
            {
                promise.UpdateProgress(request.progress);
                yield return null;
            }

            GameObject sceneTemplate = (GameObject)request.asset;
            GameObject.Instantiate(sceneTemplate);
            promise.UpdateProgress(1f);
            promise.SetResult();
        }

        /// <summary>
        /// Simulate a loading task.
        /// </summary>
        public void LoadScene()
        {
        }

    }
}
