namespace UrlShort.Models;

public class ShortUrl
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Short { get; set; } = string.Empty;
}