using System.ComponentModel.DataAnnotations;

public class BookItemAdding
{
    /// <summary>
    /// Title of the book.
    /// </summary>
    [Required]
    public string? Title { get; set; }
    /// <summary>
    /// Author of the book.
    /// </summary>
    [Required]
    public string? Author { get; set; }
    /// <summary>
    /// International Standard Book Number.
    /// </summary>
    [Required]
    public string? ISBN { get; set; }
    /// <summary>
    /// Date of publication.
    /// </summary>
    [Required]
    public DateTime PublishedDate { get; set; }
}