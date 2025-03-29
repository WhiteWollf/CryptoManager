using Services;
using Services.Services;

namespace CryptoManager
{
    public static class ServiceCollectionExtension
    {
        public static void AddLocalServices(this IServiceCollection services)
        {
            services.AddScoped<ICryptoService, CryptoService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<ICryptoPriceService, CryptoPriceService>();
            services.AddScoped<IProfitLossService, ProfitLossService>();
            services.AddScoped<ITradeService, TradeService>();
            services.AddScoped<ITransactionService, TransactionService>();
        }
    }
}
