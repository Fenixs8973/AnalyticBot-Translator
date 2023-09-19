using Telegram.Bot;
using Telegram.Bot.Types;
using HabrPost.Controllers.Messages;
using HabrPost.Controllers.DB;
using System.Text.RegularExpressions;

namespace HabrPost.Controllers.Commands
{
    class AdminPublishNews : ICommand
    {
        public TelegramBotClient botClient => Bot.GetTelegramBot();

        public string Name => "AdminPublishNews";

        public async Task Execute(Update update)
        {
            string[] subscriptionArray = new string[0];
            Regex regex = new Regex($"\n-(.*);");
            string messageText = update.CallbackQuery.Message.Text;
            MatchCollection regexMatches = regex.Matches(messageText);
            if(regexMatches.Count > 0)
            {
                //Задали размер массива количеством подписок
                subscriptionArray = new string[regexMatches.Count];
                Match match = regex.Match(messageText);
                for(int i = 0; i < regexMatches.Count; i++)
                {
                    subscriptionArray[i] = match.Groups[1].ToString();
                }
            }
            
            string sqlReqest = $"SELECT tg_id FROM user_subscriptions JOIN subscriptions AS sub ON user_subscriptions.subscription_id = sub.id WHERE sub.title =";
            //Формирование SQL запроса
            if(subscriptionArray.Length == 0)
            {

            }
            else if(subscriptionArray.Length == 1)
            {
                sqlReqest += $" '{subscriptionArray[0]}'";
            }
            else if(subscriptionArray.Length > 1)
            {
                sqlReqest += $" '{subscriptionArray[0]}'";
                for(int i = 1; i < subscriptionArray.Length; i++)
                {
                    sqlReqest += $" '{subscriptionArray[i]}'";
                }
            }

            List<long> subs = await DBRequest.GetListSubs(sqlReqest);
            messageText = messageText.Remove(0, 13);
            messageText = "Вышел новый пост:\n\n" + messageText;
            messageText = messageText.Remove(messageText.IndexOf($"\n\nПолучатели:\n-"));
            foreach(long chatId in subs)
                await MessageController.SendSimpleMessage(messageText, update);

            //"Новый пост:\n\njhjhjjkl\n\nПолучатели:\n-тестовая;\n-vip;\n-njknjnj;"
            await MessageController.ReplaceInlineKeyboardMessageForText("Пост опубликован", update);
        }
    }
}