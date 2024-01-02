/// <summary>
/// Books
/// </summary>
public class Books
{
    /// <summary>
    /// Unique identifier for each book.
    /// </summary>
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
    public DateTime PublishedDate { get; set; }
}