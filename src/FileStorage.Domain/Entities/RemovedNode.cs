using System;

namespace FileStorage.Domain.Entities
{
    public class RemovedNode
    {
        public int Id { get; set; }
        public Guid? NodeId { get; set; }
        public DateTime RemovedOn { get; set; }
        public Node Node { get; set; }
        public DateTime DateOfRemoval { get; set; }
        public string ExecutorUserId { get; set; }
        public ApplicationUser ExecutorUser { get; set; }
    }
}
