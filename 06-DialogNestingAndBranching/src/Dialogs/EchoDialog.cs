using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Dialogs
{
    public class EchoDialog : ComponentDialog
    {
        public EchoDialog(string dialogId) : base(dialogId)
        {
        }

        public override Task<DialogTurnResult> BeginDialogAsync(
            DialogContext dc, 
            object options = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return ContinueDialogAsync(dc, cancellationToken);
        }

        public async override Task<DialogTurnResult> ContinueDialogAsync(
            DialogContext dc, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var message = MessageFactory.Text($"User said: {dc.Context.Activity.Text}");
            await dc.Context.SendActivityAsync(message, cancellationToken);
            return new DialogTurnResult(DialogTurnStatus.Waiting);
        }
    }
}
