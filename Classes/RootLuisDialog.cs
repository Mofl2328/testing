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
            bool responseGiven = false;
            foreach (var ent in entitiesSorted["Funktion"])
            {
                var entString = ent.Entity.ToLower();

                if (KeyWords.functionHolidayWords.Contains(entString) && !responseGiven)
                {
                    await context.PostAsync("Der Urlaubsmodus ist unter Profil -> Optionen -> Anwesenheit aktivierbar.");
                    responseGiven = true; context.Done(true);
                }
                else if (KeyWords.functionHelperWords.Contains(entString) && !responseGiven)
                {
                    await context.PostAsync("Sie können neue Helfer auf der Startseite über das + in der rechten oberen Ecke hinzufügen.");
                    responseGiven = true; context.Done(true);
                }
            }
            if (!responseGiven)
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
                            responseGiven = true; context.Done(true);
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
                            responseGiven = true; context.Done(true);
                        }
                    }

                }
                else if (KeyWords.actionAddWords.Contains(ent.Entity.ToLower()) && !responseGiven)
                {
                    foreach (EntityRecommendation function in entitiesSorted["Funktion"])
                    {
                        if (KeyWords.functionHelperWords.Contains(function.Entity.ToLower()))
                        {
                            PromptDialog.Text(context, addHelperDialogInput, "Wen möchten sie hinzufügen?"); ;
                            responseGiven = true;
                        }
                    }

                }

            }
            if (!responseGiven)
                context.Done(true);

        }

        private async Task setFunction(IDialogContext context, string function, bool set)
        {
            await context.PostAsync("app:setfunction:\"" + function + "\":" + set);
        }

        private void addHelperDialogStart(IDialogContext context)
        {
            PromptDialog.Text(context, addHelperDialogInput, "Wen möchten sie hinzufügen?");
        }


        private async Task addHelperDialogInput(IDialogContext context, IAwaitable<string> result)
        {
            var person = await result;
            if (person.Contains(","))
            {
                var persons = person.Split(new Char[] { ',' });

                await addHelperDialogEnd(context, new List<string>(persons));
            } else
            {
                var persons = new List<string>();
                persons.Add(person);
                await addHelperDialogEnd(context, persons);

            }

        }

        private async Task addHelperDialogEnd(IDialogContext context, List<string> persons)
        {
            string personsString = "";
            foreach (var person in persons)
            {
                if (personsString == "")
                {
                    personsString = "\"" + person + "\"";
                }
                else
                {
                    personsString += ", \"" + person + "\"";

                }
            }

            await context.PostAsync("app:addHelper:{" + personsString + "}");
            context.Done(true);

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
                            responseGiven = true; context.Done(true);
                        } else
                        {
                            await sendStatusOfPerson(context, entitiesSorted["Person"].First().Entity.ToLower());
                            responseGiven = true; context.Done(true);

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
                        responseGiven = true; context.Done(true);
                    }
                }
            }
            if (!responseGiven)
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
                    nameString += "," + name;
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