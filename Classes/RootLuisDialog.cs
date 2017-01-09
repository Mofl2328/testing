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

    [LuisModel("2bbf5697-9b51-45fb-bb4c-e2f66ca2f416", "34593796f7524f69b4592a14bfa8aa94")]
    public class RootLuisDialog : LuisDialog<object>
    {
        [LuisIntent("Hilfe")]
        public async Task HelpDialoge(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Ich war zwar in der Lage es zu verstehen kann aber nicht helfen.");

        }

        [LuisIntent("Auskunft")]
        public async Task AuskunftDialoge(IDialogContext context, LuisResult result)
        {
            foreach (var entity in result.Entities)
            {
                switch (entity.Type)
                {
                    case "ServiceKeyword":

                        break;
                    case "Funktion":

                        break;
                    case "Person":

                        break;
                    case "Zeitraum":
                        await context.PostAsync("Test: " + entity.Entity.ToLower());
                        break;
                    case "Zeiteinheit":

                        break;
                    case "Anzahl":

                        break;
                }
                //await context.PostAsync("Test: " + entity.Type + entity.Entity.ToLower());

            }


        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task DidntUnderstand(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Ich war nicht in der Lage die Nachricht zu verstehen. Bitte probiere es noch einmal.");
        }
    }
}