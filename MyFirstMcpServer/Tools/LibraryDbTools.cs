using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using MyFirstMcpServer.Data;
using ModelContextProtocol.Server;

namespace MyFirstMcpServer.Tools;

[McpServerToolType]
public class LibraryDbTools(LibraryDbContext db)
{
    [McpServerTool, Description("Searches the library catalog by title or author and returns up to 10 matching books.")]
    public async Task<string> SearchBooks(string searchTerm)
    {
        var matches = await db.Books
            .Where(b => EF.Functions.Like(b.Title, $"%{searchTerm}%") ||
                        EF.Functions.Like(b.Author, $"%{searchTerm}%"))
            .OrderBy(b => b.Title)
            .Take(10)
            .ToListAsync();

        if (matches.Count == 0)
            return $"No books found matching '{searchTerm}'.";

        var lines = matches.Select(b =>
            $"- {b.Title} by {b.Author} ({b.PublishedYear}, {b.Genre}) - {b.CopiesAvailable} copies available");

        return $"Found {matches.Count} book(s) matching '{searchTerm}':\n" + string.Join("\n", lines);
    }

    [McpServerTool, Description("Lists up to 10 books in a specific genre.")]
    public async Task<string> GetBooksByGenre(string genre)
    {
        var matches = await db.Books
            .Where(b => b.Genre == genre)
            .OrderBy(b => b.Title)
            .Take(10)
            .ToListAsync();

        if (matches.Count == 0)
            return $"No books found in genre '{genre}'.";

        var lines = matches.Select(b =>
            $"- {b.Title} by {b.Author} ({b.PublishedYear}) - {b.CopiesAvailable} copies available");

        return $"Found {matches.Count} book(s) in '{genre}':\n" + string.Join("\n", lines);
    }

    [McpServerTool, Description("Returns summary statistics for the entire library catalog.")]
    public async Task<string> GetLibraryStats()
    {
        var totalBooks = await db.Books.CountAsync();
        var totalCopies = await db.Books.SumAsync(b => b.CopiesAvailable);
        var byGenre = await db.Books
            .GroupBy(b => b.Genre)
            .Select(g => new { Genre = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToListAsync();

        var breakdown = string.Join("\n", byGenre.Select(g => $"  - {g.Genre}: {g.Count} books"));

        return $"Library catalog: {totalBooks} books, {totalCopies} copies available.\nBy genre:\n{breakdown}";
    }
}
