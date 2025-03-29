﻿using AutoMapper;
using DataContext.Entities;
using DataContext.Dtos;
namespace Services.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User Mappings
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserRegisterDto, User>();
            CreateMap<UserUpdateDto, User>();

            CreateMap<WalletCrypto, WalletCryptoDto>().ReverseMap();
            CreateMap<WalletCrypto, WalletCryptoDto>();
            CreateMap<WalletCryptoDto, WalletCrypto>();

            CreateMap<Wallet, WalletDto>().ReverseMap();
            CreateMap<Wallet, WalletDto>();
            CreateMap<WalletDto, Wallet>();

            CreateMap<Crypto, CryptoDto>().ReverseMap();
            CreateMap<Crypto, CryptoDto>();
            CreateMap<CryptoDto, Crypto>();

            CreateMap<Wallet, WalletDetailDto>()
            .ForMember(dest => dest.Cryptos, opt => opt.MapFrom(src => src.WalletCryptos));
            CreateMap<WalletCrypto, WalletCryptoDetailDto>()
                .ForMember(dest => dest.CryptoName, opt => opt.MapFrom(src => src.Crypto.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Crypto.Symbol))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Crypto.Price));

            CreateMap<Wallet, PortfolioDto>()
           .ForMember(dest => dest.Cryptos, opt => opt.MapFrom(src => src.WalletCryptos))
           .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.WalletCryptos.Sum(wc=>wc.Amount*wc.Crypto.Price)));
            CreateMap<WalletCrypto, PortfolioDetailDto>()
                .ForMember(dest => dest.CryptoName, opt => opt.MapFrom(src => src.Crypto.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Crypto.Symbol))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Crypto.Price))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Amount * src.Crypto.Price));

            CreateMap<CryptoPriceLog, CryptoPriceLogDto>()
                .ForMember(dest => dest.CryptoName, opt => opt.MapFrom(src => src.Crypto.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Crypto.Symbol));

            CreateMap<TransactionLog, TransactionDto>()
                .ForMember(dest => dest.CryptoName, opt => opt.MapFrom(src => src.Crypto.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Crypto.Symbol));

            CreateMap<TransactionLog, DetailedTransactionDto>()
                .ForMember(dest => dest.CryptoName, opt => opt.MapFrom(src => src.Crypto.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Crypto.Symbol))
                .ForMember(dest => dest.CurrentUnitPrice, opt => opt.MapFrom(src => src.Crypto.Price));


            /*
            // Food Mappings
            CreateMap<Food, FoodDto>().ReverseMap();
            CreateMap<FoodCreateDto, Food>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.FoodCategoryId));
            CreateMap<FoodUpdateDto, Food>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.FoodCategoryId));

            CreateMap<FoodCategory, FoodCategoryDto>();

            // Order Mappings
            CreateMap<Order, OrderDto>();
            CreateMap<OrderCreateDto, Order>();
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.FoodName, opt => opt.MapFrom(src => src.MenuItem.Name))
                .ForMember(dest => dest.FoodUnitPrice, opt => opt.MapFrom(src => src.MenuItem.Price));
            CreateMap<OrderItemCreateDto, OrderItem>();

            CreateMap<Role, RoleDto>();

            // Restaurant Mappings
            CreateMap<Restaurant, RestaurantDto>();
            */
        }
    }
}
