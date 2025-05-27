namespace FUNewsManagementWebRazorPage.Utils
{
    public class ShortRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route,
                          string routeKey, RouteValueDictionary values,
                          RouteDirection routeDirection)
        {
            if (!values.TryGetValue(routeKey, out var value)) return false;
            return short.TryParse(Convert.ToString(value), out _);
        }
    }
}
