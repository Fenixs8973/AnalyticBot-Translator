using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace HabrPost.Controllers
{
    [ApiController]
    [Route("/")]

    public class BotController : ControllerBase
    {        
        private UpdateDistributor updateDistributor = new UpdateDistributor();

        private TelegramBotClient bot = Bot.GetTelegramBot();

        
        [HttpPost]
        public async void Post(Update update)//Сюда будут приходить апдейты
        {
            if(update.Message == null && update.CallbackQuery == null)
                return;

            await updateDistributor.UpdateProcessing(update);//Ловит обновление(новое сообщение) 
        }
        [HttpGet]
        public string Get()
        {
            //Здесь мы пишем, что будет видно если зайти на адрес,
            //указаную в ngrok и launchSettings
            return "Telegram bot was started";
        }
    }
}
