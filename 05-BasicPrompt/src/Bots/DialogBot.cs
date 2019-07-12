﻿using EchoBot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Bots
{
    public class DialogBot : ActivityHandler
    {
        private readonly ConversationState _conversationState;
        private readonly UserState _userState;
        private readonly IStatePropertyAccessor<string> _usernameProperty;
        private readonly DialogSet _dialogs;

        public DialogBot(ConversationState conversationState, UserState userState)
        {
            _conversationState = conversationState;
            _userState = userState;

            _usernameProperty = _userState.CreateProperty<string>("username");

            var dialogStateProperty = _conversationState
                .CreateProperty<DialogState>(nameof(DialogState));
            _dialogs = new DialogSet(dialogStateProperty);
            _dialogs.Add(new EchoDialog(nameof(EchoDialog)));
            _dialogs.Add(new TextPrompt("usernamePrompt"));
        }

        protected async override Task OnMessageActivityAsync(
            ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var dc = await _dialogs.CreateContextAsync(turnContext);

            if (dc.ActiveDialog != null)
            {
                var result = await dc.ContinueDialogAsync();
                if(result.Status == DialogTurnStatus.Complete && result.Result is string username)
                    await _usernameProperty.SetAsync(turnContext, username, cancellationToken);
            }
            else
            {
                if (string.IsNullOrEmpty(
                    await _usernameProperty.GetAsync(turnContext, () => "", cancellationToken)))
                {
                    var message = MessageFactory.Text("What is your name?");
                    await dc.BeginDialogAsync("usernamePrompt", new PromptOptions { Prompt = message });
                }
                else
                    await dc.BeginDialogAsync(nameof(EchoDialog));
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and welcome!"), cancellationToken);
                }
            }
        }
    }
}
