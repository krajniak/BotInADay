// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly ConversationState _conversationState;
        private readonly UserState _userState;
        private readonly IStatePropertyAccessor<int> _userMsgCount;
        private readonly IStatePropertyAccessor<int> _conversationMsgCount;

        public EchoBot(ConversationState conversationState, UserState userState)
        {
            _conversationState = conversationState;
            _userState = userState;

            _userMsgCount = _userState.CreateProperty<int>(nameof(_userMsgCount));
            _conversationMsgCount = _conversationState.CreateProperty<int>(nameof(_conversationMsgCount));
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var umc = await _userMsgCount.GetAsync(turnContext, () => 0, cancellationToken) + 1;
            var cmc = await _conversationMsgCount.GetAsync(turnContext, () => 0, cancellationToken) + 1;

            await _userMsgCount.SetAsync(turnContext, umc);
            await _conversationMsgCount.SetAsync(turnContext, cmc);

            await _conversationState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
            await _userState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);

            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);
            await turnContext.SendActivityAsync(MessageFactory.Text($"Number of messages from user: {umc}"), cancellationToken);
            await turnContext.SendActivityAsync(MessageFactory.Text($"Number of messages in conversation: {cmc}"), cancellationToken);
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
