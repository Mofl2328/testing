using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using movl_test_bot.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace movl_test_bot
{
//    https://api.projectoxford.ai/luis/v2.0/apps/2bbf5697-9b51-45fb-bb4c-e2f66ca2f416?subscription-key=f553425f9dbf4534844c145fa0580cd8&verbose=true
    [LuisModel("2bbf5697-9b51-45fb-bb4c-e2f66ca2f416", "f553425f9dbf4534844c145fa0580cd8")]
    public class RootLuisDialog : LuisDialog<object>
    {
        [LuisIntent("Hilfe")]
        public async Task HelpDialoge(IDialogContext context, LuisResult result)
        {
            Dictionary<string, List<EntityRecommendation>> entitiesSorted = sortByEntity(result);
            await context.PostAsync("TODO");
            context.Done(true);

        }

        [LuisIntent("Aktion")]
        public async Task ActionDialoge(IDialogContext context, LuisResult result)
        {
            Dictionary<string, List<EntityRecommendation>> entitiesSorted = sortByEntity(result);

            bool responseGiven = false;
            foreach (var ent in entitiesSorted["ServiceKeyWord"])
            {
                if (KeyWords.actionOnWords.Contains(ent.Entity.ToLower()) && !responseGiven)
                {
                    foreach (EntityRecommendation function in entitiesSorted["Funktion"])
                    {
                        if (KeyWords.functionHolidayWords.Contains(function.Entity.ToLower()))
                        {
                            await setFunction(context, "urlaubsmodus", true);
                            responseGiven = true;
                        }
                    }

                }
                else if (KeyWords.actionOffWords.Contains(ent.Entity.ToLower()) && !responseGiven)
                {
                    foreach (EntityRecommendation function in entitiesSorted["Funktion"])
                    {
                        if (KeyWords.functionHolidayWords.Contains(function.Entity.ToLower()))
                        {
                            await setFunction(context, "urlaubsmodus", false);
                            responseGiven = true;
                        }
                    }

                }

            }
            context.Done(true);
        }

        private async Task setFunction(IDialogContext context, string function, bool set)
        {
            await context.PostAsync("app:setfunction:\"" + function + "\":" + set);
        }




        [LuisIntent("Auskunft")]
        public async Task AuskunftDialoge(IDialogContext context, LuisResult result)
        {


            Dictionary<string, List<EntityRecommendation>> entitiesSorted = sortByEntity(result);

            bool responseGiven = false;
            foreach (var ent in entitiesSorted["ServiceKeyWord"])
            {
                if (KeyWords.statusWords.Contains(ent.Entity.ToLower()) && !responseGiven)
                {
                    if (entitiesSorted["Person"].Count == 1)
                    {
                        if (KeyWords.allPersonsWords.Contains(entitiesSorted["Person"].First().Entity.ToLower()))
                        {
                            await sendStatusOfAll(context);
                            responseGiven = true;
                        } else
                        {
                            await sendStatusOfPerson(context, entitiesSorted["Person"].First().Entity.ToLower());
                            responseGiven = true;

                        }
                    } else if (entitiesSorted["Person"].Count > 1)
                    {
                        List<string> names = new List<string>();
                        foreach (EntityRecommendation person in entitiesSorted["Person"])
                        {
                            if (!KeyWords.allPersonsWords.Contains(person.Entity.ToLower()))
                            {
                                names.Add(person.Entity.ToLower());
                            }
                        }
                        await sendStatusOfPerson(context, names);
                        responseGiven = true;
                    }
                }
            }
            context.Done(true);
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task DidntUnderstand(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Ich war nicht in der Lage die Nachricht zu verstehen. Frag doch mal wie es den Personen geht, die du betreust.");
            context.Done(true);

        }


        private async Task sendStatusOfAll(IDialogContext context)
        {
            await context.PostAsync("app:OpenStatusOfAll");
        }

        private async Task sendStatusOfPerson(IDialogContext context, List<string> names)
        {
            string nameString = "";

            foreach (string name in names)
            {
                if (nameString != "")
                {
                    nameString += ", " + name;
                }
                else
                {
                    nameString = name;
                }
            }

            await context.PostAsync("app:OpenStatusOf{" + nameString + "}");
        }

        private async Task sendStatusOfPerson(IDialogContext context, string name)
        {
            await context.PostAsync("app:OpenStatusOf{" + name + "}");
        }

        private Dictionary<string, List<EntityRecommendation>> sortByEntity(LuisResult result)
        {

            Dictionary<string, List<EntityRecommendation>> entitiesSorted = new Dictionary<string, List<EntityRecommendation>>();

            string[] types = { "ServiceKeyWord", "Funktion", "Person", "Zeitraum", "Zeiteinheit", "Anzahl" };

            foreach(string type in types)
            {
                entitiesSorted.Add(type, new List<EntityRecommendation>());
            }

            foreach (var entity in result.Entities)
            {
                entitiesSorted[entity.Type].Add(entity);
            }

            return entitiesSorted;

        }
    }
}