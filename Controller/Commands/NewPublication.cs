using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.Messages;
using Telegram.Bot.Types.ReplyMarkups;
using HabrPost.Controllers.DB;

namespace HabrPost.Controllers.Commands
{
    class NewPublication : ICommand
    {
        public static string news;
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "/new";

        public async Task Execute(Update update)
        {
            long chatId = update.Message.Chat.Id;
            MessageController mc = new MessageController();
            if(UpdateDistributor.redirection)
            {
                DBRequest db = new DBRequest();
                InlineKeyboardMarkup inlineKeyboard = new(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Опубликовать", callbackData: "PublishNews"),
                        InlineKeyboardButton.WithCallbackData(text: "Отложить(future)", callbackData: "PutAside"),
                    }
                });
                news = update.Message.Text;
                db.SQLInstructionSet(($"INSERT INTO news (news_text, status) VALUES ({news} , 0)"));
                mc.SendInlineKeyboardMessage($"Новый пост:\n\n{news}", inlineKeyboard, chatId);
                UpdateDistributor.redirection = false;
            }
            else
            {
                mc.SendSimpleMessage("Пришлите текст новости", chatId);
                UpdateDistributor.redirection = true;
            }
        }
    }
}