using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
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
            if (file == null || !file.FileName.EndsWith(".xlsx"))
                throw new ArgumentException("Invaild Format FILE");

            var books = new List<Book>(); // коллекция для dal
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
                        var dto = new Models.DTO.Book
                        {
                            Id = int.Parse(worksheet.Cell(row, 1).GetValue<string>()),
                            Name = worksheet.Cell(row, 2).GetString(),
                            Author = worksheet.Cell(row, 3).GetString(),
                            DataRealese = DateTime.Parse(worksheet.Cell(row, 4).GetString())
                        };

                        var dal = new Models.DAL.book_excel.Book
                        {
                            Id = dto.Id,
                            BookName = dto.Name,
                            BookAuthor = dto.Author,
                            BookDataRealese = dto.DataRealese,
                        };

                        books.Add(dal);
                    }
                }
            }
            foreach (var book in books)
            {
                var exBook = await _context.Books.FindAsync(book.Id);
                if (exBook != null)
                {
                    // Обновление существующей записи
                    _context.Entry(exBook).CurrentValues.SetValues(book);
                }
                else
                {
                    // Добавление новой записи
                    await _context.Books.AddAsync(book);
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
