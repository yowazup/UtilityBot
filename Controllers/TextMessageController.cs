using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using UtilityBot.Utilities;
using UtilityBot.Services;


namespace UtilityBot.Controllers
{
    public class TextMessageController
    {
        private readonly IStorage _memoryStorage;
        private readonly ITelegramBotClient _telegramClient;

        public TextMessageController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            Console.WriteLine($"Контроллер {GetType().Name} получил сообщение");

            switch (message.Text)
            {
                case "/start":

                    // Объект, представляющий кноки
                    var buttons = new List<InlineKeyboardButton[]>();
                    buttons.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData($"\U0001F4C4 Символы" , $"chars"),
                        InlineKeyboardButton.WithCallbackData($"\U0001F4B0 Числа" , $"numbers"),
                    });

                    // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"<b>  Этот бот умеет складывать символы или числа.</b> {Environment.NewLine}" +
                        $"{Environment.NewLine}Выберете, что хочется сложить:{Environment.NewLine}", cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));
                    break;

                default:

                    //Получим выбор метода расчета из сессии пользователя
                    string selectionCode = _memoryStorage.GetSession(message.Chat.Id).selectionCode;

                    //Используем разные алгоритмы для суммы символов и суммы чисел
                    string selectionResult = selectionCode switch
                    {
                        "chars" => $"Длина сообщения: {message.Text.Length} символов",
                        "numbers" => $"Сумма введенных чисел: {SumCalculator.Calculation(message.Text, out _)}",
                        _ => String.Empty
                    };

                    // Отправляем ответ
                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"{selectionResult}.{Environment.NewLine}");
                    break;
            }
        }
    }
}
