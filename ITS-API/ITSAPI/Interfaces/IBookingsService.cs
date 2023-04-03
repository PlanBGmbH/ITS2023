using ITSAPI.Models;

namespace ITSAPI.Interfaces
{
    public interface IBookingsService
    {
        Task<IEnumerable<Booking>> GetBookingsAsync(string query);

        Task AddBookingAsync(Booking booking);

        Task DeleteBookingAsync(string id);

        Task UpdateBookingAsync(Booking booking);
    }
}
