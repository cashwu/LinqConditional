using System.ComponentModel.DataAnnotations;

namespace testLinqConditional
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}