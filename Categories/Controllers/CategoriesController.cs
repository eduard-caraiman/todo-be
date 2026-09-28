using Microsoft.AspNetCore.Mvc;
using todo_be.Categories.Requests;
using todo_be.Categories.Responses;
using todo_be.Categories.Services;

namespace todo_be.Categories.Controllers;

public class CategoriesController : BaseController
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }


    /// <summary>
    /// Gets All Categories
    /// </summary>
    /// <returns>Return the All Categories in JSON </returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllCategories()
    {
        var categoriesList = await _categoryService.GetAllAsync();

        return Ok(categoriesList.Select(category => GetCategoryResponse.From(category)));
    }


    /// <summary>
    /// Gets Category by ID
    /// </summary>
    /// <returns>Return the Category in JSON </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCategoryById([FromRoute] int id)
    {
        var foundCategory = await _categoryService.GetByIdAsync(id);

        if (foundCategory == null)
        {
            return NotFound();
        }

        var categoryResponse = GetCategoryResponse.From(foundCategory);

        return Ok(categoryResponse);
    }


    /// <summary>
    /// Create Todo Category
    /// </summary>
    /// <returns>Return the new created Todo Category in JSON </returns>
    [HttpPost]
    [ProducesResponseType(typeof(GetCategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var validationResults = await ValidateAsync(request);
        if (!validationResults.IsValid)
        {
            return BadRequest(validationResults.ToModelStateDictionary());
        }


        var newCategory = await _categoryService.CreateAsync(request);

        return Created($"/api/categories/{newCategory.Id}", GetCategoryResponse.From(newCategory));
    }

    /// <summary>
    /// Update Category
    /// </summary>
    /// <returns>Return the updated Category in JSON </returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(GetCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            var validationResults = await ValidateAsync(request);
            if (!validationResults.IsValid)
            {
                return BadRequest(validationResults.ToModelStateDictionary());
            }


            var foundTodo = await _categoryService.UpdateAsync(id, request);

            if (foundTodo is null)
            {
                return NotFound();
            }


            return Ok(GetCategoryResponse.From(foundTodo));
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while updating the todo");
        }
    }

    /// <summary>
    /// Delete Category
    /// </summary>
    /// <returns>Return no content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCategory([FromRoute] int id)
    {
        var wasDeleted = await _categoryService.RemoveAsync(id);
        if (wasDeleted == false)
        {
            return NotFound();
        }

        return NoContent();
    }
}