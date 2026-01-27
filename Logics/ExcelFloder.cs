using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Linq;
using WebDataTransfer.Models.DAL.book_excel;

namespace WebDataTransfer.Logics
{
    public class ExcelFloder
    {
        private readonly bookContext _context;
        public ExcelFloder(bookContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> ExcelData(IFormFile file)
        {

            var books = new List<Book>();

            if (file == null || !file.FileName.EndsWith(".xlsx"))
                throw new ArgumentException("Invaild Format File");

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheets.First();

                    var rowCount = worksheet.LastRowUsed().RowNumber();

                    var colCount = worksheet.LastColumnUsed().ColumnNumber();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        for (int col = 1; col <= colCount; col++)
                        {
                            var cellValue = worksheet.Cell(row, col).GetValue<string>();
                        }

                        var dto = new Models.DTO.Book
                        {
                            Id = int.Parse(worksheet.Cell(row, 1).GetValue<string>()),
                            Name = worksheet.Cell(row, 2)
                            .GetString(),
                            Author = worksheet.Cell(row, 3)
                            .GetString(),
                            DataRelease = DateTime.Parse(worksheet.Cell(row, 4)
                            .GetString())
                        };

                        var dal = new Models.DAL.book_excel.Book
                        {
                            Id = dto.Id,
                            BookName = dto.Name,
                            BookAuthor = dto.Author,
                            BookDataRelease = dto.DataRelease,
                        };

                        books.Add(dal);
                    }
                }
            }
            // Загрузить все существующие ID за ОДИН запрос
            var existingBooks = await _context.Books
                .Where(b => books.Select(x => x.Id)
                .Contains(b.Id))
                .ToDictionaryAsync(b => b.Id);

            foreach (var book in books)
            {
                if (existingBooks.TryGetValue(book.Id, out var exBook))
                {
                    _context.Entry(exBook).CurrentValues.SetValues(book);
                }
                else
                {
                    _context.Books.Add(book);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Логирование и обработка ошибки
                Console.WriteLine($"Ошибка обновления базы данных: {ex.Message}");
                throw;
            }

            return books;
        }

    }
}
