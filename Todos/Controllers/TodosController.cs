using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_be.Database;

namespace todo_be.Todos.Controllers;

public class TodosController : BaseController
{
    private readonly ILogger<TodosController> _logger;
    private readonly AppDbContext _dbContext;

    public TodosController(
        ILogger<TodosController> logger,
        AppDbContext dbContext
    )
    {
        _logger = logger;
        _dbContext = dbContext;
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
        var todos = await _dbContext.Todos.Include(t => t.Comments).ToArrayAsync();

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
        var foundTodo = await _dbContext.Todos.Include(t => t.Comments).SingleOrDefaultAsync(t => t.Id == id);

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


        var newTodo = new Todo
        {
            Title = request.Title,
            Description = request.Description,
            IsCompleted = request.IsCompleted,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Comments = []
        };

        _dbContext.Todos.Add(newTodo);
        await _dbContext.SaveChangesAsync();

        return Created($"/todo/{newTodo.Id}", newTodo);
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
        _logger.LogInformation("Updating todo with ID: {TodoId}", id);

        var foundTodo = await _dbContext.Todos.FindAsync(id);

        if (foundTodo is null)
        {
            _logger.LogWarning("Todo with ID: {TodoId} not found", id);
            return NotFound();
        }

        _logger.LogDebug("Updating todo details for ID: {TodoId}", id);

        foundTodo.Id = id;
        foundTodo.Title = request.Title;
        foundTodo.Description = request.Description;
        foundTodo.IsCompleted = request.IsCompleted;


        try
        {
            await _dbContext.SaveChangesAsync();
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
        var foundTodo = await _dbContext.Todos.FindAsync(id);
        if (foundTodo == null)
        {
            return NotFound();
        }

        _dbContext.Todos.Remove(foundTodo);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }


    /// <summary>
    /// Add Todo Comment
    /// </summary>
    /// <returns>Return status 201</returns>
    [HttpPost("{todoId}/comments")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(typeof(GetTodoCommentResponse), StatusCodes.Status201Created)]
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


        var foundTodo = await _dbContext.Todos.FindAsync(todoId);

        if (foundTodo == null)
        {
            return NotFound("Todo-ul pe care incerci sa-l accesezi nu exista");
        }

        var comment = new TodoComment
        {
            TodoId = todoId,
            Content = request.Content,
            CreatedAt = DateTime.Now
        };

        _dbContext.TodoComments.Add(comment);
        await _dbContext.SaveChangesAsync();

        return Created($"/todos/{todoId}/comments/{comment.Id}", comment);
    }

    /// <summary>
    /// Mark TODO as completed
    /// </summary>
    /// <returns>Return status 204</returns>
    [HttpPut("{todoId}/completed")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkTodoAsCompleted([FromRoute] int todoId)
    {
        var foundTodo = await _dbContext.Todos
            .AsTracking()
            .FirstOrDefaultAsync(t => t.Id == todoId);

        if (foundTodo == null)
        {
            return NotFound("Todo-ul nu exista!");
        }


        if (!foundTodo.IsCompleted)
        {
            foundTodo.IsCompleted = true;
            foundTodo.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        return NoContent();
    }
}