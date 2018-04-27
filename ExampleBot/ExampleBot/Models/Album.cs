using System;
using System.Collections.Generic;

namespace ExampleBot.Models
{
    public class Album
    {
        public Guid Id { get; set; }
        public string Artist { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class AlbumList : List<Album>
    {

    }
}
