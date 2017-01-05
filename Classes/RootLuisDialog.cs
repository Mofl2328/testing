using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace movl_test_bot
{
    [LuisModel("2c60d7a6-cadc-42cd-9116-64f67c2bea7d", "34593796f7524f69b4592a14bfa8aa94")]
    public class RootLuisDialog : LuisDialog<object>
    {
        [LuisIntent("Hilfe")]
        public async Task HelpDialoge(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Ich war zwar in der Lage es zu verstehen kann aber nicht helfen.");

        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task DidntUnderstand(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Ich war nicht in der Lage die Nachricht zu verstehen. Bitte probiere es noch einmal.");
        }
    }
}