using HabrPost.Controllers.Commands;
using Telegram.Bot.Types;

namespace HabrPost.Controllers
{
    public class UpdateDistributor
    {
        static public bool redirection;
        List<ICommand> commandsList = new List<ICommand>()
        {
            new StartCommand(),
            new QuerySubscribe(),
            new QueryUnSubscribe(),
            new NewPublication(),
            new SubscriptionManagement(),
            new PublishNews()
        };
        
        public async Task UpdateProcessing(Update update)
        {
            if(update.Message != null)
            {
                if(redirection)
                {
                    NewPublication nw = new NewPublication();
                    nw.Execute(update);
                }
                else
                {
                    Message message = update.Message;
                    foreach (var command in commandsList)
                    {
                        if(message.Text == command.Name)
                        {
                            await command.Execute(update);
                            break;
                        }
                    }
                }
            }
            else
            {
                CallbackQuery callBack = update.CallbackQuery;
                foreach (var command in commandsList)
                {
                    if(callBack.Data == command.Name)
                    {
                        await command.Execute(update);
                        break;
                    }
                }
            }
        }
    }
}