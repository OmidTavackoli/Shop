using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain.Models.BaseEntities
{
    public class BaseEntity
    {
        public long Id { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
