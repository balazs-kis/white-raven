﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WhiteRaven.Domain.Models.Authentication;
using WhiteRaven.Domain.Operations.Interfaces;
using WhiteRaven.Web.Api.Examples;

namespace WhiteRaven.Web.Api.Controllers
{
    /// <summary>
    /// API controller for User operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserOperations _userOperations;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class
        /// </summary>
        /// <param name="userOperations">The user operations</param>
        public UsersController(IUserOperations userOperations)
        {
            _userOperations = userOperations;
        }


        /// <summary>
        /// Creates (registers) a new user
        /// </summary>
        /// <param name="registration">The registration info</param>
        /// <returns>The created user</returns>
        [AllowAnonymous]
        [HttpPost]
        [SwaggerRequestExample(typeof(Registration), typeof(RegistrationExample))]
        public async Task<IActionResult> RegisterUser([Required, FromBody]Registration registration)
        {
            var newUser = await _userOperations.CreateUser(registration);
            return CreatedAtAction(nameof(GetUser), new { id = registration.Email }, JsonApi.DataObject(newUser));
        }

        /// <summary>
        /// Returns a user by its unique ID (email address)
        /// </summary>
        /// <param name="id">The user's ID</param>
        /// <returns>The user with the given ID</returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute]string id) =>
            JsonApi.OkDataObject(await _userOperations.GetUser(id));

        /// <summary>
        /// Searches for users by (partial) email address
        /// </summary>
        /// <param name="email">The (partial) email address</param>
        /// <returns>A list of matching users</returns>
        [Authorize]
        [HttpGet("search/email/{email}")]
        public async Task<IActionResult> SearchUsersByEmail([FromRoute]string email) =>
            JsonApi.OkDataObject(await _userOperations.SearchUserByEmail(email));

        /// <summary>
        /// Searches for users by (partial) first name
        /// </summary>
        /// <param name="firstName">The (partial) first name</param>
        /// <returns>A list of matching users</returns>
        [Authorize]
        [HttpGet("search/firstname/{firstName}")]
        public Task<IActionResult> SearchUsersByFirstName([FromRoute]string firstName) =>
            SearchUser(firstName, null);

        /// <summary>
        /// Searches for users by (partial) last name
        /// </summary>
        /// <param name="lastName">The (partial) last name</param>
        /// <returns>A list of matching users</returns>
        [Authorize]
        [HttpGet("search/lastname/{lastName}")]
        public Task<IActionResult> SearchUsersByLastName([FromRoute]string lastName) =>
            SearchUser(null, lastName);

        /// <summary>
        /// Searches for users by (partial) first and last names
        /// </summary>
        /// <param name="firstName">The (partial) first name</param>
        /// <param name="lastName">The (partial) last name</param>
        /// <returns>A list of matching users</returns>
        [Authorize]
        [HttpGet("search/firstname/{firstName}/lastname/{lastName}")]
        public Task<IActionResult> SearchUsersByFullName([FromRoute]string firstName, [FromRoute]string lastName) =>
            SearchUser(firstName, lastName);

        /// <summary>
        /// Changes the user's password
        /// </summary>
        /// <param name="passwords">The old and the new password</param>
        /// <returns>No content</returns>
        [Authorize]
        [HttpPatch("update/password")]
        [SwaggerRequestExample(typeof(PasswordUpdate), typeof(PasswordUpdateExample))]
        public async Task<IActionResult> ChangePassword([Required, FromBody]PasswordUpdate passwords)
        {
            var email = GetCurrentUserEmailAddress();
            await _userOperations.UpdateUserPassword(email, passwords);

            return NoContent();
        }

        /// <summary>
        /// Updates the user's information
        /// </summary>
        /// <param name="newInfo">The updated user information</param>
        /// <returns>The updated user</returns>
        [Authorize]
        [HttpPatch("update/info")]
        [SwaggerRequestExample(typeof(InfoUpdate), typeof(InfoUpdateExample))]
        public async Task<IActionResult> UpdateUserInfo([Required, FromBody]InfoUpdate newInfo)
        {
            var email = GetCurrentUserEmailAddress();
            var updatedUser = await _userOperations.UpdateUserInfo(email, newInfo);

            return JsonApi.OkDataObject(updatedUser);
        }


        private async Task<IActionResult> SearchUser(string partialFirstName, string partialLastName) =>
            JsonApi.OkDataObject(await _userOperations.SearchUserByName(partialFirstName, partialLastName));
    }
}