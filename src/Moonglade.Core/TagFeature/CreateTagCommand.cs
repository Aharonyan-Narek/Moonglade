﻿using Moonglade.Data.Spec;
using Moonglade.Utils;

namespace Moonglade.Core.TagFeature;

public record CreateTagCommand(string Name) : IRequest<Tag>;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Tag>
{
    private readonly IRepository<TagEntity> _tagRepo;

    public CreateTagCommandHandler(IRepository<TagEntity> tagRepo) => _tagRepo = tagRepo;

    public async Task<Tag> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        if (!Tag.ValidateName(request.Name)) return null;

        var normalizedName = Tag.NormalizeName(request.Name, Helper.TagNormalizationDictionary);
        if (await _tagRepo.AnyAsync(t => t.NormalizedName == normalizedName))
        {
            return _tagRepo.SelectFirstOrDefault(new TagSpec(normalizedName), Tag.EntitySelector);
        }

        var newTag = new TagEntity
        {
            DisplayName = request.Name,
            NormalizedName = normalizedName
        };

        var tag = await _tagRepo.AddAsync(newTag, cancellationToken);

        return new()
        {
            DisplayName = tag.DisplayName,
            NormalizedName = tag.NormalizedName
        };
    }
}