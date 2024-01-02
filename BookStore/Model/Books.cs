using System.ComponentModel.DataAnnotations;

/// <summary>
/// Books
/// </summary>
public class Books
{
    /// <summary>
    /// Unique identifier for each book.
    /// </summary>
    [Key]
    public int BookID { get; set; }
    /// <summary>
    /// Title of the book.
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// Author of the book.
    /// </summary>
    public string? Author { get; set; }
    /// <summary>
    /// International Standard Book Number.
    /// </summary>
    public string? ISBN { get; set; }
    /// <summary>
    /// Date of publication.
    /// </summary>
    public DateTime PublishDate { get; set; }
    /// <summary>
    /// Book Prices
    /// </summary>
    public ICollection<BooksPrices>? BookPrices { get; set; }
}