using Microsoft.AspNetCore.Mvc;
using todo_be.Todos.Service;

namespace todo_be.Todos.Controllers;

public class TodosController : BaseController
{
    private readonly ILogger<TodosController> _logger;
    private readonly ITodoService _todoService;

    public TodosController(
        ILogger<TodosController> logger,
        ITodoService todoService
    )
    {
        _logger = logger;
        _todoService = todoService;
    }


    /// <summary>
    /// Gets all todos available
    /// </summary>
    /// <returns>Return the Todos in JSON array</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetTodoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllTodos()
    {
        _logger.LogInformation("Getting all Todos");
        var todos = await _todoService.GetAllAsync();

        return Ok(todos.Select(todo => new GetTodoResponse
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            Comments = todo.Comments,
            CreatedAt = todo.CreatedAt,
            UpdatedAt = todo.UpdatedAt
        }));
    }

    /// <summary>
    /// Gets todo by ID
    /// </summary>
    /// <returns>Return the Todo in JSON </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetTodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTodoById([FromRoute] int id)
    {
        var foundTodo = await _todoService.GetByIdAsync(id);

        if (foundTodo == null)
        {
            return NotFound();
        }

        var todoResponse = new GetTodoResponse
        {
            Id = foundTodo.Id,
            Title = foundTodo.Title,
            Description = foundTodo.Description,
            IsCompleted = foundTodo.IsCompleted,
            Comments = foundTodo.Comments,
            CreatedAt = foundTodo.CreatedAt,
            UpdatedAt = foundTodo.UpdatedAt
        };

        return Ok(todoResponse);
    }


    /// <summary>
    /// Create Todo
    /// </summary>
    /// <returns>Return the new created Todo in JSON </returns>
    [HttpPost]
    [ProducesResponseType(typeof(GetTodoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTodo([FromBody] CreateTodoRequest request)
    {
        var validationResults = await ValidateAsync(request);
        if (!validationResults.IsValid)
        {
            return BadRequest(validationResults.ToModelStateDictionary());
        }


        var newTodo = await _todoService.CreateAsync(request);

        return Created($"/todos/{newTodo.Id}", newTodo);
    }


    /// <summary>
    /// Update Todo
    /// </summary>
    /// <returns>Return the updated Todo in JSON </returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(GetTodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateTodo([FromRoute] int id, [FromBody] UpdateTodoRequest request)
    {
        try
        {
            _logger.LogInformation("Updating todo with ID: {TodoId}", id);

            var foundTodo = await _todoService.UpdateAsync(id, request);

            if (foundTodo is null)
            {
                _logger.LogWarning("Todo with ID: {TodoId} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Todo with ID: {TodoId} successfully updated", id);

            return Ok(foundTodo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating todo with ID: {TodoId}", id);
            return StatusCode(500, "An error occurred while updating the todo");
        }
    }

    /// <summary>
    /// Delete Todo
    /// </summary>
    /// <returns>Return no content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTodo([FromRoute] int id)
    {
        var foundTodo = await _todoService.RemoveAsync(id);
        if (foundTodo == false)
        {
            return NotFound();
        }

        return NoContent();
    }


    /// <summary>
    /// Add Todo Comment
    /// </summary>
    /// <returns>Return status 201</returns>
    [HttpPost("{todoId}/comments")]
    [ProducesResponseType(typeof(GetTodoCommentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTodoComment(
        [FromRoute] int todoId,
        [FromBody] CreateTodoCommentRequest request)
    {
        var validationResults = await ValidateAsync(request);
        if (!validationResults.IsValid)
        {
            return BadRequest(validationResults.ToModelStateDictionary());
        }


        var newComment = await _todoService.CreateCommentAsync(todoId, request);

        if (newComment == null)
        {
            return NotFound("Todo-ul pe care incerci sa-l accesezi nu exista");
        }


        return Created($"/todos/{todoId}/comments/{newComment.Id}", newComment);
    }

    /// <summary>
    /// Toggle TODO completion
    /// </summary>
    /// <returns>Return status 204</returns>
    [HttpPut("{todoId}/toggle-completion")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkTodoAsCompleted([FromRoute] int todoId)
    {
        var foundTodo = await _todoService.ToggleCompletedAsync(todoId);

        if (!foundTodo)
        {
            return NotFound("Todo-ul nu exista!");
        }


        return NoContent();
    }
}