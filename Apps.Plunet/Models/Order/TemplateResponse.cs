using Blackbird.Plugins.Plunet.DataOrder30Service;

namespace Blackbird.Plugins.Plunet.Models.Order;

public class TemplateResponse
{
    public string Id { get; set; }
    public string Name { get; set; }

    public TemplateResponse(Template template)
    {
        Id = template.templateID.ToString();
        Name = template.templateName;
    }
}