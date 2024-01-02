using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// BookPrices
/// </summary>
public class BooksPrices
{
    /// <summary>
    /// Unique identifier for each price entry.
    /// </summary>
    [Key]
    public int PriceID { get; set; }
    /// <summary>
    /// References BookID in the Books table.
    /// </summary>
    public int BookID { get; set; }
    /// <summary>
    /// The price of the book.
    /// </summary>
    public decimal Price { get; set; }
    /// <summary>
    /// The date from which this price is effective.
    /// </summary>
    public DateTime EffectiveDate { get; set; }
    /// <summary>
    /// The date when this price is no longer effective. Can be NULL if the price is still current.
    /// </summary>
    public DateTime? EndDate { get; set; }
    /// <summary>
    /// Books FK
    /// </summary>
    [ForeignKey("BookID")]
    public virtual Books? Books { get; set; }
}