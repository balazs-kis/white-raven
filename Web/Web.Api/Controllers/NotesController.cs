﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;
using System.Threading.Tasks;
using WhiteRaven.Domain.Models.Note;
using WhiteRaven.Domain.Operations.Interfaces;
using WhiteRaven.Web.Api.Examples;

namespace WhiteRaven.Web.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : Controller
    {
        private readonly INoteOperations _noteOperations;


        public NotesController(INoteOperations noteOperations)
        {
            _noteOperations = noteOperations;
        }


        [Authorize]
        [HttpGet("all")]
        public Task<IActionResult> GetAllNotes() =>
            GetNotes();

        [Authorize]
        [HttpGet("mine")]
        public Task<IActionResult> GetMyNotes() =>
            GetNotes(ContributionType.Owner);

        [Authorize]
        [HttpGet("shared/read")]
        public Task<IActionResult> GetSharedReadOnlyNotes() =>
            GetNotes(ContributionType.Reader);

        [Authorize]
        [HttpGet("shared/write")]
        public Task<IActionResult> GetSharedWritableNotes() =>
            GetNotes(ContributionType.Writer);

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var email = GetCurrentUserEmailAddress();
            return JsonApi.OkDataObject(await _noteOperations.GetNoteById(email, id));
        }

        [Authorize]
        [HttpPost]
        [SwaggerRequestExample(typeof(Commit), typeof(CommitExample))]
        public async Task<IActionResult> Create([FromBody]Commit commit)
        {
            var email = GetCurrentUserEmailAddress();
            var note = await _noteOperations.CreateNote(email, commit);

            return CreatedAtAction(nameof(GetById), new { id = note.Id }, JsonApi.DataObject(note));
        }

        [Authorize]
        [HttpPatch("{id}")]
        [SwaggerRequestExample(typeof(Commit), typeof(CommitExample))]
        public async Task<IActionResult> Update(string id, [FromBody] Commit commit)
        {
            var email = GetCurrentUserEmailAddress();
            return JsonApi.OkDataObject(await _noteOperations.UpdateNote(email, id, commit));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var email = GetCurrentUserEmailAddress();
            await _noteOperations.DeleteNote(email, id);

            return NoContent();
        }


        private async Task<IActionResult> GetNotes(ContributionType? contributionType = null)
        {
            var email = GetCurrentUserEmailAddress();
            return JsonApi.OkDataObject(await _noteOperations.GetNotesByUser(email, contributionType));
        }
    }
}