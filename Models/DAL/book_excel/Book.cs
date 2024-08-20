using System;
using System.Collections.Generic;

namespace WebDataTransfer.Models.DAL.book_excel;

public partial class Book
{
    public int Id { get; set; }

    public string? BookName { get; set; }

    public string? BookAuthor { get; set; }

    public DateTime BookDataRealese { get; set; }
}
