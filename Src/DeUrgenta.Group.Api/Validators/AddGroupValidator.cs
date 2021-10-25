﻿using System.Threading.Tasks;
using DeUrgenta.Common.Validation;
using DeUrgenta.Domain.Api;
using DeUrgenta.Group.Api.Commands;
using DeUrgenta.Group.Api.Options;
using DeUrgenta.I18n.Service.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DeUrgenta.Group.Api.Validators
{
    public class AddGroupValidator : IValidateRequest<AddGroup>
    {
        private readonly DeUrgentaContext _context;
        private readonly IamI18nProvider _i18nProvider;
        private readonly GroupsConfig _groupsConfig;

        public AddGroupValidator(DeUrgentaContext context, IamI18nProvider i18nProvider, IOptions<GroupsConfig> groupsConfig)
        {
            _context = context;
            _i18nProvider = i18nProvider;
            _groupsConfig = groupsConfig.Value;
        }

        public async Task<ValidationResult> IsValidAsync(AddGroup request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Sub == request.UserSub);
            if (user == null)
            {
                return ValidationResult.GenericValidationError;
            }

            if (user.GroupsAdministered.Count >= _groupsConfig.MaxCreatedGroupsPerUser)
            {
                return new DetailedValidationError("Cannot create more groups", "You have reached maximum number of groups per user.");
            }

            return ValidationResult.Ok;
        }
    }
}