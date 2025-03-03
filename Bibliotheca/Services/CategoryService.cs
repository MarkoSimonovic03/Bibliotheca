using Bibliotheca.Interfaces;
using Bibliotheca.Models;
using Microsoft.EntityFrameworkCore;

namespace Bibliotheca.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly BibliothecaContext _context;
		public CategoryService(BibliothecaContext context)
		{
			_context = context;
		}
		public ICollection<Category> GetAllCategories()
		{
			return _context.Categories.ToList();
		}

		public ICollection<Category> GetCategoriesByIds(IEnumerable<int> ids)
		{
            return _context.Categories.Where(c => ids.Contains(c.Id)).ToList();
        }

		public ICollection<Category> GetCategoriesByNames(List<string> categories)
		{
			return _context.Categories.Where(c => categories.Contains(c.Name)).ToList();
		}

		public Category GetCategoryById(int id)
		{
			var category = _context.Categories.Find(id);
			if (category == null )
			{
				return null;
			}
			return category;
		}

		public void AddCategory(Category category)
		{
			_context.Categories.Add(category);
		}

		public void UpdateCategory(Category category)
		{
			_context.Categories.Update(category);
		}

		public void DeleteCategory(int id)
		{
			var category = _context.Categories.Find(id);
			if (category != null)
			{
				_context.Categories.Remove(category);
			}
		}
	}
}
