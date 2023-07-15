using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot;
using HabrPost.Controllers.Messages;

namespace HabrPost.Controllers
{
    [ApiController]
    [Route("/")]

    public class BotController : ControllerBase
    {        
        private TelegramBotClient bot = Bot.GetTelegramBot();
        private UpdateDistributor<CommandExecutor> updateDistributor = new UpdateDistributor<CommandExecutor>();

        
        [HttpPost]
        public async void Post(Update update)//Сюда будут приходить апдейты
        {
            if(update.Message != null)
            {
                if(update.Message.SuccessfulPayment != null)
                {
                    InvoicePayloadController invoicePayload = new InvoicePayloadController();
                    await invoicePayload.SuccessfulPayment(update);
                    return;
                }
                await updateDistributor.UpdateProcessing(update);
                return;
            }
            else if(update.CallbackQuery != null)
            {
                await updateDistributor.UpdateProcessing(update);
                return;
            }
            else if(update.PreCheckoutQuery != null)
            {
                await MessageController.SendAnswerPreCheckoutQuery(update);
                return;
            }

            await updateDistributor.UpdateProcessing(update);//Ловит обновление
            return;
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
