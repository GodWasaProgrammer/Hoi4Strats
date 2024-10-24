using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DBModels
{
    public class ForumThread
    {
        // id of thread
        public int Id { get; set; }
        public string Title { get; set; }
        // id of creating User
        public int UserID { get; set; }
        public int ForumCategoryId { get; set; }
        public DateTime DateTime { get; set; }
    }
}
