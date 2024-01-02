using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{
    private readonly ILogger<BooksController> _logger;

    public BooksController(ILogger<BooksController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get List of Books
    /// Returns a list of books with details and their current prices.
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "")]
    public IEnumerable<BookItem> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new BookItem
        {
        })
        .ToArray();
    }

    /// <summary>
    /// Add a New Book
    /// Adds a new book to the Books table.
    /// </summary>
    /// <returns></returns>
    [HttpPost(Name = "")]
    public void Add(BookItemAdding bookItemAdding)
    { 
    }

    /// <summary>
    /// Update the Price of a Book
    /// Updates the price of a specific book. 
    /// This should add a new record to BookPrices and update the EndDate of the previous record if necessary.
    /// </summary>
    /// <returns></returns>
    [HttpPut(Name = "{bookId}/price")]
    public void UpdatePrice(BookItemPriceUpdating bookItemPriceUpdating)
    { 
    }

        /// <summary>
    /// Get Historical Prices of a Specific Book
    /// Returns all historical prices of a specific book.
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "{bookId}/prices")]
    public IEnumerable<BookItem> GetHistoricalPricesOfABook()
    {
        return Enumerable.Range(1, 5).Select(index => new BookItem
        {
        })
        .ToArray();
    }

    /// <summary>
    /// Calculate Revenue for a Book
    /// Receives a list of sales records (each record containing a sale date and quantity) 
    /// and calculates the total revenue generated for the specified
    /// </summary>
    /// <returns></returns>
    [HttpPost(Name = "{bookId}/revenue")]
    public void CalculateRevenueForABook(BookItemAdding bookItemAdding)
    { 
    }

}
