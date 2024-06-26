﻿namespace Domain.Models;

public class Tag
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public ICollection<BookTag>? BookTags { get; set; }
}