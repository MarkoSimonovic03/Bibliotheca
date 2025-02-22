using System.ComponentModel.DataAnnotations;

namespace Bibliotheca.Models
{
    public class Book : IValidatableObject
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [StringLength(3000, ErrorMessage = "Summary cannot exceed 3000 characters.")]
        public string Summary { get; set; }
        [Required]
        public string Author { get; set; }
        public string Publisher { get; set; }
        [Display(Name = "Year of publication")]
        [Range(1, int.MaxValue)]
        public int YearOfPublication { get; set; }
        [Display(Name = "Number of pages")]
        [Range(1, int.MaxValue)]
        public int NumberOfPages { get; set; }
        [Required]
        [Display(Name = "Available quantity")]
        [Range(1, int.MaxValue)]
        public int AvailableQuantity { get; set; }
        [Required]
        public bool IsDeleted {  get; set; }
        public string? ImageUrl {  get; set; }
        public IEnumerable<Review>? Reviews { get; set; }
        public ICollection<Category>? Categories { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (YearOfPublication > DateTime.Now.Year)
            {
                yield return new ValidationResult("Year of publication cannot be in the future.", new[] { nameof(YearOfPublication) });
            }
        }
    }
}
