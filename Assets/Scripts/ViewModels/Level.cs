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
        private int answerValue = 0;
	    private int rightAnswer = 7;

        private InteractionRequest dismissRequest;
        private InteractionRequest answerWrongRequest;

        public int StageLevel
        {
            get { return this.stageLevel; }
	        set { this.stageLevel = value; }
        }

	    public int AnswerValue { 
            get { return this.answerValue; }
	        set { this.Set(ref this.answerValue, value, nameof(AnswerValue)); }
        }

        public Level() : this(null)
        {
        }

        public Level(IMessenger messenger) : base(messenger)
        {
	        this.AnswerValue = 1;
            this.dismissRequest = new InteractionRequest(this);
            this.answerWrongRequest = new InteractionRequest(this);
        }

        public IInteractionRequest DismissRequest
        {
            get { return this.dismissRequest; }
        }

	    public IInteractionRequest AnswerWrongRequest
	    {
	        get { return this.answerWrongRequest; }
	    }

        public void doClose()
        {
            dismissRequest.Raise();
        }

	    public void doPlus()
	    {
	        this.AnswerValue += 1;        
	    }

	    public void doMinus()
	    {
	        this.AnswerValue = Math.Max(0, this.AnswerValue - 1);        
	    }

	    public void doConfirm()
	    {
	        if (this.AnswerValue != this.rightAnswer)
    		{
                this.answerWrongRequest.Raise();		        
		        return;
		    }
	    }
    }
}
