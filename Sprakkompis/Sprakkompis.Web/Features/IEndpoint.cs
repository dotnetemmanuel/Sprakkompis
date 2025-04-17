namespace Sprakkompis.Web.Endpoints;

public interface IEndpoint
{
    public static abstract void MapEndpoint(IEndpointRouteBuilder app);
}
