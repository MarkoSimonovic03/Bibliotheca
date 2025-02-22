using Bibliotheca.Models;

namespace Bibliotheca.Interfaces
{
	public interface ICategoryService
	{
		ICollection<Category> GetAllCategories();
		ICollection<Category> GetCategoriesByIds(IEnumerable<int> ids);
		Category GetCategoryById(int id);
		void AddCategory(Category category);
		void UpdateCategory(Category category);
		void DeleteCategory(int id);
	}
}
