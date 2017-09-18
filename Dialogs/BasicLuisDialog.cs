using System;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Text;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        
        private bool hasGreetingBeenSaid = false;

        public bool HasGreetingBeenSaid
        {
            get { return hasGreetingBeenSaid; }
            set { hasGreetingBeenSaid = value; }
        }

        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(ConfigurationManager.AppSettings["LuisAppId"], ConfigurationManager.AppSettings["LuisAPIKey"])))
        {
            
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached the none intent. You said: {result.Query}"); //
            context.Wait(MessageReceived);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "MyIntent" with the name of your newly created intent in the following handler
        [LuisIntent("greeting")]
        public async Task MyIntent(IDialogContext context, LuisResult result)
        {
            //await context.PostAsync($"{HasGreetingBeenSaid} is the current value");
           IMessageActivity activity = await CommonChecks(context, result);
           int nullTest;
           if (activity == null) {nullTest = 1; }
           else { nullTest = 2; }

           var sb = new StringBuilder();
           //sb.AppendLine($"{result.Query} {activity.Recipient.Name} to you to valued Microsoft employee");
            sb.AppendLine($"{result.Query} to you to valued Microsoft employee /n");
            sb.AppendLine($"I am the New Employee Bot and I am here to assist you during you orientation.  Is it null {nullTest}");
           await context.PostAsync(sb.ToString()); //

            #region hbs
            HasGreetingBeenSaid = true;
            #endregion

            context.Wait(MessageReceived);
        }

        private async Task<IMessageActivity> CommonChecks(IDialogContext context, LuisResult result)
        {
            HasGreetingBeenSaid = false;
            if (HasGreetingBeenSaid == true)
            {
                await context.PostAsync($"We have already been introduced.  What can I do to help you? {HasGreetingBeenSaid}");
                return null;
            }

            IDialogContext returnContext = context;
            LuisResult returnResult = result;
            return scope.Resolve<Queue<IMessageActivity>>().Dequeue();
            //return await CommonChecks(context, result); //.ConfigureAwait(false);
        }
    }
}