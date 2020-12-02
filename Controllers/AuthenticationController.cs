using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System;
using System.Collections.Generic;
using AutoMapper;
using TodoApp.Models;
using TodoApp.Dtos;

namespace TodoApp.Controllers
{
    public class AuthenticationController: ControllerBase
    {
        private readonly UserManager<ApplicationsUser> userManager;
        private readonly SignInManager<ApplicationsUser> signInManager;
        private readonly IConfiguration configuration;

        public AuthenticationController(UserManager<ApplicationsUser> userManager, SignInManager<ApplicationsUser> signInManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [HttpPost("register-user")]
        [Consumes(System.Net.Mime.MediaTypeNames.Application.Json)]
        [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<ActionResult> RegisterUserAccount([FromBody] REGISTER_USER_REQUEST_DTO request)
        {
            try
            {
                ApplicationsUser applicationsUser = new ApplicationsUser()
                {
                    UserName = request.Username,
                    Email = request.Email
                };
                IdentityResult result = await this.userManager.CreateAsync(applicationsUser,request.Password);
                if(result.Succeeded)
                {
                    return StatusCode(201,new { message="User created successfully" });
                }
                else
                {
                    return StatusCode(500);
                }
            }
            catch (System.Exception)
            {
                return StatusCode(500);
            }
        }
   
        [HttpPost("access-user")]
        [Consumes(System.Net.Mime.MediaTypeNames.Application.Json)]
        [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
        public async Task<ActionResult<ACCESS_USER_RESPONSE_DTO>> AccessUserAccount([FromBody] ACCESS_USER_REQUEST_DTO request)
        {
            try
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await this.signInManager.PasswordSignInAsync(request.Username, request.Password, isPersistent: false, lockoutOnFailure: false);
                if(result.Succeeded)
                {
                    return this.GenerateToken(request);
                }
                else
                {
                    return Unauthorized(new { message="User or password incorrect" });
                }
            }
            catch (System.Exception)
            {
                return StatusCode(500);
            }
        }

        private ACCESS_USER_RESPONSE_DTO GenerateToken(ACCESS_USER_REQUEST_DTO request)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName,request.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JWT:key"]));
            SigningCredentials credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            DateTime expiration = DateTime.UtcNow.AddMonths(1);

            JwtSecurityToken token = new JwtSecurityToken(issuer:null,audience:null,claims:claims,expires:expiration,signingCredentials:credentials);
            return new ACCESS_USER_RESPONSE_DTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}