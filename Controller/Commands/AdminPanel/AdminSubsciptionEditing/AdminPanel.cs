using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.Messages;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.DB;

namespace HabrPost.Controllers.Commands.Admin
{
    class AdminPanel : ICommand
    {
        TelegramBotClient ICommand.botClient => Bot.GetTelegramBot();

        string ICommand.Name => "/admin";

        public async Task Execute(Update update)
        {

            long chatId = CommandExecutor.GetChatId(update);
            //Если у пользователя есть права администратора
            if (await DBRequest.AdminVerify(chatId))
            {
                InlineKeyboardMarkup inlineKeyboard = new(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Новый пост", callbackData: "AdminNewPublication"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Управление подписками", callbackData: "AdminSubscriptionManager"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Управление Администраторами", callbackData: "AdminManager"),
                    }
                });

                if(update.Message != null)
                    await MessageController.SendInlineKeyboardMessage("Вы находитесь в панели администратора", inlineKeyboard, update);
                else if(update.CallbackQuery != null)
                    await MessageController.ReplaceInlineKeyboardMessageForMarkup("Вы находитесь в панели администратора", inlineKeyboard, update);
            }
        }
    }
}