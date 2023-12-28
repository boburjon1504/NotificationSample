﻿using FluentValidation;
using Sms.Infrastructure.Domain.Enum;
using Sms.Infrustructure.Application.Common.Identity.Service;
using Sms.Infrustructure.Application.Common.Notification.Model;

namespace Sms.Infrostucture.Infrastructure.Common.Validators;
public class NotificationRequestValidator : AbstractValidator<NotificationRequest>

{
    public NotificationRequestValidator(IUserService userService)
    {
        // TODO : to external
        var templatesRequireSender = new List<NotificationTemplateType>
        {
            NotificationTemplateType.ReferralNotification
        };

        RuleFor(request => request.SenderUserId)
            .NotEqual(Guid.Empty)
            .NotNull()
            .When(request => templatesRequireSender.Contains(request.TemplateType))
            .CustomAsync(async (senderUserId, context, cancellationToken) =>
            {
                var user = await userService.GetByIdAsync(senderUserId!.Value, true, cancellationToken);

                if (user is null)
                    context.AddFailure("Sender user not found");
            });

        RuleFor(request => request.ReceiverUserId)
            .NotEqual(Guid.Empty)
            .CustomAsync(async (senderUserId, context, cancellationToken) =>
            {
                var user = await userService.GetByIdAsync(senderUserId, true, cancellationToken);

                if (user is null)
                    context.AddFailure("Sender user not found");
            });
    }
}
