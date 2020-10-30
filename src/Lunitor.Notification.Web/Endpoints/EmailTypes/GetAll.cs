﻿using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Lunitor.Notification.Core.Repository;
using Lunitor.Notification.Web.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lunitor.Notification.Web.Endpoints.EmailTypes
{
    public class GetAll : BaseAsyncEndpoint<IEnumerable<EmailType>>
    {
        private readonly IEmailFactoryTypeRepository _emailFactoryTypeRepository;

        public GetAll(IEmailFactoryTypeRepository emailFactoryTypeRepository)
        {
            Guard.Against.Null(emailFactoryTypeRepository, nameof(emailFactoryTypeRepository));
            _emailFactoryTypeRepository = emailFactoryTypeRepository;
        }

        [HttpGet("/api/getemailtypes")]
        public override async Task<ActionResult<IEnumerable<EmailType>>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var typeNames = _emailFactoryTypeRepository.GetAllNames();
            var emailTypes = typeNames.Select(name => new EmailType
            {
                Name = name,
                Placeholders = _emailFactoryTypeRepository.GetPlaceholders(name)
            });

            return Ok(emailTypes);
        }
    }
}
