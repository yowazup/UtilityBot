using Microsoft.Extensions.Hosting;
using UtilityBot.Controllers;
using Telegram.Bot;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using UtilityBot.Services;
using UtilityBot.Configuration;

namespace UtilityBot
{
    public class Program
    {
        public static async Task Main()
        {
            Console.OutputEncoding = Encoding.Unicode;

            //Объект, отвечающий за постоянный жизненный цикл приложения
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) => ConfigureServices(services)) // Задаем конфигурацию
                .UseConsoleLifetime() // Позволяет поддерживать приложение активным в консоли
                .Build(); // Собираем

            //Запускаем сервис
            Console.WriteLine("Сервис запущен.");
            await host.RunAsync();
            Console.WriteLine("Сервис остановлен.");
        }

        static void ConfigureServices(IServiceCollection services)
        {
            //Добавляем инициализацию конфигурации
            AppSettings appSettings = BuildAppSettings();
            services.AddSingleton(BuildAppSettings());

            //Добавляем хранилище сессий в контейнер зависимостей
            services.AddSingleton<IStorage, MemoryStorage>();

            //Подключаем контроллеры сообщений и кнопок
            services.AddTransient<DefaultMessageController>();
            services.AddTransient<TextMessageController>();
            services.AddTransient<InlineKeyboardController>();

            //Регистрируем объект TelegramBotClient c токеном подключения
            services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(appSettings.BotToken));

            //Регистрируем постоянно активный сервис бота
            services.AddHostedService<Bot>();
        }
        static AppSettings BuildAppSettings()
        {
            return new AppSettings()
            {
                BotToken = "5858477123:AAFTszzFho_6iOaBnBQOwOGwWn132ZN0pmE",
            };
        }
    }
}