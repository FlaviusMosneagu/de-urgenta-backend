﻿using System.Threading.Tasks;
using DeUrgenta.Common.Validation;
using DeUrgenta.Domain.Api;
using DeUrgenta.Group.Api.Commands;
using DeUrgenta.I18n.Service.Providers;
using Microsoft.EntityFrameworkCore;

namespace DeUrgenta.Group.Api.Validators
{
    public class DeleteSafeLocationValidator : IValidateRequest<DeleteSafeLocation>
    {
        private readonly DeUrgentaContext _context;
        private readonly IamI18nProvider _i18nProvider;

        public DeleteSafeLocationValidator(DeUrgentaContext context, IamI18nProvider i18nProvider)
        {
            _context = context;
            _i18nProvider = i18nProvider;
        }

        public async Task<ValidationResult> IsValidAsync(DeleteSafeLocation request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Sub == request.UserSub);
            if (user == null)
            {
                return ValidationResult.GenericValidationError;
            }

            var safeLocationExists = await _context.GroupsSafeLocations.AnyAsync(gsl => gsl.Id == request.SafeLocationId);
            if (!safeLocationExists)
            {
                return ValidationResult.GenericValidationError;
            }

            var isGroupAdmin = await _context.GroupsSafeLocations
                .AnyAsync(gsl => gsl.Group.Admin.Id == user.Id && gsl.Id == request.SafeLocationId);

            if (!isGroupAdmin)
            {
                return new DetailedValidationError("Cannot delete safe locations", "Only group admins can delete safe locations.");
            }

            return ValidationResult.Ok;
        }
    }
}