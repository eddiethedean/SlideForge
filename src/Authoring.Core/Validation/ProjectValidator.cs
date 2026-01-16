using Authoring.Core.Models;

namespace Authoring.Core.Validation;

/// <summary>
/// Provides validation for Project objects.
/// </summary>
public static class ProjectValidator
{
    /// <summary>
    /// Validates a project and returns a list of validation errors.
    /// </summary>
    /// <param name="project">The project to validate.</param>
    /// <returns>A list of error messages. Empty list if validation passes.</returns>
    public static List<string> ValidateProject(Project project)
    {
        var errors = new List<string>();

        if (project == null)
        {
            errors.Add("Project cannot be null.");
            return errors;
        }

        // Validate project name
        if (string.IsNullOrWhiteSpace(project.Name))
        {
            errors.Add("Project name cannot be empty.");
        }

        // Validate slide IDs are unique
        var slideIds = project.Slides.Select(s => s.Id).ToList();
        var duplicateSlideIds = slideIds.GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        foreach (var duplicateId in duplicateSlideIds)
        {
            errors.Add($"Duplicate slide ID found: {duplicateId}");
        }

        // Validate variable IDs are unique
        var variableIds = project.Variables.Select(v => v.Id).ToList();
        var duplicateVariableIds = variableIds.GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        foreach (var duplicateId in duplicateVariableIds)
        {
            errors.Add($"Duplicate variable ID found: {duplicateId}");
        }

        // Validate each slide
        foreach (var slide in project.Slides)
        {
            ValidateSlide(slide, project, errors);
        }

        // Validate variable references in actions
        ValidateVariableReferences(project, errors);

        // Validate slide references in navigation actions
        ValidateSlideReferences(project, errors);

        // Validate layer references in actions
        ValidateLayerReferences(project, errors);

        // Validate object references in triggers
        ValidateObjectReferences(project, errors);

        return errors;
    }

    private static void ValidateSlide(Slide slide, Project project, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(slide.Title))
        {
            errors.Add($"Slide '{slide.Id}' has an empty title.");
        }

        if (slide.Width <= 0)
        {
            errors.Add($"Slide '{slide.Id}' has invalid width: {slide.Width}");
        }

        if (slide.Height <= 0)
        {
            errors.Add($"Slide '{slide.Id}' has invalid height: {slide.Height}");
        }

        // Validate layer IDs are unique within the slide
        var layerIds = slide.Layers.Select(l => l.Id).ToList();
        var duplicateLayerIds = layerIds.GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        foreach (var duplicateId in duplicateLayerIds)
        {
            errors.Add($"Duplicate layer ID '{duplicateId}' found in slide '{slide.Id}'");
        }

        // Validate object IDs are unique within layers
        foreach (var layer in slide.Layers)
        {
            var objectIds = layer.Objects.Select(o => o.Id).ToList();
            var duplicateObjectIds = objectIds.GroupBy(id => id)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            foreach (var duplicateId in duplicateObjectIds)
            {
                errors.Add($"Duplicate object ID '{duplicateId}' found in layer '{layer.Id}' of slide '{slide.Id}'");
            }

            // Validate triggers have at least one action
            foreach (var obj in layer.Objects)
            {
                foreach (var trigger in obj.Triggers)
                {
                    if (trigger.Actions.Count == 0)
                    {
                        errors.Add($"Trigger '{trigger.Id}' on object '{obj.Id}' has no actions.");
                    }
                }
            }
        }
    }

    private static void ValidateVariableReferences(Project project, List<string> errors)
    {
        var variableIds = project.Variables.Select(v => v.Id).ToHashSet();

        foreach (var slide in project.Slides)
        {
            foreach (var layer in slide.Layers)
            {
                foreach (var obj in layer.Objects)
                {
                    foreach (var trigger in obj.Triggers)
                    {
                        foreach (var action in trigger.Actions)
                        {
                            if (action is SetVariableAction setVarAction)
                            {
                                if (!variableIds.Contains(setVarAction.VariableId))
                                {
                                    errors.Add($"SetVariableAction in trigger '{trigger.Id}' references non-existent variable '{setVarAction.VariableId}'");
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private static void ValidateSlideReferences(Project project, List<string> errors)
    {
        var slideIds = project.Slides.Select(s => s.Id).ToHashSet();

        foreach (var slide in project.Slides)
        {
            foreach (var layer in slide.Layers)
            {
                foreach (var obj in layer.Objects)
                {
                    foreach (var trigger in obj.Triggers)
                    {
                        foreach (var action in trigger.Actions)
                        {
                            if (action is NavigateToSlideAction navAction)
                            {
                                if (!slideIds.Contains(navAction.TargetSlideId))
                                {
                                    errors.Add($"NavigateToSlideAction in trigger '{trigger.Id}' references non-existent slide '{navAction.TargetSlideId}'");
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private static void ValidateLayerReferences(Project project, List<string> errors)
    {
        foreach (var slide in project.Slides)
        {
            var layerIds = slide.Layers.Select(l => l.Id).ToHashSet();

            foreach (var layer in slide.Layers)
            {
                foreach (var obj in layer.Objects)
                {
                    foreach (var trigger in obj.Triggers)
                    {
                        foreach (var action in trigger.Actions)
                        {
                            string? referencedLayerId = null;
                            if (action is ShowLayerAction showAction)
                            {
                                referencedLayerId = showAction.LayerId;
                            }
                            else if (action is HideLayerAction hideAction)
                            {
                                referencedLayerId = hideAction.LayerId;
                            }

                            if (referencedLayerId != null && !layerIds.Contains(referencedLayerId))
                            {
                                errors.Add($"Layer action in trigger '{trigger.Id}' references non-existent layer '{referencedLayerId}' in slide '{slide.Id}'");
                            }
                        }
                    }
                }
            }
        }
    }

    private static void ValidateObjectReferences(Project project, List<string> errors)
    {
        foreach (var slide in project.Slides)
        {
            var objectIds = new HashSet<string>();
            foreach (var layer in slide.Layers)
            {
                foreach (var obj in layer.Objects)
                {
                    objectIds.Add(obj.Id);
                }
            }

            // Check triggers that reference objects
            foreach (var layer in slide.Layers)
            {
                foreach (var obj in layer.Objects)
                {
                    foreach (var trigger in obj.Triggers)
                    {
                        if (trigger.ObjectId != null && !objectIds.Contains(trigger.ObjectId))
                        {
                            errors.Add($"Trigger '{trigger.Id}' references non-existent object '{trigger.ObjectId}' in slide '{slide.Id}'");
                        }
                    }
                }
            }
        }
    }
}
