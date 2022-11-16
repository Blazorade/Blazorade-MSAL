using Blazorade.Msal.Security;
using Shouldly;
using Xunit;

namespace Blazorade.Msal.UnitTests;

public class AuthenticationResultTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2Njg2MTM0MjAsImV4cCI6MTcwMDE0OTQyMCwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoianJvY2tldEBleGFtcGxlLmNvbSIsIkdpdmVuTmFtZSI6IkpvaG5ueSIsIlN1cm5hbWUiOiJSb2NrZXQiLCJFbWFpbCI6Impyb2NrZXRAZXhhbXBsZS5jb20iLCJyb2xlcyI6Ik1hbmFnZXIifQ.Vgs8ZRdukLMr2u-AmhxNR")]
    public void Should_return_accesstoken_as_empty_claims_list_on_invalid_token(string? jwt)
    {
        var authResult = new AuthenticationResult
        {
            AccessToken = jwt
        };
        
        authResult.AccessTokenClaims.ShouldBeEmpty();
    }

    [Fact]
    public void Should_return_accesstoken_as_claims()
    {
        // http://jwtbuilder.jamiekurtz.com/
        // {
        //     "iss": "Online JWT Builder",
        //     "iat": 1668613420,
        //     "exp": 1700149420,
        //     "aud": "www.example.com",
        //     "sub": "jrocket@example.com",
        //     "GivenName": "Johnny",
        //     "Surname": "Rocket",
        //     "Email": "jrocket@example.com",
        //     "roles": [
        //     "Manager",
        //     "Project Administrator"
        //      ]
        // }
        
        const string accessTokenJwt =
            @"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2Njg2MTM0MjAsImV4cCI6MTcwMDE0OTQyMCwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoianJvY2tldEBleGFtcGxlLmNvbSIsIkdpdmVuTmFtZSI6IkpvaG5ueSIsIlN1cm5hbWUiOiJSb2NrZXQiLCJFbWFpbCI6Impyb2NrZXRAZXhhbXBsZS5jb20iLCJyb2xlcyI6Ik1hbmFnZXIifQ.Vgs8ZRdukLMr2u-AmhxNROvxiv7T1HHltv9EWFr77H0";

        var authResult = new AuthenticationResult
        {
            AccessToken = accessTokenJwt
        };
        
        authResult.AccessTokenClaims.ShouldNotBeEmpty();
        authResult.AccessTokenClaims.Count().ShouldBe(9);
        authResult.AccessTokenClaims.First(c => c.Type == "Email").Value.ShouldBe("jrocket@example.com");
    }
}