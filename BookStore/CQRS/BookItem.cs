public class BookItem
{
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
    /// Price
    /// </summary>
    public decimal Price { get; set; }
}