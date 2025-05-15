using AutoMapper;
using DataContext.Entities;
using DataContext.Dtos;
namespace Services.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User Mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();

            CreateMap<UserRegisterDto, User>();

            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Username));

            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            //Role Mappings
            CreateMap<Role, RoleDto>().ReverseMap();

            //WalletMappings
            CreateMap<Wallet, WalletDto>().ReverseMap();

            CreateMap<Wallet, WalletDetailDto>()
                .ForMember(dest => dest.Cryptos, opt => opt.MapFrom(src => src.WalletCryptos));
            CreateMap<Wallet, PortfolioDto>()

               .ForMember(dest => dest.Cryptos, opt => opt.MapFrom(src => src.WalletCryptos))

               .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.WalletCryptos.Sum(wc=>wc.Amount*wc.Crypto.Price)));
            CreateMap<WalletCrypto, PortfolioDetailDto>()
                .ForMember(dest => dest.CryptoName, opt => opt.MapFrom(src => src.Crypto.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Crypto.Symbol))
                .ForMember(dest => dest.BuyPrice, opt => opt.MapFrom(src => src.BuyPrice))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Amount * src.Crypto.Price));

            //WalletCrypto Mappings
            CreateMap<WalletCrypto, WalletCryptoDetailDto>()
                .ForMember(dest => dest.CryptoName, opt => opt.MapFrom(src => src.Crypto.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Crypto.Symbol))
                .ForMember(dest => dest.BuyPrice, opt => opt.MapFrom(src => src.BuyPrice));

            //Crypto Mappings
            CreateMap<Crypto, CryptoDto>().ReverseMap();

            //CryptoPriceLog Mappings
            CreateMap<CryptoPriceLog, CryptoPriceLogDto>()
                .ForMember(dest => dest.CryptoName, opt => opt.MapFrom(src => src.Crypto.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Crypto.Symbol));

            //TransactionLog Mapping
            CreateMap<TransactionLog, TransactionDto>()
                .ForMember(dest => dest.CryptoName, opt => opt.MapFrom(src => src.Crypto.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Crypto.Symbol))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")));

            CreateMap<TransactionLog, DetailedTransactionDto>()
                .ForMember(dest => dest.CryptoName, opt => opt.MapFrom(src => src.Crypto.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Crypto.Symbol))
                .ForMember(dest => dest.CurrentUnitPrice, opt => opt.MapFrom(src => src.Crypto.Price))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")));

            //Alert Mapping
            CreateMap<AlertCreateDto, Alert>();

            CreateMap<Alert, AlertDto>()
                .ForMember(dest => dest.CryptoName, opt => opt.MapFrom(src => src.Crypto.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Crypto.Symbol))
                .ReverseMap();
            CreateMap<AlertLog, AlertLogDto>()
                .ForMember(dest => dest.CryptoName, opt => opt.MapFrom(src => src.Crypto.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Crypto.Symbol))
                .ReverseMap();

            //MarketListing Mapping
            CreateMap<MarketListingCreateDto, MarketListing>();
            CreateMap<MarketListing, MarketListingDto>()
                .ForMember(dest => dest.CryptoName, opt => opt.MapFrom(src => src.Crypto.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Crypto.Symbol))
                .ReverseMap();

            //Fee Mapping
            CreateMap<TransactionLog, TransactionFeeDto>();

        }
    }
}
