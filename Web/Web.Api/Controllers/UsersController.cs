﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;
using System.Threading.Tasks;
using WhiteRaven.Domain.Models.Authentication;
using WhiteRaven.Domain.Operations.Interfaces;
using WhiteRaven.Web.Api.Examples;

namespace WhiteRaven.Web.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserOperations _userOperations;


        public UsersController(IUserOperations userOperations)
        {
            _userOperations = userOperations;
        }


        [AllowAnonymous]
        [HttpPost]
        [SwaggerRequestExample(typeof(Registration), typeof(RegistrationExample))]
        public async Task<IActionResult> RegisterUser([FromBody] Registration registration)
        {
            var newUser = await _userOperations.CreateUser(registration);
            return CreatedAtAction(nameof(GetUser), new { id = registration.Email }, JsonApi.DataObject(newUser));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id) =>
            JsonApi.OkDataObject(await _userOperations.GetUser(id));

        [Authorize]
        [HttpGet("search/email/{partialEmail}")]
        public async Task<IActionResult> SearchUsersByEmail(string partialEmail) =>
            JsonApi.OkDataObject(await _userOperations.SearchUserByEmail(partialEmail));

        [Authorize]
        [HttpGet("search/firstname/{partialFirstName}")]
        public Task<IActionResult> SearchUsersByFirstName(string partialFirstName) =>
            SearchUser(partialFirstName, null);

        [Authorize]
        [HttpGet("search/lastname/{partialLastName}")]
        public Task<IActionResult> SearchUsersByLastName(string partialLastName) =>
            SearchUser(null, partialLastName);

        [Authorize]
        [HttpGet("search/firstname/{partialFirstName}/lastname/{partialLastName}")]
        public Task<IActionResult> SearchUsersByFullName(string partialFirstName, string partialLastName) =>
            SearchUser(partialFirstName, partialLastName);

        [Authorize]
        [HttpPatch("update/password")]
        [SwaggerRequestExample(typeof(PasswordUpdate), typeof(PasswordUpdateExample))]
        public async Task<IActionResult> ChangePassword([FromBody]PasswordUpdate passwords)
        {
            var email = GetCurrentUserEmailAddress();
            await _userOperations.UpdateUserPassword(email, passwords);

            return NoContent();
        }

        [Authorize]
        [HttpPatch("update/info")]
        [SwaggerRequestExample(typeof(InfoUpdate), typeof(InfoUpdateExample))]
        public async Task<IActionResult> UpdateUserInfo([FromBody]InfoUpdate newInfo)
        {
            var email = GetCurrentUserEmailAddress();
            var updatedUser = await _userOperations.UpdateUserInfo(email, newInfo);

            return JsonApi.OkDataObject(updatedUser);
        }


        private async Task<IActionResult> SearchUser(string partialFirstName, string partialLastName) =>
            JsonApi.OkDataObject(await _userOperations.SearchUserByName(partialFirstName, partialLastName));
    }
}