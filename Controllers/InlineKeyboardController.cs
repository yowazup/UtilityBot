using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UtilityBot.Services;

namespace UtilityBot.Controllers
{
    internal class InlineKeyboardController
    {
        private readonly IStorage _memoryStorage;
        private readonly ITelegramBotClient _telegramClient;

        public InlineKeyboardController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(CallbackQuery? callbackQuery, CancellationToken ct)
        {
            if (callbackQuery?.Data == null)
                return;

            //Обновление пользовательской сессии новыми данными
            _memoryStorage.GetSession(callbackQuery.From.Id).selectionCode = callbackQuery.Data;

            Console.WriteLine($"Контроллер {GetType().Name} обнаружил нажатие на кнопку {callbackQuery.Data}.");

            // Создаем информационное сообщение
            string selectionText = callbackQuery.Data switch
            {
                "chars" => " 📄 сложить символы. Далее надо ввести любой текст и отправить мне сообщением. В ответ я скажу вам сколько в нем символов (букв, пробелов, цифр и т.д.)",
                "numbers" => " 💰 сложить числа. Далее надо ввести любое количество чисел через пробел и отправить мне сообщением. В ответ я скажу вам сумму",
                _ => String.Empty
            };

            // Отправляем в ответ уведомление о выборе
            await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id,
                $"<b>Вы выбрали  - {selectionText}.{Environment.NewLine}</b>" +
                $"{Environment.NewLine}Можно поменять в Главном меню.", cancellationToken: ct, parseMode: ParseMode.Html);
        }
    }
}
