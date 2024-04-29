using CatalogAPI.Models;

namespace CatalogAPI.DTOs.Mappings
{
    public static class CategoryDTOMappingExtensions
    {
        // 'this Category' means this method will be attributed to Category
        public static CategoryDTO? ToCategoryDTO(this Category category)
        {
            if (category == null) return null;

            var categoryDTO = new CategoryDTO()
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
            };

            return categoryDTO;
        }

        public static Category? ToCategory(this CategoryDTO categoryDto) 
        { 
            if (categoryDto == null) return null;

            return new Category
            {
                Id = categoryDto.Id,
                Name = categoryDto.Name,
                ImageUrl = categoryDto.ImageUrl,
            };
        }

        public static IEnumerable<CategoryDTO> ToCategoryDTOList(this IEnumerable<Category> categories) 
        { 
            if (categories is null || !categories.Any()) 
                return new List<CategoryDTO>();

            return categories.Select(category => new CategoryDTO 
            { 
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
            }).ToList();
        }
    }
}
