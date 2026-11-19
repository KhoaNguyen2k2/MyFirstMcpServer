using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Bogus;
namespace MyFirstMcpServer.Data
{
    public static class LibrarySeeder
    {
        private static readonly string[] Genres =
     [
         "Fantasy", "Science Fiction", "Mystery", "Romance", "History",
        "Biography", "Thriller", "Horror", "Poetry", "Self-Help"
     ];

        public static async Task SeedAsync(LibraryDbContext db)
        {
            if (await db.Books.AnyAsync())
                return;

            var titleCase = CultureInfo.InvariantCulture.TextInfo;
            var faker = new Faker<Book>()
                .UseSeed(2026)
                .RuleFor(b => b.Title, f => titleCase.ToTitleCase(string.Join(' ', f.Random.Words(f.Random.Int(1, 3)))))
                .RuleFor(b => b.Author, f => f.Name.FullName())
                .RuleFor(b => b.Genre, f => f.PickRandom(Genres))
                .RuleFor(b => b.PublishedYear, f => f.Date.Past(80).Year)
                .RuleFor(b => b.CopiesAvailable, f => f.Random.Int(1, 12));

            db.Books.AddRange(faker.Generate(5_000));
            await db.SaveChangesAsync();
        }

    }
}
