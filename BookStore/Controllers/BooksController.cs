using BooksApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
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
    ///        "PublishDate": "2023-01-02"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns as successful</response>
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
            PublishDate = bookItemAdding.PublishDate,
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Create), null);
    }

    /// <summary>
    /// Update the Price of a Book
    /// Updates the price of a specific book. 
    /// This should add a new record to BookPrices and update the EndDate of the previous record if necessary.
    /// </summary>
    /// <param name="bookId"/>
    /// <param name="bookItemPriceUpdating"/>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /books
    ///     {
    ///         "BookId": 1,
    ///         "Price" : 33
    ///     }
    ///
    /// </remarks>
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
        var bookWithPrices = await _context.Books
                            .Where(b => b.BookID == bookId)
                            .Include(b => b.BookPrices)
                            .FirstOrDefaultAsync();
        if (bookWithPrices == null || bookWithPrices.BookPrices == null)
        {
            return NotFound();
        }
        var bookPrice = bookWithPrices.BookPrices.FirstOrDefault(p => p.EndDate == null);
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
    /// <param name="bookId"/>
    /// <returns></returns>
    [HttpGet("{bookId}/prices")]
    public async Task<ActionResult<IEnumerable<HistoricalPrices>>> GetHistoricalPrices(int bookId)
    {
        var historicalPrices = await _context.BookPrices.Where(p => p.BookID == bookId)
            .Select(x => BookPricesToDTO(x)).ToListAsync();
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
    /// <param name="bookId"/>
    /// <param name="calculateRevenueItems"/>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /{bookId}/prices
    ///     [
    ///     {
    ///        "Date": "2023-01-01",
    ///        "Count": 1
    ///     },
    ///     {
    ///        "Date": "2023-02-01",
    ///        "Count": 2
    ///     }
    ///     ]
    ///
    /// </remarks>
    /// <returns></returns>
    [HttpPost("{bookId}/revenue")]
    public async Task<ActionResult<decimal>> CalculateRevenueForABook(int bookId, CalculateRevenueItem[] calculateRevenueItems)
    {
        var historicalPrices = await _context.BookPrices.Where(p => p.BookID == bookId).ToListAsync();
        if (historicalPrices == null)
        {
            return NotFound();
        }
        var totalRevenue = 0m;
        foreach (var item in calculateRevenueItems)
        {
            var price = historicalPrices.FirstOrDefault(p => item.Date <= p.EffectiveDate);
            if (price == null)
            {
                continue;
            }
            totalRevenue += price.Price * item.Count;
        }
        return Ok(totalRevenue);
    }

    private static BookItem ItemToDTO(Books book) =>
       new BookItem
       {
           Title = book.Title,
           Author = book.Author,
           ISBN = book.ISBN,
           //    Price = book.Price,
           PublishDate = book.PublishDate
       };

    private static HistoricalPrices BookPricesToDTO(BooksPrices booksPrice) =>
        new HistoricalPrices
        {
            Price = booksPrice.Price,
            EffectiveDate = booksPrice.EffectiveDate,
            EndDate = booksPrice.EndDate
        };
}
