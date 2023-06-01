using Telegram.Bot.Types;

namespace HabrPost.Controllers
{    
    public class UpdateDistributor<T> where T : ITelegramUpdateRedirection, new()
    {
        private static Dictionary<long, T>? redirections;

        public UpdateDistributor()
        {
            if(redirections == null)
                redirections = new Dictionary<long, T>();
        }
        public async Task UpdateProcessing(Update update)
        {
            long chatId = 0;
            if (update.Message != null)
                chatId = update.Message.Chat.Id;
            else if(update.CallbackQuery != null)
                chatId = update.CallbackQuery.Message.Chat.Id;
                
                T? redirection = redirections.GetValueOrDefault(chatId);
                if (redirection is null)
                {
                    redirection = new T();
                    redirections.Add(chatId, redirection);
                    await redirection.UpdateProcessing(update);
                    return;
                }
                await redirection.UpdateProcessing(update);
        }

        public static void RemoveRedirection(Update update)
        {
            long chatId;
            try
            {
                chatId = update.CallbackQuery.Message.Chat.Id;
            }
            catch
            {
                chatId = update.Message.Chat.Id;
            }
            redirections.Remove(chatId);
        }
    }
}