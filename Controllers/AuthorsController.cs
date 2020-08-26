using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   [ProducesResponseType(StatusCodes.Status200OK)]
   public class AuthorsController : ControllerBase
   {
      private readonly IAuthorRepository _authorRepository;
      private readonly ILoggerService _logger;
      private readonly IMapper _mapper;
      public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
      {
         _authorRepository = authorRepository;
         _logger = logger;
         _mapper = mapper;
      }

      /// <summary>
      /// Get all Authors
      /// </summary>
      /// <returns>List of Authors</returns>
      [HttpGet]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status500InternalServerError)]
      public async Task<IActionResult> GetAuthors()
      {
         try
         {
            _logger.LogInfo("Attempted Get All Authors");
            var authors = await _authorRepository.FindAll();
            var response = _mapper.Map<IList<AuthorDTO>>(authors);
            _logger.LogInfo("Successfully got all Authors");
            return Ok(response);
         }
         catch (Exception e)
         {
            return InternalError($"{ e.Message} - { e.InnerException}");
         }

      }
      /// <summary>
      /// Get an author by ID
      /// </summary>
      /// <param name="id"></param>
      /// <returns>An author's record</returns>

      [HttpGet("{id}")]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status500InternalServerError)]
      [ProducesResponseType(StatusCodes.Status404NotFound)]
      public async Task<IActionResult> GetAuthor(int id)
      {
         try
         {
            _logger.LogInfo("Attempted to get author with id:{id}");
            var author = await _authorRepository.FindById(id);
            if (author == null)
            {
               _logger.LogWarn($"author with id:{ id}");
               return NotFound();
            }
            var response = _mapper.Map<AuthorDTO>(author);
            _logger.LogInfo("Successfully got authors with id:{id}");
            return Ok(response);
         }
         catch (Exception e)
         {
            return InternalError($"{ e.Message} - { e.InnerException}");
         }

      }
      private ObjectResult InternalError(string message)
      {
         _logger.LogError(message);
         return StatusCode(500, "Something went wrong.  Please contact the administrator");
      }

      /// <summary>
      /// Creates an author
      /// </summary>
      /// <param name="authorDTO"></param>
      /// <returns></returns>
      [HttpPost]
      [ProducesResponseType(StatusCodes.Status201Created)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      [ProducesResponseType(StatusCodes.Status500InternalServerError)]
      public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
      {
         try
         {
            _logger.LogWarn($"Author Submission Attempted");
            if (authorDTO == null)
            {
               _logger.LogWarn($"Empty Rrequest was submitted");
               return BadRequest(ModelState);
            }
            if (!ModelState.IsValid)
            {
               _logger.LogWarn($"Author data was incomplete");
               return BadRequest(ModelState);
            }
            var author = _mapper.Map<Author>(authorDTO);
            var isSuccess = await _authorRepository.Create(author);
            if (!isSuccess)
            {
               return InternalError($"Author creation failed");
            }
            _logger.LogInfo("Author Created");
            return Created("Create", new { author });

         }
         catch (Exception e)
         {
            return InternalError($"{ e.Message} - { e.InnerException}");
         }

      }
      /// <summary>
      /// Updates an author
      /// </summary>
      /// <param name="id"></param>
      /// <param name="authorDTO"></param>
      /// <returns></returns>
      [HttpPut("{id}")]
      [ProducesResponseType(StatusCodes.Status204NoContent)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      [ProducesResponseType(StatusCodes.Status500InternalServerError)]
      public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
      {
         try
         {
            _logger.LogInfo("Author with id: {id} update attempted");
            if (id < 1 || authorDTO == null || id != authorDTO.Id)
            {
               _logger.LogWarn($"Author update failed with bad data");
               return BadRequest();
            }
            var isExists = await _authorRepository.isExists(id);
            if (!isExists)
            {
               _logger.LogWarn($"Author with id:{id} was not found");
               return NotFound();
            }
            if (!ModelState.IsValid)
            {
               _logger.LogInfo("Author data was incomplete");
               return BadRequest(ModelState);
            }
            var author = _mapper.Map<Author>(authorDTO);
            var isSuccess = await _authorRepository.Update(author);
            if (!isSuccess)
            {

               return InternalError($"Update operation failed");
            }
            return NoContent();
         }
         catch (Exception e)
         {
            return InternalError($"{ e.Message} - { e.InnerException}");
         }
      }
      /// <summary>
      /// Removes an author by ID
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      [HttpDelete("{id}")]
      [ProducesResponseType(StatusCodes.Status204NoContent)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      [ProducesResponseType(StatusCodes.Status500InternalServerError)]
      public async Task<IActionResult> Delete(int id)
      {
         try
         {
            _logger.LogWarn($"Author with id: {id} delete attempted");
            if (id < 1)
            {
               _logger.LogWarn($"Author delete failed with bad data");
               return BadRequest();
            }
            var isExists = await _authorRepository.isExists(id);
            if (!isExists)
            {
               _logger.LogWarn($"Author with id:{id} was not found");
               return NotFound();
            }
            var author = await _authorRepository.FindById(id);                       
            var isSuccess = await _authorRepository.Delete(author);
            if (!isSuccess)
            {
               return InternalError($"Author delet failed");
            }
            _logger.LogWarn($"Author with id:{id} successfully deleted");
            return NoContent();
         }
         catch (Exception e)
         {
            return InternalError($"{ e.Message} - { e.InnerException}");
         }
      }
   }
}
