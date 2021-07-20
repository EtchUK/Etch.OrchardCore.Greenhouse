using Etch.OrchardCore.Greenhouse.Services.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Etch.OrchardCore.Greenhouse.ModelBinding
{
    public interface IGreenhouseCandidateModelBinder
    {
        object Bind(ModelStateDictionary modelState, HttpRequest request, GreenhouseQuestion question);
    }
}
