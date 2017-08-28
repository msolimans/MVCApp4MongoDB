using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M101DotNet.WebApp.Models
{
    public class Comment
    {
        // XXX WORK HERE
        // Add in the appropriate properties.
        // The homework instructions have the
        // necessary schema.
        [BsonId]
        public ObjectId PostId { get; set; }

        public string Content { get; set; }
        public Post Post { get; set; }

        public string Author { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}