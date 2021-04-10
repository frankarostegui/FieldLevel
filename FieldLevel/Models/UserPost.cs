using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FieldLevel.Models
{
    public class UserPost
    {
        // The user post model - updated
        public string UserId { get; set; }
        public string PostId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
