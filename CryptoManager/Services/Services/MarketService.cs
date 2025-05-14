using AutoMapper;
using BCrypt.Net;
using DataContext.Context;
using DataContext.Dtos;
using DataContext.Entities;
using DataContext.Enums;
using DataContext.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface IMarketService
    {
        Task<IList<MarketListingDto>> GetMarketListingsAsync();
        Task<MarketListingDto> CreateMarketListingAsync(MarketListingCreateDto marketListingCreateDto);
        Task DeleteMarketListingAsync(int listingId);
        Task AcceptMarketListingAsync(int listingId, AcceptMarketListingDto acceptDto);
    }
    public class MarketService : IMarketService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;

        public MarketService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AcceptMarketListingAsync(int listingId, AcceptMarketListingDto acceptDto)
        {
            var listing = await _context.MarketListings.FirstOrDefaultAsync(l => l.Id == listingId);
            if (listing == null)
            {
                throw new Exception("Listing not found");
            }
            if (listing.UserId == acceptDto.UserId) {
                throw new Exception("You cannot buy or sell your own listings");    
            }
            if (listing.Status == EMarketStatus.Inactive)
            {
                throw new Exception("This listing is Inactive");
            }
            var buyer = new User();
            var seller = new User();
            if (listing.Type == EMarketType.Buy)
            {
                buyer = await _context.Users.FirstOrDefaultAsync(u => u.Id == listing.UserId);
                seller = await _context.Users.FirstOrDefaultAsync(u => u.Id == acceptDto.UserId);
            }
            else
            {
                seller = await _context.Users.FirstOrDefaultAsync(u => u.Id == listing.UserId);
                buyer = await _context.Users.FirstOrDefaultAsync(u => u.Id == acceptDto.UserId);
            }
            if (buyer == null || seller == null)
            {
                throw new Exception("User not found");
            }
            var sellerWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == seller.Id);
            var buyerWallett = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == buyer.Id);

            if (sellerWallet == default || buyerWallett == default)
            {
                throw new Exception("Wallet not found");
            }
            WalletCrypto sellerwallettcrypto = await _context.WalletCrypto.FirstOrDefaultAsync(wc => wc.WalletId == sellerWallet.Id && wc.CryptoId == listing.CryptoId);
            if (sellerwallettcrypto == default)
            {
                throw new Exception("Crypto not found in sellers wallet");
            }
            //Valszeg felesleges, mivel lockolva van az a mennyiség, dehát elérhető lesz
            /*if (sellerwallettcrypto.Amount - sellerwallettcrypto.LockedAmount < listing.Amount)
            {
                throw new Exception("Not enough crypto available");
            }*/
            if (buyerWallett.Balance < listing.Price)
            {
                throw new Exception("Not enough balance on they buyers wallett");
            }
            WalletCrypto buyerwallettCrypto = await _context.WalletCrypto.FirstOrDefaultAsync(wc => wc.WalletId == buyerWallett.Id && wc.CryptoId == listing.CryptoId);
            if (buyerwallettCrypto == default)
            {
                buyerwallettCrypto = new WalletCrypto { WalletId = buyerWallett.Id, CryptoId = listing.CryptoId, Amount = listing.Amount, BuyPrice = listing.Price };
                await _context.WalletCrypto.AddAsync(buyerwallettCrypto);
            }
            else
            {
                buyerwallettCrypto.Amount += listing.Amount;
                buyerwallettCrypto.BuyPrice = listing.Price / listing.Amount;
            }
            buyerWallett.Balance -= listing.Price;
            sellerwallettcrypto.LockedAmount -= listing.Amount;
            sellerWallet.Balance += listing.Price;
            sellerwallettcrypto.Amount -= listing.Amount;
            if (sellerwallettcrypto.Amount == 0)
            {
                _context.WalletCrypto.Remove(sellerwallettcrypto);
            }
            var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Id == listing.CryptoId);
            if (crypto == null)
            {
                throw new Exception("Crypto not found");
            }
            var buytransaction = new TransactionLog
            {
                UserId = buyer.Id,
                User = buyer,
                CryptoId = listing.CryptoId,
                Crypto = crypto,
                Amount = listing.Amount,
                PricePerUnit = listing.Price / listing.Amount,
                Type = TransactionType.Buy,
                Description = $"{buyer.Name} bought {listing.Amount} {crypto.Name} for {listing.Price} from the Market from {seller.Name}$",
                Timestamp = DateTime.Now
            };

            _context.TransactionLogs.Add(buytransaction);

            var selltransaction = new TransactionLog
            {
                UserId = seller.Id,
                User = seller,
                CryptoId = listing.CryptoId,
                Crypto = crypto,
                Amount = listing.Amount,
                PricePerUnit = listing.Price / listing.Amount,
                Type = TransactionType.Sell,
                Description = $"{seller.Name} sold {listing.Amount} {crypto.Name} for {listing.Price} on the Market to {buyer.Name}$",
                Timestamp = DateTime.Now
            };

            _context.TransactionLogs.Add(buytransaction);

            listing.Status = EMarketStatus.Inactive;

            await _context.SaveChangesAsync();

        }

        public async Task<MarketListingDto> CreateMarketListingAsync(MarketListingCreateDto marketListingCreateDto)
        {
            if ((int)marketListingCreateDto.Type < 0 || (int)marketListingCreateDto.Type > 1)
            {
                throw new Exception("Incorrect Type");
            }
            var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Id == marketListingCreateDto.CryptoId);
            if (crypto == default)
            {
                throw new Exception("Crypto not found");
            }
            if (marketListingCreateDto.Amount < 0)
            {
                throw new Exception("Insufficent amount");
            }
            if (marketListingCreateDto.Price <= 0)
            {
                throw new Exception("Insufficent price");
            }
            var listing = new MarketListing();
            if (marketListingCreateDto.Type == EMarketType.Buy)
            {
                var userwallett = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == marketListingCreateDto.UserId);
                if (userwallett.Balance < marketListingCreateDto.Price)
                {
                    throw new Exception("Not enough balance");
                }
                listing = _mapper.Map<MarketListing>(marketListingCreateDto);
                await _context.MarketListings.AddAsync(listing);
            }
            else if (marketListingCreateDto.Type == EMarketType.Sell)
            {
                var userwallett = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == marketListingCreateDto.UserId);
                var walletcrypto = await _context.WalletCrypto.FirstOrDefaultAsync(wc => wc.WalletId == userwallett.Id && wc.CryptoId == marketListingCreateDto.CryptoId);
                if (walletcrypto == default)
                {
                    throw new Exception("Crypto not found in wallet");
                }
                if (walletcrypto.Amount < marketListingCreateDto.Amount)
                {
                    throw new Exception("Not enough crypto available");
                }
                walletcrypto.LockedAmount += marketListingCreateDto.Amount;
                listing = _mapper.Map<MarketListing>(marketListingCreateDto);
                await _context.MarketListings.AddAsync(listing);
            }
            await _context.SaveChangesAsync();
            return _mapper.Map<MarketListingDto>(listing);
        }

        public async Task DeleteMarketListingAsync(int listingId)
        {
            var listing = await _context.MarketListings.FirstOrDefaultAsync(c => c.Id == listingId);
            if (listing == default)
            {
                throw new Exception("MarketListing not found!");
            }
            listing.Status = EMarketStatus.Inactive;
            if (listing.Type == EMarketType.Sell)
            {
                var userWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == listing.UserId);
                var userCrypto = await _context.WalletCrypto.FirstOrDefaultAsync(wc => wc.WalletId == userWallet.Id && wc.CryptoId == listing.Id);
                userCrypto.LockedAmount -= listing.Amount;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IList<MarketListingDto>> GetMarketListingsAsync()
        {
            var listings = await _context.MarketListings
                .Include(ml => ml.Crypto)
                .Where(l => l.Status == EMarketStatus.Active)
                .ToListAsync();
            return _mapper.Map<IList<MarketListingDto>>(listings);
        }
    }
}
