using HabrPost.Controllers.DB;
using HabrPost.Controllers.Messages;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace HabrPost.Controllers.Commands.Admin
{
    class AdminSubscriptionNew : ICommand, IRedirection
    {

        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminSubscriptionNew";

        public CommandExecutor Executor { get; }
        
        public AdminSubscriptionNew(CommandExecutor executor)
        {
            Executor = executor;
        }

        string? title = null;
        string? description = null;
        int price = 0;

        public async Task Execute(Update update)
        {
            await MessageController.SendSimpleMessage("Введите название подписки", update);
            Executor.StartListen(this);
        }
        public async Task Redirection(Update update)
        {
            if(title == null)
            {
                title = update.Message.Text;
                await MessageController.SendSimpleMessage("Введите описание для подписки", update);
            }
            else if(description == null)
            {
                description = update.Message.Text;
                await MessageController.SendSimpleMessage("Введите цену для подписки", update);
            }
            else
            {
                if(!Int32.TryParse(update.Message.Text, out price))
                {
                    await MessageController.SendSimpleMessage("Не верная цена. Введите цену для подписки", update);
                    return;
                }
                await DBRequest.SQLInstructionSet($"INSERT INTO subscriptions (title, description, price) VALUES ('{title}', '{description}', '{price}')");
                InlineKeyboardMarkup inlineKeyboardMarkup = new(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Меню", callbackData: "/admin")
                    }
                });
                await MessageController.SendInlineKeyboardMessage("Подписка успешно добавлена", inlineKeyboardMarkup, update);
                Executor.StopListen(update);
            }
        }
    }
}