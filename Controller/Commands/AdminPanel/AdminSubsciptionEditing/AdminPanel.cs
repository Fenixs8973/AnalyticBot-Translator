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
            DBRequest db = new DBRequest();

            long chatId;
            //Получение chatId из текстового сообщения или из кнопки
            try
            {
                chatId = update.Message.Chat.Id;
            }
            catch (System.Exception)
            {
                chatId = update.CallbackQuery.Message.Chat.Id;
            }
            //Если у пользователя есть права администратора
            if (db.AdminVerify(chatId))
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
                MessageController mc = new MessageController();
                try
                {
                    CallbackQuery callback = update.CallbackQuery;
                    await mc.ReplaceInlineKeyboardMessageForMarkup("Вы находитесь в панели администратора", inlineKeyboard, update);
                }
                catch
                {
                    await mc.SendInlineKeyboardMessage("Вы находитесь в панели администратора", inlineKeyboard, update);
                }
            }
        }
    }
}