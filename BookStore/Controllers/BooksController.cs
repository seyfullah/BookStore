using BooksApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{
    private readonly ILogger<BooksController> _logger;
    private readonly BooksContext _context;

    public BooksController(ILogger<BooksController> logger, BooksContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Get List of Books
    /// Returns a list of books with details and their current prices.
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<BookItem>>> Get()
    {
        var books = await _context.Books.Select(x => ItemToDTO(x)).ToListAsync();
        if (books == null)
        {
            return NotFound();
        }
        return books;
    }

    /// <summary>
    /// Add a New Book
    /// Adds a new book to the Books table.
    /// </summary>
    /// <param name="bookItemAdding"></param>
    /// <returns></returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /books
    ///     {
    ///        "Title": "Software Development",
    ///        "Author": "Mehmet Usta",
    ///        "ISBN": "12983487"
    ///        "PublishedDate": "2023-01-02"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Create(BookItemAdding bookItemAdding)
    {
        var book = new Books
        {
            Title = bookItemAdding.Title,
            Author = bookItemAdding.Author,
            ISBN = bookItemAdding.ISBN,
            PublishedDate = bookItemAdding.PublishedDate,
        }
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Add), null);
    }

    /// <summary>
    /// Update the Price of a Book
    /// Updates the price of a specific book. 
    /// This should add a new record to BookPrices and update the EndDate of the previous record if necessary.
    /// </summary>
    /// <returns></returns>
    [HttpPut("{bookId}/price")]
    public async Task<IActionResult> UpdatePrice(int bookId, BookItemPriceUpdating bookItemPriceUpdating)
    {
        if (bookId != bookItemPriceUpdating.BookId)
        {
            return BadRequest();
        }

        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
        {
            return NotFound();
        }

        var bookPrice = await _context.BookPrices
            .Where(entity => entity.BookID == bookId && entity.EndDate == null)
            .FirstOrDefaultAsync();
        if (bookPrice == null)
        {
            return NotFound();
        }

        bookPrice.EndDate = DateTime.Now;

        var newPrice = new BooksPrices
        {
            BookID = bookId,
            Price = bookItemPriceUpdating.Price,
            EffectiveDate = DateTime.Now,
        };
        _context.BookPrices.Add(newPrice);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Get Historical Prices of a Specific Book
    /// Returns all historical prices of a specific book.
    /// </summary>
    /// <param name="bookId">
    /// <returns></returns>
    [HttpGet("{bookId}/prices")]
    public async Task<ActionResult<IEnumerable<HistoricalPrices>>> GetHistoricalPrices(int bookId)
    {
        var historicalPrices = await _context.BookPrices.Select(x => BookPricesToDTO(x)).ToListAsync();
        if (historicalPrices == null)
        {
            return NotFound();
        }
        return historicalPrices;
    }

    /// <summary>
    /// Calculate Revenue for a Book
    /// Receives a list of sales records (each record containing a sale date and quantity) 
    /// and calculates the total revenue generated for the specified
    /// </summary>
    /// <returns></returns>
    [HttpPost(Name = "{bookId}/revenue")]
    public async Task<ActionResult<decimal>> CalculateRevenueForABook(int bookId, Dictionary<DateTime, int> calculateRevenue)
    {
        var historicalPrices = await _context.BookPrices.ToListAsync();
        if (historicalPrices == null)
        {
            return NotFound();
        }
        var totalRevenue = 0m;
        foreach (var item in calculateRevenue)
        {
            var price = historicalPrices.FirstOrDefault(p => item.Key <= p.EffectiveDate);
            if (price == null)
            {
                continue;
            }
            totalRevenue += price.Price * item.Value;
        }
        return Ok(totalRevenue);
    }

    private bool BookPricesExists(int priceId)
    {
        return _context.BookPrices.Any(e => e.PriceID == priceId);
    }

    private static BookItem ItemToDTO(Books book) =>
       new BookItem
       {
           Title = book.Title,
           Author = book.Author,
           ISBN = book.ISBN,
           //    Price = book.Price,
           PublishedDate = book.PublishedDate
       };

    private static HistoricalPrices BookPricesToDTO(BooksPrices booksPrice) =>
        new HistoricalPrices
        {
            Price = booksPrice.Price,
            EffectiveDate = booksPrice.EffectiveDate,
            EndDate = booksPrice.EndDate
        };
}

